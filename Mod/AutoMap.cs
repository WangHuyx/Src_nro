using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace NRO;

public class AutoMap : IActionListener
{
    public class NextMap
    {
        public int MapID;
        public int Npc;
        public int[] Index;
        public int type;
        public object[] info;

        public NextMap(int mapnextID, int npcID, int[] index, int typemove, object[] captions)
        {
            MapID = mapnextID;
            Npc = npcID;
            Index = index;
            type = typemove;
            info = captions;
        }

        public void GotoMap()
        {
            switch (type)
            {
                case Type.next_move:
                    Waypoint wayPoint = GetWayPoint();
                    if (wayPoint != null) Enter(wayPoint);
                    break;

                case Type.moveto:
                    if (info != null && info.Length >= 2)
                    {
                        TeleportTo((int)info[0], (int)info[1]);
                        Service.gI().requestChangeMap();
                    }
                    break;

                case Type.npc_move:
                    npcMove(type);
                    break;
                case Type.textmenu_move:
                    npcMove(type);
                    break;

                case Type.useitem_move:
                    if (info != null && info.Length > 0)
                    {
                        int itemId = (int)info[0];
                        sbyte bagIndex = FindItemIndexInBag(itemId);
                        if (bagIndex != -1)
                        {
                            Service.gI().useItem(0, 1, bagIndex, -1);
                        }
                    }
                    break;
            }
        }

        private void npcMove(int moveType)
        {
            //if (GameCanvas.gameTick % 50 != 0) return;
            if (Npc == -1 || Index == null) return;
            if (!GameCanvas.menu.showMenu)
            {
                Service.gI().openMenu(Npc);
                return;
            }
            int menuSize = GameCanvas.menu.menuItems.size();
            for (int i = 0; i < menuSize; i++)
            {
                string caption = ((Command)GameCanvas.menu.menuItems.elementAt(i)).caption.Replace('\n', ' ').ToLower();
                if (caption.Contains("candy") || caption.Contains("cho kẹo") ||
                    caption.Contains("chúc mừng năm mới") || caption.Contains("happy new year"))
                {
                    longCloseClick = mSystem.currentTimeMillis();
                    GameCanvas.menu.menuSelectedItem = i;
                    GameCanvas.gI().keyPressedz(-5);
                    //GameCanvas.menu.doCloseMenu();
                    return;
                }
                if (caption.Contains("chúc mừng") || caption.Contains("merry"))
                {
                    for (int j = 0; j < menuSize; j++)
                    {
                        if (((Command)GameCanvas.menu.menuItems.elementAt(j)).caption.Contains("OK"))
                        {
                            longCloseClick = mSystem.currentTimeMillis();
                            GameCanvas.menu.menuSelectedItem = j;
                            GameCanvas.gI().keyPressedz(-5);
                            //GameCanvas.menu.doCloseMenu();
                            return;
                        }
                    }
                    return;
                }
            }
            // Với npc_move chỉ số dương thông thường: Gửi thẳng gói tin không cần chờ UI Menu hiện
            if (moveType == Type.npc_move && GameCanvas.menu.menuItems != null)
            {
                for (int k = 0; k < Index.Length; k++)
                {
                    longCloseClick = mSystem.currentTimeMillis();
                    Service.gI().confirmMenu(0, (sbyte)Index[k]);
                }
                return;
            }



            // Duyệt từng lớp menu cho textmenu_move
            for (int k = 0; k < Index.Length; k++)
            {
                if (Index[k] < 0)
                {
                    int captionIdx = (-Index[k]) - 1;
                    if (info != null && captionIdx < info.Length)
                    {
                        string menuText = info[captionIdx]?.ToString() ?? "";
                        for (int j = 0; j < GameCanvas.menu.menuItems.size(); j++)
                        {
                            string itemCap = ((Command)GameCanvas.menu.menuItems.elementAt(j)).caption.Replace('\n', ' ');
                            if (itemCap.IndexOf(menuText, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                longCloseClick = mSystem.currentTimeMillis();
                                Service.gI().confirmMenu(0, (sbyte)j);
                                GameCanvas.menu.doCloseMenu(); // Đóng UI menu sau khi đã chọn để giải phóng màn hình
                                return;
                            }
                        }
                    }
                }
                else
                {
                    longCloseClick = mSystem.currentTimeMillis();
                    Service.gI().confirmMenu(0, (sbyte)Index[k]);
                }
            }
            if (GameCanvas.menu.showMenu) GameCanvas.menu.doCloseMenu();
        }

        public Waypoint GetWayPoint()
        {
            for (int i = 0; i < TileMap.vGo.size(); i++)
            {
                Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
                if (waypoint.popup != null)
                {
                    string mapName = GetMapName(waypoint.popup);
                    if (GetMapName().Equals(mapName))
                    {
                        return waypoint;
                    }
                }
            }
            return null;
        }

        public string GetMapName()
        {
            return TileMap.mapNames[MapID];
        }

        public string GetMapName(PopUp popup)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < popup.says.Length; i++)
            {
                stringBuilder.Append(popup.says[i]);
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString().Trim();
        }

