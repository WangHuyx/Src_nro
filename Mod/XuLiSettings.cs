using QLTK;

namespace NRO;
public class XuLiSettings
{
    public static bool OutCondition;
    private static bool GotoBuyTDLT;
    private static long lastTimeCheck;
    private static long lastTimeCheckDoTrain;
    private static long lastTimeCheckOther;
    private static long lastTimeuseItem;
    private static int CountTimeOut;
    public static int TrangThai;

    private static Dictionary<int, int> _pickedItemsCount = new Dictionary<int, int>();
    static XuLiSettings()
    {
    }

    public static void Update()
    {
        if (!GotoBuyTDLT)
        {
            if(AutoMap.isXmaping) AutoMap.isXmaping = false;
            AutoTrain.Update();
            AutoItem.Update();
            AutoPean.Update();
            AutoSkill.Update();
            AutoPick.Update();
            AutoPoint.Update();
            if (mSystem.currentTimeMillis() - lastTimeCheck > 1000L)
            {
                lastTimeCheck = mSystem.currentTimeMillis();
                DoTimer();
            }
            if (mSystem.currentTimeMillis() - lastTimeCheckDoTrain > 100L)
            {
                lastTimeCheckDoTrain = mSystem.currentTimeMillis();
                DoTrain();
            }
        }
        if (mSystem.currentTimeMillis() - lastTimeCheckOther > 1500L)
        {
            lastTimeCheckOther = mSystem.currentTimeMillis();
            DoOther();
        }
    }
    private static int QuantityItemBag(int int_20)
    {
        for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        {
            Item item = Char.myCharz().arrItemBag[i];
            if (item != null && item.template.id == int_20)
            {
                return item.quantity;
            }
        }
        return 0;
    }
   //khác
    private static void DoOther()
    {
        var setting = MainMod.CurrentSetting;
        AutoPick.isAutoPick = ( setting != null && setting.PickItemType != AccountSetting.TypePickItem.NotPick);
        if(setting.IsClearBag) AutoVutItem();
        if(setting.IsByTDLT) AutoBuyTDLT();
        if (setting.IsUseflag)
        {
            if (Char.myCharz().cFlag != setting.Flag) Service.gI().getFlag(1, (sbyte)setting.Flag);
        }
        else if (Char.myCharz().cFlag != 0) Service.gI().getFlag(1, 0);
        if(setting.IsUseItem) AutoUseItem();
        if (setting.IsAutoPoint)
        {
            AutoPoint.isAutoPoint = true;
            AutoPoint.damageToAuto = setting.DamePoint;
            AutoPoint.hpToAuto = setting.HPPoint;
            AutoPoint.mpToAuto = setting.MPPoint;
        }
        AutoPean.isAutoDonatePean= setting.PeansDonate;
        AutoPean.isAutoRequestPean= setting.GetPeans;
    }
    private static void AutoUseItem()
    {
        var setting = MainMod.CurrentSetting;
        if (setting==null||setting.ListItemuse.Length <= 0)
        {
            return;
        }
        for(int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        {
            Item item = Char.myCharz().arrItemBag[i];
            if (item!=null&&setting.ListItemuse.Contains(item.template.id)&&!ItemTime.isExistItem(item.template.iconID))
            {
                Service.gI().useItem(0, 1, -1, (short)item.template.id);
                break;
            }
        }
    }
    private static void AutoVutItem()
    {
        var setting = MainMod.CurrentSetting;
        if (setting == null || setting.ListItemthow.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        {
            Item item = Char.myCharz().arrItemBag[i];
            if (item != null && setting.ListItemthow.Contains(item.template.id))
            {
				Service.gI().useItem(2, 1, (sbyte)i, -1);
                break;
            }
        }
    }
    private static int GetTimeTDLT()
    {
        int num = 0;
        int itemIcon = 4387;
        for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        {
            Item item = Char.myCharz().arrItemBag[i];
            if (item != null && item.template.id == 521)
            {
                itemIcon = item.template.iconID;
                num = item.itemOption[0].param;
                break;
            }
        }
        if (ItemTime.isExistItem(itemIcon))
        {
            for(int j = 0;j < Char.vItemTime.size(); j++)
            {
                ItemTime itemTime = (ItemTime)Char.vItemTime.elementAt(j);
                if (itemTime.idIcon == itemIcon)
                {
                    num += itemTime.minute;
                    break;
                }
            }
        }
        return num;
    }
    private static void AutoBuyTDLT()
    {
        var setting = MainMod.CurrentSetting;
        if (setting == null) return;
        int TimeTDLT = GetTimeTDLT();
        if(TimeTDLT <= 0) GotoBuyTDLT = true;
        if (!GotoBuyTDLT) return;
        int MapLang = Char.myCharz().cgender * 7;
        if (TileMap.mapID != MapLang)
        {
            AutoTrain.isAutoTrain = false;
            GameScr.isAutoPlay = true;
            AutoMap.StartRunToMapId(MapLang);
            return;
        }
        Npc npc = GameScr.findNPCInMap((short)(Char.myCharz().cgender + 7));
        if (npc!=null)
        {
            if(TimeTDLT >= setting.TDLTTime)
            {
                GotoBuyTDLT = false;
                return;
            }
            int idBuy = 521;
            if(setting.TDLTTime-TimeTDLT >=500) idBuy = 1524;
            else if (setting.TDLTTime - TimeTDLT >= 200) idBuy = 1523;
            if(Char.myCharz().npcFocus != npc) Char.myCharz().npcFocus = @npc;
            if(Char.myCharz().cx != npc.cx)AutoMap.TeleportTo(npc.cx, AutoMap.GetYGround(npc.cx));
            if (!GameCanvas.panel.isShow)
            {
                Service.gI().openMenu((sbyte)npc.template.npcTemplateId);
                Service.gI().menu((sbyte)npc.template.npcTemplateId, 0, 0);
                GameCanvas.gI().keyPressedz(-5);
                return;
            }
            Service.gI().buyItem(1, idBuy,0);
        }
    }
    //Train
    private static void DoTrain()
    {
        var setting = MainMod.CurrentSetting;
        AutoTrain.isGoBack = (setting.IsGobackTime||setting.IsGobackCondition);
        AutoTrain.SaveSecondGoback = setting.GobackTime;
        AutoTrain.gobackMapID = setting.Map;
        AutoTrain.isAvoidUnCommonMob = !setting.IsTrainBasicMob;
        AutoTrain.isAvoidSuperMob = !setting.IsTrainSuperMob;
        //if(TileMap.mapID == AutoTrain.gobackMapID && !AutoMap.isXmaping)AutoTrain.isAutoTrain = true;
        FindGLT();
        UseTDLT();
        UsePorata();
    }

    private static void FindGLT()
    {
        var setting = MainMod.CurrentSetting;
        lastTimeCheckDoTrain = mSystem.currentTimeMillis() + 3000L;

        if (setting.IsUseGLT && Char.myCharz().arrItemBody[6] == null)
        {
            for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
            {
                Item item = Char.myCharz().arrItemBag[i];
                if (item != null && item.template.type == 32)
                {
                    Service.gI().getItem(4, (sbyte)item.indexUI);
                    break;
                }
            }
        }
        else if (!setting.IsUseGLT && Char.myCharz().arrItemBody[6] != null)
        {
            Service.gI().getItem(5, 6);
        }

    }
    public static void UseTDLT()
    {
        var setting = MainMod.CurrentSetting;
        if (setting.IsUseTDLT && ItemTime.isExistItem(4387)) return;
        if (!setting.IsUseTDLT && !ItemTime.isExistItem(4387)) return;
        lastTimeCheckDoTrain = mSystem.currentTimeMillis() + 2000L;

        for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        {
            Item item = Char.myCharz().arrItemBag[i];
            if (item != null && item.template.id == 521)
            {
                Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
                break;
            }
        }
    }
    public static void UsePorata()
    {

        var setting = MainMod.CurrentSetting;
        if (setting.IsUseBT && Char.myCharz().isNhapThe) return;
        if (!setting.IsUseBT && !Char.myCharz().isNhapThe) return;
        lastTimeCheckDoTrain = mSystem.currentTimeMillis() + 2000L;

        for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        {
            Item item = Char.myCharz().arrItemBag[i];
            if (item != null && item.template.name.Contains("Porata"))
            {
                Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
                break;
            }
        }
    }
    // kiểm tra theo thời gian/ số lượng
    public static void ResetPickedItems() => _pickedItemsCount.Clear();
    public static int GetCurrentItemCount(int id)
    {
        // TryGetValue giúp tránh lỗi crash nếu ID chưa từng được nhặt
        if (_pickedItemsCount.TryGetValue(id, out int count))
        {
            return count;
        }
        return 0;
    }
    public static void CheckItemPick(int id)
    {
        var setting = MainMod.CurrentSetting;
        if (setting?.ListItemIds == null) return;
        if (setting.ListItemIds.Contains(id))
        {
            if(setting.IsOutMaxTime) CountTimeOut = 0;
            if (setting.IsOutMaxItem)
            {
                if (_pickedItemsCount.ContainsKey(id))
                {
                    _pickedItemsCount[id]++;
                }
                else
                {
                    _pickedItemsCount[id] = QuantityItemBag(id);                  
                }
            }
        }

    }
    private static void EnoughTimer()
    {
        var setting = MainMod.CurrentSetting;
        if (setting?.ListItemIds == null) return;

        //bool isConfigChanged = SaveQuantityLast.Length != setting.ListItemIds.Length;
        //if (isConfigChanged)
        //{
        //    SaveQuantityLast = new int[setting.ListItemIds.Length];
        //    SaveIdLast = new int[setting.ListItemIds.Length];
        //}

        if (CountTimeOut >= setting.MaxTime) SetStatus(1);
        CountTimeOut++;
        //Console.WriteLine(CountTimeOut.ToString());
        //for (int i = 0; i < setting.ListItemIds.Length; i++)
        //{
        //    int idItem = setting.ListItemIds[i];
        //    int currentQty = QuantityItemBag(idItem);
        //    if (idItem != SaveIdLast[i])
        //    {
        //        SaveIdLast[i] = idItem;
        //        SaveQuantityLast[i] = currentQty;
        //        CountTimeOut = 0; // Reset timer vì đây là vật phẩm mới cần theo dõi
        //        Cout.Log($"[CONFIG] Đã đổi sang Item ID: {idItem} tại slot {i}.");
        //        continue;
        //    }
        //    if (currentQty != SaveQuantityLast[i])
        //    {
        //        SaveQuantityLast[i] = currentQty;
        //        CountTimeOut = 0;
        //        Cout.Log($"[DEBUG] Item {idItem} thay đổi: {currentQty}. Reset Timer.");
        //    }
        //}
    }
    private static void EnoughQuantity()
    {
        var setting = MainMod.CurrentSetting;
        int @out = 1;
        for (int i = 0; i < setting.ListItemIds.Length; i++)
        {
            int idItem = setting.ListItemIds[i];
            if (GetCurrentItemCount(idItem) < setting.MaxItem)
            {
                @out = 0;
            }
        }
        SetStatus(@out);

    }
    public static void SetStatus(int status)
    {
        TrangThai = status;
        OutCondition = true;
    }
    private static void DoTimer()
    {
        var setting = MainMod.CurrentSetting;
        //Thoát khi đến giờ
        //if (!string.IsNullOrWhiteSpace(setting.TimeAutoOff))
        //{
        //    // 1. Tách chuỗi bằng dấu ',' thành mảng các khung giờ
        //    string[] timeSlots = setting.TimeAutoOff.Split(',');
        //    TimeSpan currentTime = DateTime.Now.TimeOfDay;

        //    foreach (string slot in timeSlots)
        //    {
        //        // 2. Trim() để xóa khoảng trắng thừa và TryParse để kiểm tra định dạng
        //        if (TimeSpan.TryParse(slot.Trim(), out TimeSpan targetTime))
        //        {
        //            // 3. So sánh giờ và phút
        //            if (currentTime.Hours == targetTime.Hours && currentTime.Minutes == targetTime.Minutes)
        //            {
        //               SetStatus(2);
        //                break; // Tìm thấy 1 khung giờ khớp là đủ, thoát vòng lặp ngay (Early Exit)
        //            }
        //        }
        //        else
        //        {
        //            // Nếu có ít nhất 1 khung giờ sai định dạng, báo lỗi status
        //            SetStatus(2);
        //        }
        //    }
        //}
        //Thoát khi đủ SM
        if (setting.IsOutMaxSM)
        {
            if (Char.myCharz().cPower >= setting.MaxSM) SetStatus(1);
        }
        //Thoát khi vật phẩm không nhặt được
        if (setting.IsOutMaxTime)
        {
            EnoughTimer();
        }
        //Thoát khi vật phẩm đủ số lượng
        if (setting.IsOutMaxItem)
        {
            EnoughQuantity();
        }
    }
}

