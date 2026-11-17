

using System.Text.RegularExpressions;

namespace NRO;
public class XuLiSettings
{
    public static bool OutCondition;
    private static bool GotoBuyTDLT;
    private static long lastTimeCheck;
    private static long lastTimeCheckDoTrain;
    private static long lastTimeCheckOther;
    private static long lastTimeuseItem;
    private static long lastTimeout_lag;
    public static int CountTimeOut;
    public static int TrangThai;
    public static int timeout_lag;
    public static int getMinutebaotri=9999;
    public static readonly DateTime Savetimeonline;
    public static TimeSpan ThoiGianDaHoatDong => DateTime.UtcNow - Savetimeonline;

    private static Dictionary<int, int> _pickedItemsCount = new Dictionary<int, int>();
    static XuLiSettings()
    {
        Savetimeonline = DateTime.UtcNow;
    }
    public static void Update()
    {
        if (!GotoBuyTDLT)
        {
            OutLag();
            if (AutoMap.isXmaping) AutoMap.isXmaping = false;
            AutoTrain.Update();
            if (Char.myCharz().meDead) return;
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

    public static void OutLag()
    {
        if (mSystem.currentTimeMillis() - lastTimeout_lag < 1000) return;
        lastTimeout_lag = mSystem.currentTimeMillis();
        if (TileMap.mapID != Char.myCharz().cgender+21)
        {
            timeout_lag = 0;
            return;
        }
        timeout_lag++;
        if(timeout_lag>=60)Program.Cleanup();
    }
    public static string TinhTrungBinhTrenGio(long tongSoLuong)
    {
        double soGio = ThoiGianDaHoatDong.TotalHours;

        // Tránh lỗi chia cho 0 hoặc Infinity khi hệ thống vừa khởi động
        if (soGio <= 0.016666666666)
        {
            return "0";
        }
        long tongSoLuongInt = (long)(tongSoLuong / soGio);
        return NinjaUtil.getMoneys(tongSoLuongInt);
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
        if (setting == null) return;
        if (setting.IsByTDLT) AutoBuyTDLT();
        if (TileMap.mapID == AutoTrain.gobackMapID)
        {
            AutoPick.isAutoPick = setting.PickItemType != AccountSetting.TypePickItem.NotPick;
            if (setting.IsClearBag) AutoVutItem();
            if (TileMap.mapID != setting.Map) return;
            if (setting.IsUseflag)
            {
                if (Char.myCharz().cFlag != setting.Flag) Service.gI().getFlag(1, (sbyte)setting.Flag);
            }
            else if (Char.myCharz().cFlag != 0) Service.gI().getFlag(1, 0);
            if (setting.IsUseItem) AutoUseItem();
        }
        if (setting.IsAutoPoint)
        {
            AutoPoint.isAutoPoint = true;
            AutoPoint.damageToAuto = setting.DamePoint;
            AutoPoint.hpToAuto = setting.HPPoint;
            AutoPoint.mpToAuto = setting.MPPoint;
        }
        //AutoPean.isAutoDonatePean= setting.PeansDonate;
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
        if (TileMap.mapID == AutoTrain.gobackMapID)
        {
            FindGLT();
            UseTDLT();
            UsePorata();
        }
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
    //public static void ResetPickedItems() => _pickedItemsCount.Clear();
    //public static int GetCurrentItemCount(int id)
    //{
    //    // TryGetValue giúp tránh lỗi crash nếu ID chưa từng được nhặt
    //    if (_pickedItemsCount.TryGetValue(id, out int count))
    //    {
    //        return count;
    //    }
    //    return 0;
    //}
    public static void CheckItemPick(int id)
    {
        var setting = MainMod.CurrentSetting;
        if (setting.IsOutMaxItem)
        {
            if (setting?.ListItemIds == null) return;
            if (setting.ListItemIds.Contains(id))
            {
                CountTimeOut = 0;
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
        if (CountTimeOut >= setting.MaxTime) SetStatus(1);
        CountTimeOut++;

    }
    private static void EnoughQuantity()
    {
        var setting = MainMod.CurrentSetting;
        for (int i = 0; i < setting.ListItemIds.Length; i++)
        {
            int idItem = setting.ListItemIds[i];
            if (QuantityItemBag(idItem) >= setting.MaxItem)
            {
                SetStatus(1);
                break;
            }
        }
    }
    public static string Get_listQuantity()
    {
        var setting = MainMod.CurrentSetting;
        string result = "";
        for (int i = 0; i < setting.ListItemIds.Length; i++)
        {
            int idItem = setting.ListItemIds[i];
            result += $"x{QuantityItemBag(idItem)}/{setting.MaxItem}\n";
        }
        return result;
    }
    public static void SetStatus(int status)
    {
        TrangThai = status;
        OutCondition = true;
    }

    public static void GetInfoMe(string s)
    {
        var setting = MainMod.CurrentSetting;
        if (s.ToLower().Contains("bảo trì"))
        {
            Match match = Regex.Match(s, "\\d+");
            int rs;
            if (match.Success && int.TryParse(match.Value, out rs))
            {
                if(rs <= 5 && setting.IsOutbaotri) SetStatus(1);
            }
        }
        if(s.ToLower().Contains("thể lực đã cạn"))
        {
            return;
        }
    }
    private static void DoTimer()
    {
        var setting = MainMod.CurrentSetting;
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