        public void Enter(Waypoint waypoint)
        {
            int num = ((waypoint.maxX < 60) ? 20 : ((waypoint.minX <= TileMap.pxw - 60) ? ((waypoint.minX + waypoint.maxX) / 2) : (TileMap.pxw - 20)));
            int maxY = waypoint.maxY;
            if (num == -1 || maxY == -1)
            {
                GameScr.info1.addInfo("Có lỗi xảy ra", 0);
                return;
            }
            TeleportTo(num, maxY);
            if (waypoint.isOffline)
            {
                Service.gI().getMapOffline();
                return;
            }
            Service.gI().requestChangeMap();
        }

        public void TeleportTo(int x, int y)
        {
            AutoMap.TeleportTo(x, y);
        }
    }

    public class Type
    {
        public const int next_move = 0;
        public const int npc_move = 1;
        public const int textmenu_move = 2;
        public const int moveto = 3;
        public const int useitem_move = 4;
        public const int capsule_move = 5;
    }

    public class MapData
    {
        public List<CoordinateMap> coordinate_maps { get; set; }
        public List<UseItemMap> useitem_maps { get; set; }
        public List<NpcMap> npc_maps { get; set; }
        public List<int[]> link_maps { get; set; }
        public List<MenuMap> menu_maps { get; set; }
    }

    public class CoordinateMap
    {
        public List<int> map { get; set; }
        public PointData fw { get; set; }
        public PointData bw { get; set; }
    }

    public class UseItemMap
    {
        public List<int> map { get; set; }
        public ItemAction fw { get; set; }
        public ItemAction bw { get; set; }
    }

    public class NpcMap
    {
        public List<int> map { get; set; }
        public NpcAction fw { get; set; }
        public NpcAction bw { get; set; }
    }

    public class NpcAction
    {
        public int npcId { get; set; }
        public int[] selectIndex { get; set; }
        public string[] captions { get; set; }
    }

    public class MenuMap
    {
        public string menu { get; set; }
        public int[] IDmap { get; set; }
    }

    public class PointData
    {
        public int cx { get; set; }
        public int cy { get; set; }
    }

    public class ItemAction
    {
        public int itemId { get; set; }
    }

    public static AutoMap _Instance;
    private static Dictionary<int, List<NextMap>> linkMaps;
    private static Dictionary<string, int[]> planetDictionary;

    public static bool isXmaping;
    public static int IdMapEnd;

    private static int[] wayPointMapLeft;
    private static int[] wayPointMapCenter;
    private static int[] wayPointMapRight;

    private static bool isEatChicken;
    private static bool isHarvestPean;
    private static bool isUseCapsule;
    private static bool isUsingCapsule;
    private static bool isOpeningPanel;
    private static long lastTimeOpenedPanel;
    private static bool isSaveData;
    private static long lastWaitTime;

    private static int[] idMapsTuongLai;
    public static long longCloseClick;
    public static bool XmapNrsd;
    public static long LastNextMap;
    public static long LongWaitNextMap;
    public static long Lastselectmap;
    public static bool isteleport;
    public static bool isXmapSpecial;

    private static Dictionary<string, List<int>> mapNameLookup = new();

    // Thêm 3 biến theo dõi trạng thái thực tế của Capsule (Đặt cùng khu vực khai báo biến static)
    private static int lastTrackedMapId = -1;
    private static int previousMapIdForCapsule = -1; // Lưu map "Về chỗ cũ"
    private static HashSet<int> syncedCapsuleMaps = null; // Lưu các map thực tế nick đã mở khóa trên bảng Capsule

    private static bool lockOnly;
    static AutoMap()
    {
        LongWaitNextMap = 2000L;
        linkMaps = new Dictionary<int, List<NextMap>>();
        planetDictionary = new Dictionary<string, int[]>();
        isEatChicken = true;
        isUseCapsule = true;
        idMapsTuongLai = new int[10] { 102, 92, 93, 94, 96, 97, 98, 99, 100, 103 };
        InitializeXmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "map_data.json"));
    }

    public static AutoMap getInstance()
    {
        if (_Instance == null)
        {
            _Instance = new AutoMap();
        }
        return _Instance;
    }

    public static void Update()
    {
        if (Char.myCharz().meDead)
        {
            lastWaitTime = mSystem.currentTimeMillis() + 1000L;
        }
        if (TileMap.mapID != lastTrackedMapId)
        {
            if (lastTrackedMapId != -1 && !isNRDMap(lastTrackedMapId) &&
                (lastTrackedMapId < 53 || lastTrackedMapId > 62) &&
                (lastTrackedMapId < 135 || lastTrackedMapId > 138) &&
                (lastTrackedMapId < 147 || lastTrackedMapId > 152))
            {
                previousMapIdForCapsule = lastTrackedMapId;
            }
            lastTrackedMapId = TileMap.mapID;
        }
        if (TileMap.mapID == IdMapEnd)
        {
            FinishXmap();
            return;
        }
        if (!isXmaping || Char.myCharz().cHP <= 0 || Char.myCharz().meDead) return;

        if (TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23)
        {
            if (isEatChicken)
            {
                for (int i = 0; i < GameScr.vItemMap.size(); i++)
                {
                    ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
                    if ((itemMap.playerId == Char.myCharz().charID || itemMap.playerId == -1) && itemMap.template.id == 74)
                    {
                        Char.myCharz().itemFocus = itemMap;
                        if (mSystem.currentTimeMillis() - lastWaitTime > 600L)
                        {
                            lastWaitTime = mSystem.currentTimeMillis();
                            Service.gI().pickItem(Char.myCharz().itemFocus.itemMapID);
                            break;
                        }
                        return;
                    }
                }
            }
            if (isHarvestPean && GameScr.hpPotion < 10 && GameScr.gI().magicTree.currPeas > 0 && mSystem.currentTimeMillis() - lastWaitTime > 1000L)
            {
                lastWaitTime = mSystem.currentTimeMillis();
                Service.gI().openMenu(4);
                Service.gI().menu(4, 0, 0);
            }
        }

        long waitDelay = isUsingCapsule ? 200L : LongWaitNextMap;
        if (mSystem.currentTimeMillis() - lastWaitTime <= 1000L || mSystem.currentTimeMillis() - LastNextMap <= waitDelay) return;

        LastNextMap = mSystem.currentTimeMillis();

        if (isFutureMap(IdMapEnd) && !isFutureMap(TileMap.mapID))
        {
            if (GameScr.findNPCInMap(38) == null)
            {
                if (TileMap.mapID == 27 || TileMap.mapID == 29)
                {
                    UpdateXmap(28);
                    return;
                }
                if (TileMap.mapID == 28)
                {
                    if (Char.myCharz().cx < TileMap.pxw / 2) UpdateXmap(29);
                    else UpdateXmap(27);
                    return;
                }
            }
        }

        UpdateXmap(IdMapEnd);
    }

    public void perform(int idAction, object p)
    {
        switch (idAction)
        {
            case 1:
                ShowPlanetMenu();
                break;
            case 2:
                isEatChicken = !isEatChicken;
                GameScr.info1.addInfo("Ăn Đùi Gà: " + (isEatChicken ? "Bật" : "Tắt"), 0);
                break;
            case 3:
                isHarvestPean = !isHarvestPean;
                GameScr.info1.addInfo("Thu Đậu: " + (isHarvestPean ? "Bật" : "Tắt"), 0);
                break;
            case 4:
                isUseCapsule = !isUseCapsule;
                GameScr.info1.addInfo("Sử Dụng Capsule: " + (isUseCapsule ? "Bật" : "Tắt"), 0);
                break;
            case 5:
                isSaveData = !isSaveData;
                GameScr.info1.addInfo("Lưu Cài Đặt: " + (isSaveData ? "Bật" : "Tắt"), 0);
                break;
            case 6:
                ShowMapsMenu((int[])p);
                break;
            case 7:
                IdMapEnd = (int)p;
                if (!isNRDMap((int)p))
                {
                    isXmaping = true;
                }
                break;
        }
    }

    public static void ShowMenu()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command("Load Map", getInstance(), 1, null));
        myVector.addElement(new Command(string.Format("Ăn Đùi Gà\n{0}", isEatChicken ? "[ON]" : "[OFF]"), getInstance(), 2, null));
        myVector.addElement(new Command(string.Format("Thu Đậu\n{0}", isHarvestPean ? "[ON]" : "[OFF]"), getInstance(), 3, null));
        myVector.addElement(new Command(string.Format("Sử Dụng Capsule\n{0}", isUseCapsule ? "[ON]" : "[OFF]"), getInstance(), 4, null));
        myVector.addElement(new Command(string.Format("Lưu Cài Đặt\n{0}", isSaveData ? "[ON]" : "[OFF]"), getInstance(), 5, null));
        GameCanvas.menu.startAt(myVector, 3);
    }

    private static void ShowPlanetMenu()
    {
        MyVector myVector = new MyVector();
        foreach (KeyValuePair<string, int[]> item in planetDictionary)
        {
            myVector.addElement(new Command(item.Key, getInstance(), 6, item.Value));
        }
        GameCanvas.menu.startAt(myVector, 3);
    }

    private static void ShowMapsMenu(int[] mapIDs)
    {
        MyVector myVector = new MyVector();
        for (int i = 0; i < mapIDs.Length; i++)
        {
            if ((Char.myCharz().cgender != 0 || (mapIDs[i] != 22 && mapIDs[i] != 23)) &&
                (Char.myCharz().cgender != 1 || (mapIDs[i] != 21 && mapIDs[i] != 23)) &&
                (Char.myCharz().cgender != 2 || (mapIDs[i] != 21 && mapIDs[i] != 22)))
            {
                myVector.addElement(new Command(GetMapName(mapIDs[i]), getInstance(), 7, mapIDs[i]));
            }
        }
        GameCanvas.menu.startAt(myVector, 3);
    }

    public static void StartRunToMapId(int mapID)
    {
        isXmaping = true;
        IdMapEnd = mapID;
    }

    private static void FinishXmap()
    {
        isXmaping = false;
        isUsingCapsule = false;
        isOpeningPanel = false;
    }

    // Cập nhật động liên kết từ Siêu thị (84) về đúng Trạm tàu vũ trụ theo hệ nhân vật (24, 25, 26)
    private static void AddLinkMapOnly()
    {
        if (lockOnly) return;
        lockOnly = true;
        linkMaps[84] = new List<NextMap>
        {
            new NextMap(24 + Char.myCharz().cgender, 10, new int[] { 0 }, Type.npc_move, null)
        };
    }

    private static sbyte FindItemIndexInBag(int itemId)
    {
        if (Char.myCharz()?.arrItemBag == null) return -1;
        for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        {
            Item item = Char.myCharz().arrItemBag[i];
            if (item != null && item.template.id == itemId)
            {
                return (sbyte)item.indexUI;
            }
        }
        return -1;
    }

    private static bool CanUseCapsuleHere(out sbyte capsuleBagIndex)
    {
        capsuleBagIndex = -1;
        if (!isUseCapsule || isOpeningPanel) return false;

        int m = TileMap.mapID;
        // Các map phụ bản/đặc biệt bị khóa dùng Capsule
        //if (isNRDMap(m) || (m >= 53 && m <= 62) || (m >= 135 && m <= 138) || (m >= 147 && m <= 152) || m == 113 || m == 129)
        //    return false;

        capsuleBagIndex = FindItemIndexInBag(194);
        if (capsuleBagIndex == -1)
        {
            capsuleBagIndex = FindItemIndexInBag(193);
        }
        return capsuleBagIndex != -1;
    }

    // - Rừng Bamboo = 27 (Trái Đất)
    // - Vách núi đen = 20 (Xayda)
    // - Núi khỉ đỏ = 79 (Fide)
    private static List<int> GetEstimatedCapsuleMaps()
    {
        List<int> result = new List<int>();

        // Nếu đã từng mở bảng Capsule, dùng chính xác danh sách map mà nick này thực sự có
        if (syncedCapsuleMaps != null && syncedCapsuleMaps.Count > 0)
        {
            result.AddRange(syncedCapsuleMaps);
        }
        else
        {
            // Lần đầu tiên chưa mở bảng thì mới dùng danh sách 16 map chuẩn
            int gender = Char.myCharz().cgender;
            result.AddRange(new int[]
            {
            21 + gender, 24 + gender,
            0, 5, 27, 45, 47, 52,
            7, 13,
            14, 19, 20,
            79, 84, 154
            });
        }

        // Bổ sung thêm map "Về chỗ cũ" nếu hợp lệ
        if (previousMapIdForCapsule != -1 && !result.Contains(previousMapIdForCapsule))
        {
            result.Add(previousMapIdForCapsule);
        }

        // Loại bỏ chính map đang đứng (Ví dụ: đang ở Siêu thị 84 thì loại 84 ra ngay lập tức)
        result.RemoveAll(m => m == TileMap.mapID);
        return result;
    }

    private static int ResolveCapsuleMapName(string rawPanelName)
    {
        if (string.IsNullOrEmpty(rawPanelName)) return -1;
        string cleanName = rawPanelName.Replace("Về chỗ cũ: ", "").Trim();

        if (cleanName.Equals("Về nhà", StringComparison.OrdinalIgnoreCase))
            return 21 + Char.myCharz().cgender;

        if (cleanName.Equals("Trạm tàu vũ trụ", StringComparison.OrdinalIgnoreCase))
            return 24 + Char.myCharz().cgender;

        for (int i = 0; i < TileMap.mapNames.Length; i++)
        {
            if (cleanName.Equals(TileMap.mapNames[i], StringComparison.OrdinalIgnoreCase))
                return i;
        }

        if (mapNameLookup.TryGetValue(cleanName, out var ids) && ids.Count > 0)
            return ids[0];

        return -1;
    }

    private static int GetEdgeCost(NextMap edge)
    {
        return edge.type switch
        {
            Type.next_move => 10,
            Type.moveto => 10,
            Type.npc_move or Type.textmenu_move => 15 + (edge.Index != null ? edge.Index.Length * 5 : 0),
            Type.capsule_move => 25,
            Type.useitem_move => 50,
            _ => 10
        };
    }

    private static void UpdateXmap(int mapID)
    {

        try
        {
            AddLinkMapOnly();
            if (isUsingCapsule)
            {
                Stopwatch sw = Stopwatch.StartNew();
                if (GameCanvas.panel.mapNames == null || GameCanvas.panel.mapNames.Length == 0)
                {
                    if (mSystem.currentTimeMillis() - lastTimeOpenedPanel > 3000L)
                    {
                        isUsingCapsule = false;
                        isOpeningPanel = true;
                    }
                    return;
                }

                if (mSystem.currentTimeMillis() - lastTimeOpenedPanel < 500L)
                    return;

                Dictionary<int, int> panelMapToIndex = new Dictionary<int, int>();
                syncedCapsuleMaps = new HashSet<int>();
                previousMapIdForCapsule = -1; // Reset trước khi quét lại từ bảng thực tế

                for (int i = 0; i < GameCanvas.panel.mapNames.Length; i++)
                {
                    string rawName = GameCanvas.panel.mapNames[i];
                    if (string.IsNullOrEmpty(rawName)) continue;

                    int resolvedId = ResolveCapsuleMapName(rawName);
                    if (resolvedId != -1)
                    {
                        if (!panelMapToIndex.ContainsKey(resolvedId))
                            panelMapToIndex[resolvedId] = i;

                        if (!rawName.StartsWith("Về chỗ cũ", StringComparison.OrdinalIgnoreCase))
                            syncedCapsuleMaps.Add(resolvedId);
                        else
                            previousMapIdForCapsule = resolvedId;
                    }
                }

                // Bổ sung lại chính map đang đứng vào tập đã mở khóa (vì đang đứng tại đó nên Server tạm ẩn đi)
                int gender = Char.myCharz().cgender;
                int[] standardCaps = { 21 + gender, 24 + gender, 0, 5, 27, 45, 47, 52, 7, 13, 14, 19, 20, 79, 84, 154 };
                if (Array.IndexOf(standardCaps, TileMap.mapID) >= 0)
                {
                    syncedCapsuleMaps.Add(TileMap.mapID);
                }

                if (panelMapToIndex.Count > 0)
                {
                    // QUAN TRỌNG: Đưa cả các map trên bảng Capsule VÀ map hiện tại (TileMap.mapID) vào so sánh
                    // để tránh việc bay lùi về map xa hơn nếu map mục tiêu trên bảng chưa được mở khóa
                    List<int> candidateStarts = new List<int>(panelMapToIndex.Keys);
                    if (!candidateStarts.Contains(TileMap.mapID))
                    {
                        candidateStarts.Add(TileMap.mapID);
                    }

                    int[] bestPath = FindWay(mapID, candidateStarts.ToArray(), allowVirtualCapsule: false);
                    if (bestPath != null && bestPath.Length > 0)
                    {
                        int chosenStartMap = bestPath[0];
                        isUsingCapsule = false;
                        isOpeningPanel = true;

                        // Nếu đi bộ từ map đang đứng tối ưu hơn (hoặc bằng) mọi lựa chọn trên bảng Capsule -> Đóng bảng, đi bộ!
                        if (chosenStartMap == TileMap.mapID || !panelMapToIndex.ContainsKey(chosenStartMap))
                        {
                            //GameCanvas.panel?.hide();
                            return;
                        }

                        int panelIndex = panelMapToIndex[chosenStartMap];
                        lastTimeOpenedPanel = mSystem.currentTimeMillis();
                        Service.gI().requestMapSelect(panelIndex);
                        return;
                    }
                }

                isUsingCapsule = false;
                isOpeningPanel = true;
                //GameCanvas.panel?.hide();
                
                return;
            }
            // PHA 1: Tìm đường tổng quát trên đồ thị trọng số
            int[] array = FindWay(mapID);
            if (array == null || array.Length < 2)
            {
                GameScr.info1.addInfo("Không thể tìm thấy đường đi", 0);
                return;
            }

            int currentMap = array[0];
            int nextMap = array[1];
            if (ShouldTakeCapsuleForFirstStep(currentMap, nextMap) && CanUseCapsuleHere(out sbyte capsuleIdx))
            {
                GameCanvas.panel.mapNames = null;
                isUsingCapsule = true;
                isOpeningPanel = false;
                lastTimeOpenedPanel = mSystem.currentTimeMillis();
                Service.gI().useItem(0, 1, capsuleIdx, -1);
                return;
            }
            if (TileMap.mapID == currentMap && !Char.ischangingMap && !Controller.isStopReadMessage)
            {
                Goto(nextMap);
            }
            
        }
        catch (Exception ex)
        {
            GameScr.info1.addInfo("Lỗi Xmap: " + ex.Message, 0);
        }
        
    }

    private static bool ShouldTakeCapsuleForFirstStep(int currentMap, int nextMap)
    {
        if (!CanUseCapsuleHere(out _)) return false;

        if (!GetEstimatedCapsuleMaps().Contains(nextMap)) return false;

        if (linkMaps.TryGetValue(currentMap, out var edges))
        {
            foreach (var e in edges)
            {
                if (e.MapID == nextMap && GetEdgeCost(e) <= 25)
                    return false;
            }
        }
        return true;
    }

    public static void Info(string text)
    {
        if (isXmaping)
        {
            List<string> list = new List<string>
            {
                "Bạn chưa thể đến khu vực này",
                "Bang hội phải có từ 5 thành viên mới được tham gia",
                "Chỉ tiếp các bang hội, miễn tiếp khách vãng lai",
                "Gia nhập bang hội trên 2 ngày mới được tham gia",
                "Có lỗi xảy ra vui lòng thử lại sau.",
                "Chức năng chỉ dành cho bang chủ",
                "Yêu cầu sức mạnh lớn hơn"
            };
            if (list.Contains(text))
            {
                FinishXmap();
                GameScr.info1.addInfo("Đã huỷ Xmap", 0);
            }
        }
    }

    public static void LoadMapLeft() => LoadMap(0);
    public static void LoadMapCenter() => LoadMap(2);
    public static void LoadMapRight() => LoadMap(1);

    private static void AddLinkMapsXmap(params int[] link)
    {
        for (int i = 0; i < link.Length; i++)
        {
            if (!linkMaps.ContainsKey(link[i]))
            {
                linkMaps.Add(link[i], new List<NextMap>());
            }
            if (i != 0)
            {
                linkMaps[link[i]].Add(new NextMap(link[i - 1], -1, new int[] { -1 }, Type.next_move, null));
            }
            if (i != link.Length - 1)
            {
                linkMaps[link[i]].Add(new NextMap(link[i + 1], -1, new int[] { -1 }, Type.next_move, null));
            }
        }
    }

    private static void AddCoordinateLinkMapsXmap(int mapA, int mapB, int fwX, int fwY, int bwX, int bwY)
    {
        if (!linkMaps.ContainsKey(mapA))
        {
            linkMaps.Add(mapA, new List<NextMap>());
        }
        linkMaps[mapA].Add(new NextMap(mapB, -1, new int[] { -1 }, Type.moveto, new object[] { fwX, fwY }));

        if (!linkMaps.ContainsKey(mapB))
        {
            linkMaps.Add(mapB, new List<NextMap>());
        }
        linkMaps[mapB].Add(new NextMap(mapA, -1, new int[] { -1 }, Type.moveto, new object[] { bwX, bwY }));
    }

    private static void AddNPCLinkMapsXmap(int currentMapID, int nextMapID, int npcID, int[] select, string[] captions)
    {
        if (!linkMaps.ContainsKey(currentMapID))
        {
            linkMaps.Add(currentMapID, new List<NextMap>());
        }
        int moveType = (captions != null && captions.Length > 0) ? Type.textmenu_move : Type.npc_move;
        linkMaps[currentMapID].Add(new NextMap(nextMapID, npcID, select, moveType, captions));
    }

    private static void AddLinkMapsUseItemXmap(int nextMapID, int IDitem)
    {
        if (!linkMaps.ContainsKey(-1))
        {
            linkMaps.Add(-1, new List<NextMap>());
        }
        linkMaps[-1].Add(new NextMap(nextMapID, -1, new int[] { -1 }, Type.useitem_move, new object[] { IDitem }));
    }

    private static void Goto(int mapID)
    {
        if (linkMaps.TryGetValue(TileMap.mapID, out var localEdges))
        {
            foreach (NextMap item in localEdges)
            {
                if (item.MapID == mapID)
                {
                    item.GotoMap();
                    return;
                }
            }
        }

        if (linkMaps.TryGetValue(-1, out var globalItemEdges))
        {
            foreach (NextMap item in globalItemEdges)
            {
                if (item.MapID == mapID)
                {
                    item.GotoMap();
                    return;
                }
            }
        }

        GameScr.info1.addInfo("Không thể thực hiện", 0);
    }

    private static int[] FindWay(int mapID)
    {
        return FindWay(mapID, new int[] { TileMap.mapID }, allowVirtualCapsule: true);
    }

    private static int[] FindWay(int mapIDEnd, int[] mapIDStarts, bool allowVirtualCapsule = false)
    {
        if (mapIDStarts == null || mapIDStarts.Length == 0) return null;

        Dictionary<int, int> parentTree = new Dictionary<int, int>();
        Dictionary<int, int> minCost = new Dictionary<int, int>();
        PriorityQueue<int, int> pq = new PriorityQueue<int, int>();

        foreach (int startID in mapIDStarts)
        {
            if (startID == mapIDEnd) return new int[] { startID };
            if (!minCost.ContainsKey(startID))
            {
                parentTree[startID] = -1;
                minCost[startID] = 0;
                pq.Enqueue(startID, 0);
            }
        }

        bool canUseCap = allowVirtualCapsule && CanUseCapsuleHere(out _);
        List<int> capTargets = canUseCap ? GetEstimatedCapsuleMaps() : null;

        while (pq.Count > 0)
        {
            pq.TryDequeue(out int currentMap, out int currentCost);

            if (currentMap == mapIDEnd)
            {
                return ReconstructPath(parentTree, mapIDEnd);
            }

            if (currentCost > minCost[currentMap]) continue;

            List<NextMap> neighbors = new List<NextMap>();
            if (linkMaps.TryGetValue(currentMap, out var adj))
                neighbors.AddRange(adj);

            if (linkMaps.TryGetValue(-1, out var itemEdges))
                neighbors.AddRange(itemEdges);

            if (canUseCap && currentMap == TileMap.mapID)
            {
                foreach (int capMapId in capTargets)
                {
                    if (capMapId != currentMap)
                    {
                        neighbors.Add(new NextMap(capMapId, -1, null, Type.capsule_move, null));
                    }
                }
            }

            foreach (NextMap next in neighbors)
            {
                if (next.type == Type.useitem_move)
                {
                    if (next.info == null || next.info.Length == 0) continue;
                    int reqItemId = (int)next.info[0];
                    if (FindItemIndexInBag(reqItemId) == -1) continue;
                }

                if (Char.myCharz().taskMaint.taskId <= 30 && next.MapID >= 105 && next.MapID <= 110)
                    continue;

                if (currentMap == 102 && next.MapID == 24 &&
                    parentTree.TryGetValue(102, out int prevMap) &&
                    (prevMap == 27 || prevMap == 28 || prevMap == 29))
                {
                    continue;
                }

                int newCost = currentCost + GetEdgeCost(next);
                if (!minCost.TryGetValue(next.MapID, out int oldCost) || newCost < oldCost)
                {
                    minCost[next.MapID] = newCost;
                    parentTree[next.MapID] = currentMap;
                    pq.Enqueue(next.MapID, newCost);
                }
            }
        }

        return null;
    }

    private static int[] ReconstructPath(Dictionary<int, int> parentTree, int mapIDEnd)
    {
        List<int> path = new List<int>();
        int curr = mapIDEnd;
        while (curr != -1)
        {
            path.Add(curr);
            curr = parentTree[curr];
        }
        path.Reverse();
        return path.ToArray();
    }

    private static string GetMapName(int mapID)
    {
        return mapID switch
        {
            113 => string.Concat(new object[3] { "Siêu hạng\n[", mapID, "]" }),
            129 => TileMap.mapNames[mapID] + " 23\n[" + mapID + "]",
            _ => TileMap.mapNames[mapID] + "\n[" + mapID + "]",
        };
    }

    private static void LoadWaypointsInMap()
    {
        ResetSavedWaypoints();
        int num = TileMap.vGo.size();
        if (num != 2)
        {
            for (int i = 0; i < num; i++)
            {
                Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
                bool specical = TileMap.mapID == 155;

                if (waypoint.maxX < 60)
                {
                    wayPointMapLeft[0] = waypoint.minX + 15;
                    wayPointMapLeft[1] = waypoint.maxY;
                }
                else if (waypoint.maxX > TileMap.pxw - 60)
                {
                    wayPointMapRight[0] = waypoint.maxX - 15;
                    wayPointMapRight[1] = waypoint.maxY - (specical ? 24 : 0);
                }
                else
                {
                    wayPointMapCenter[0] = waypoint.minX + 15;
                    wayPointMapCenter[1] = waypoint.maxY;
                }
            }
            return;
        }

        Waypoint waypoint2 = (Waypoint)TileMap.vGo.elementAt(0);
        Waypoint waypoint3 = (Waypoint)TileMap.vGo.elementAt(1);
        if ((waypoint2.maxX < 60 && waypoint3.maxX < 60) || (waypoint2.minX > TileMap.pxw - 60 && waypoint3.minX > TileMap.pxw - 60))
        {
            wayPointMapLeft[0] = waypoint2.minX + 15;
            wayPointMapLeft[1] = waypoint2.maxY;
            wayPointMapRight[0] = waypoint3.maxX - 15;
            wayPointMapRight[1] = waypoint3.maxY;
        }
        else if (waypoint2.maxX < waypoint3.maxX)
        {
            wayPointMapLeft[0] = waypoint2.minX + 15;
            wayPointMapLeft[1] = waypoint2.maxY;
            wayPointMapRight[0] = waypoint3.maxX - 15;
            wayPointMapRight[1] = waypoint3.maxY;
        }
        else
        {
            wayPointMapLeft[0] = waypoint3.minX + 15;
            wayPointMapLeft[1] = waypoint3.maxY;
            wayPointMapRight[0] = waypoint2.maxX - 15;
            wayPointMapRight[1] = waypoint2.maxY;
        }
    }

    public static int GetYGround(int x)
    {
        int num = 50;
        int num2 = 0;
        while (num2 < 30)
        {
            num2++;
            num += 24;
            if (TileMap.tileTypeAt(x, num, 2))
            {
                if (num % 24 != 0)
                {
                    num -= num % 24;
                }
                break;
            }
        }
        return num;
    }

    public static void TeleportTo(int x, int y)
    {
        if (Char.myCharz().isLockMove && Char.myCharz().npcFocus == null)
        {
            GameScr.info1.addInfo("Đang không thể di chuyển!", 0);
            return;
        }
        Char.myCharz().cx = x;
        Char.myCharz().cy = y;
        Service.gI().charMove();
        if (!GameScr.canAutoPlay)
        {
            Char.myCharz().cx = x;
            Char.myCharz().cy = y + 1;
            Service.gI().charMove();
            Char.myCharz().cx = x;
            Char.myCharz().cy = y;
            Service.gI().charMove();
        }
    }

    private static void ResetSavedWaypoints()
    {
        wayPointMapLeft = new int[2];
        wayPointMapCenter = new int[2];
        wayPointMapRight = new int[2];
    }

    private static bool isNRDMap(int mapID)
    {
        return mapID >= 85 && mapID <= 91;
    }

    private static bool isFutureMap(int mapID)
    {
        for (int i = 0; i < idMapsTuongLai.Length; i++)
        {
            if (idMapsTuongLai[i] == mapID)
            {
                return true;
            }
        }
        return false;
    }

    private static bool itemNRD(ItemMap mapID)
    {
        return mapID.template.id >= 372 && mapID.template.id <= 378;
    }

    private static void LoadMap(int position)
    {
        if (mSystem.currentTimeMillis() - LastNextMap <= LongWaitNextMap)
        {
            return;
        }
        LastNextMap = mSystem.currentTimeMillis();
        if (isNRDMap(TileMap.mapID))
        {
            TeleportInNRDMap(position);
            return;
        }
        LoadWaypointsInMap();
        switch (position)
        {
            case 0:
                if (wayPointMapLeft[0] != 0 && wayPointMapLeft[1] != 0)
                    TeleportTo(wayPointMapLeft[0], wayPointMapLeft[1]);
                else
                    TeleportTo(60, GetYGround(60));
                break;
            case 1:
                if (wayPointMapRight[0] != 0 && wayPointMapRight[1] != 0)
                    TeleportTo(wayPointMapRight[0], wayPointMapRight[1]);
                else
                    TeleportTo(TileMap.pxw - 60, GetYGround(TileMap.pxw - 60));
                break;
            case 2:
                if (wayPointMapCenter[0] != 0 && wayPointMapCenter[1] != 0)
                    TeleportTo(wayPointMapCenter[0], wayPointMapCenter[1]);
                else
                    TeleportTo(TileMap.pxw / 2, GetYGround(TileMap.pxw / 2));
                break;
        }
        if (Char.myCharz().isInEnterOfflinePoint() != null)
        {
            Service.gI().getMapOffline();
        }
        else if (Char.myCharz().isInEnterOnlinePoint() != null)
        {
            Service.gI().requestChangeMap();
        }
    }

    private static void TeleportInNRDMap(int position)
    {
        switch (position)
        {
            case 0:
                TeleportTo(60, GetYGround(60));
                break;
            case 1:
                TeleportTo(TileMap.pxw - 60, GetYGround(TileMap.pxw - 60));
                break;
            case 2:
                for (int i = 0; i < GameScr.vItemMap.size(); i++)
                {
                    ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
                    if (itemNRD(itemMap))
                    {
                        TeleportTo(itemMap.x, itemMap.y - 3);
                        break;
                    }
                }
                for (int j = 0; j < GameScr.vNpc.size(); j++)
                {
                    Npc npc = (Npc)GameScr.vNpc.elementAt(j);
                    if (npc.template.npcTemplateId >= 30 && npc.template.npcTemplateId <= 36)
                    {
                        TeleportTo(npc.cx, npc.cy - 3);
                        break;
                    }
                }
                break;
        }
    }

    public static sbyte FindCT(int id)
    {
        for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        {
            if (Char.myCharz().arrItemBag[i] != null && Char.myCharz().arrItemBag[i].template.id == id)
            {
                return (sbyte)i;
            }
        }
        return -1;
    }

    public static void TeleVip(int CharID)
    {
        if (isteleport)
        {
            isteleport = false;
            return;
        }
        isteleport = true;
        Item[] arrItemBody = Char.myCharz().arrItemBody;
        if (FindCT(592 + Char.myCharz().cgender) == -1 && (arrItemBody[5] == null || !arrItemBody[5].template.name.Contains("Yardrat")))
        {
            GameScr.info1.addInfo("Không tìm thấy Yardat!", 0);
            isteleport = false;
            return;
        }
        if (!global::Char.ischangingMap && !Controller.isStopReadMessage)
        {
            sbyte idYD = FindCT(592 + Char.myCharz().cgender);
            if (arrItemBody[5] == null)
            {
                GameScr.info1.addInfo("Đang mặc yardat", 0);
                Service.gI().getItem(4, idYD);
                Service.gI().gotoPlayer(CharID);
                isteleport = false;
                return;
            }
            if (!arrItemBody[5].template.name.Contains("Yardrat"))
            {
                GameScr.info1.addInfo("Đang mặc yardat", 0);
                Service.gI().getItem(4, idYD);
                Service.gI().gotoPlayer(CharID);
                Service.gI().getItem(4, idYD);
                isteleport = false;
                return;
            }
            Service.gI().gotoPlayer(CharID);
        }
        isteleport = false;
    }

    private static int RequestMapnrsd(int p)
    {
        switch (p)
        {
            case 85: return 1;
            case 86: return 2;
            case 87: return 3;
            case 88: return 4;
            case 89: return 5;
            case 90: return 6;
            case 91: return 7;
            default:
                MainMod.isAutoEnterNRDMap = false;
                MainMod.isOpenMenuNPC = MainMod.isAutoEnterNRDMap;
                return -1;
        }
    }

    public static void InitMapLookup(string name, int id)
    {
        if (string.IsNullOrEmpty(name)) return;
        if (!mapNameLookup.TryGetValue(name, out var list))
        {
            list = new List<int>();
            mapNameLookup[name] = list;
        }
        if (!list.Contains(id))
        {
            list.Add(id);
        }
    }

    private static void InitializeXmap(string jsonPath)
    {
        try
        {
            string jsonContent = File.ReadAllText(jsonPath);
            MapData data = JsonSerializer.Deserialize(jsonContent, AppConfigJsonContext.Default.MapData);
            if (data == null) return;

            if (data.coordinate_maps != null)
            {
                foreach (var entry in data.coordinate_maps)
                {
                    AddCoordinateLinkMapsXmap(
                        entry.map[0],
                        entry.map[1],
                        entry.fw.cx,
                        entry.fw.cy,
                        entry.bw.cx,
                        entry.bw.cy
                    );
                }
            }

            if (data.link_maps != null)
            {
                foreach (var link in data.link_maps)
                {
                    AddLinkMapsXmap(link);
                }
            }

            if (data.npc_maps != null)
            {
                foreach (var nMap in data.npc_maps)
                {
                    int mapStart = nMap.map[0];
                    int mapEnd = nMap.map[1];

                    if (nMap.fw != null)
                        AddNPCLinkMapsXmap(mapStart, mapEnd, nMap.fw.npcId, nMap.fw.selectIndex, nMap.fw.captions);

                    if (nMap.bw != null)
                        AddNPCLinkMapsXmap(mapEnd, mapStart, nMap.bw.npcId, nMap.bw.selectIndex, nMap.bw.captions);
                }
            }

            if (data.useitem_maps != null)
            {
                foreach (var iMap in data.useitem_maps)
                {
                    foreach (int startMap in iMap.map)
                    {
                        if (iMap.fw != null) AddLinkMapsUseItemXmap(startMap, iMap.fw.itemId);
                        if (iMap.bw != null) AddLinkMapsUseItemXmap(startMap, iMap.bw.itemId);
                    }
                }
            }

            if (data.menu_maps != null)
            {
                foreach (var menuMap in data.menu_maps)
                {
                    planetDictionary[menuMap.menu] = menuMap.IDmap;
                }
            }
        }
        catch (Exception) { }
    }
}