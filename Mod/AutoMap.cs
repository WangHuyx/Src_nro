using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NRO;

public class AutoMap : IActionListener
{
	public class NextMap
	{
		public int MapID;

		public int Npc;

		public int[] Index;

		private int type;

		private object[] info;

        public NextMap(int mapnextID, int npcID, int[] index, int typemove, object[] captions)
		{
			MapID = mapnextID;
			Npc = npcID;
			Index = index;
            type= typemove;
			info = captions;
        }

		public void GotoMap()
		{
			if (Index == null) return;
			switch(type)
			{
				case Type.next_move:
                    Waypoint wayPoint = GetWayPoint();
                    if (wayPoint != null) Enter(wayPoint);
                    break;
                case Type.moveto:
                    Waypoint wayPoint2 = GetWayPoint();
					if (wayPoint2 != null)
					{
						TeleportTo((int)info[0], (int)info[1]);
						ChangeMap(wayPoint2);
					}
                    break;
				case Type.npc_move:
					npcMove(Type.npc_move);
                    break;
				case Type.textmenu_move:
					npcMove(Type.textmenu_move);
                    break;
				case Type.useitem_move:
					break;
            }
		}

		private void npcMove(int type)
		{
            if (Npc == -1 || Index == null)
            {
                return;
            }
            Service.gI().openMenu(Npc);
            for (int i = 0; i < GameCanvas.menu.menuItems.size(); i++)
            {
                string caption = ((Command)GameCanvas.menu.menuItems.elementAt(i)).caption.Replace('\n', ' ').ToLower();
                if (caption.Contains("refuse candy") || caption.Contains("từ chối nhận kẹo") || caption.Contains("chúc mừng năm mới") || caption.Contains("happy new year"))
                {
                    longCloseClick = mSystem.currentTimeMillis();
                    GameCanvas.menu.menuSelectedItem = i;
                    GameCanvas.gI().keyPressedz(-5);
                    break;
                }
                if (caption.Contains("Chúc mừng") || caption.Contains("merry"))
                {
                    for (int j = 0; j < GameCanvas.menu.menuItems.size(); j++)
                    {
                        if (((Command)GameCanvas.menu.menuItems.elementAt(j)).caption.Contains("OK"))
                        {
                            longCloseClick = mSystem.currentTimeMillis();
                            GameCanvas.menu.menuSelectedItem = j;
                            GameCanvas.gI().keyPressedz(-5);
                            break;
                        }
                    }
                    break;
                }
				if (type == Type.npc_move)
				{
					for (int k = 0; k < Index.Length; k++)
					{
						longCloseClick = mSystem.currentTimeMillis();
						Service.gI().confirmMenu(0, (sbyte)Index[k]);
					}
					return;
				};
                for (int k = 0; k < Index.Length; k++)
                {
					if (Index[k] == -1)
					{
						for(int j = 0; j < GameCanvas.menu.menuItems.size(); j++)
						{
							if (((Command)GameCanvas.menu.menuItems.elementAt(j)).caption.Replace('\n', ' ').Contains(info[0].ToString()))
							{
                                longCloseClick = mSystem.currentTimeMillis();
                                GameCanvas.menu.menuSelectedItem = j;
								Service.gI().confirmMenu(0, (sbyte)j);
                                GameCanvas.gI().keyPressedz(-5);
								break;
							}
                        }
					}
					else
					{
						longCloseClick = mSystem.currentTimeMillis();
                        Service.gI().confirmMenu(0, (sbyte)Index[k]);
                    }
                }
            }
        }

		public Waypoint GetWayPoint()
		{
			for (int i = 0; i < TileMap.vGo.size(); i++)
			{
				Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
				if (GetMapName().Equals(GetMapName(waypoint.popup)))
				{
					return waypoint;
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
        private static void ChangeMap(Waypoint waypoint)
		{
            if (waypoint.isOffline)
            {
                Service.gI().getMapOffline();
                return;
            }
            Service.gI().requestChangeMap();
        }
		public void Enter(Waypoint waypoint)
		{
			int num = ((waypoint.maxX < 60) ? 44 : ((waypoint.minX <= TileMap.pxw - 60) ? ((waypoint.minX + waypoint.maxX) / 2) : (TileMap.pxw - 44)));
			int maxY = waypoint.maxY;
			if (num == -1 || maxY == -1)
			{
				GameScr.info1.addInfo("Có lỗi xảy ra", 0);
				return;
			}
			if (Char.myCharz().cx != num) { TeleportTo(num, maxY); }
			waypoint.popup.doClick((int)(LongWaitNextMap/1000L)+1);
            ChangeMap(waypoint);
        }



		public void TeleportTo(int x, int y)
		{
			if (Char.myCharz().isLockMove && Char.myCharz().npcFocus == null)
			{
				return;
			}
            Char.myCharz().currentMovePoint = null;
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
	}

	public class Type
	{
		public const int next_move = 0;
		public const int npc_move = 1;
		public const int textmenu_move = 2;
		public const int moveto = 3;
		public const int useitem_move = 4;
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
		public string captions { get; set; }

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
    public class ItemAction { public int itemId { get; set; } }

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

	private static int[] idMapsNamek;

	private static int[] idMapsXayda;

	private static int[] idMapsTraiDat;

	private static int[] idMapsTuongLai;

	private static int[] idMapsCold;

	private static int[] idMapsNappa;

	private static int[] idMapNRSD;

    private static int[] idMapKhiGas;

    private static int[] idMapPotaufeu;

    private static int[] idMapmvbt_mhbt;

    private static int[] idMapDtdn;

    private static int[] idMaphttv;

    private static int[] idMapbdkb;

    private static int[] idMapOther;

    public static long longCloseClick;

	public static bool XmapNrsd;

	public static long LastNextMap;

    public static long LongWaitNextMap;

    public static long Lastselectmap;

    public static bool isteleport;

	public static bool isXmapSpecial;

    private static Dictionary<string, List<int>> mapNameLookup = new();
    public static AutoMap getInstance()
	{
		if (_Instance == null)
		{
			_Instance = new AutoMap();
		}
		return _Instance;
	}
	public AutoMap()
	{

    }
	public static void Update()
	{
		if (Char.myCharz().meDead)
		{
			lastWaitTime = mSystem.currentTimeMillis() + 1000L;
		}
		if (TileMap.mapID == IdMapEnd)
		{
			FinishXmap();
			return;
		}
		if (!isXmaping || Char.myCharz().cHP <= 0|| Char.myCharz().meDead)return;
		//bool flag = false;
		if ((TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23))
        {
			if (isEatChicken)
			{
				for (int i = 0; i < GameScr.vItemMap.size(); i++)
				{
					ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
					if ((itemMap.playerId == Char.myCharz().charID || itemMap.playerId == -1) && itemMap.template.id == 74)
					{
						//flag = true;
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
			if (isHarvestPean && GameScr.hpPotion < 10 && GameScr.gI().magicTree.currPeas > 0 && mSystem.currentTimeMillis() - lastWaitTime > 1000)
			{
				lastWaitTime = mSystem.currentTimeMillis();
				Service.gI().openMenu(4);
				Service.gI().menu(4, 0, 0);
			}
		}
		if (mSystem.currentTimeMillis() - lastWaitTime <= 1000L || mSystem.currentTimeMillis() - LastNextMap <= LongWaitNextMap) return;

		LastNextMap = mSystem.currentTimeMillis();
        if (isFutureMap(IdMapEnd))
		{
            if (GameScr.findNPCInMap(38) == null)
			{
				if (TileMap.mapID == 27)
				{
					UpdateXmap(28);
					return;
				}
				if (TileMap.mapID == 29)
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
			if(!isNRDMap(TileMap.mapID)) ShowPlanetMenu();
			else { ShowMapsMenu(planetDictionary.TryGetValue("Ngọc rồng\nsao đen", out var mapIDs) ? mapIDs : Array.Empty<int>()); }
			break;
		case 2:
			isEatChicken = !isEatChicken;
			GameScr.info1.addInfo("Ăn Đùi Gà\n" + (isEatChicken ? "[ON]" : "[OFF]"), 0);
			if (isSaveData)
			{
				Rms.saveRMSInt("AutoMapIsEatChicken", isEatChicken ? 1 : 0);
			}
			break;
		case 3:
			isHarvestPean = !isHarvestPean;
			GameScr.info1.addInfo("Thu Đậu\n" + (isHarvestPean ? "[ON]" : "[OFF]"), 0);
			if (isSaveData)
			{
				Rms.saveRMSInt("AutoMapIsHarvestPean", isHarvestPean ? 1 : 0);
			}
			break;
		case 4:
			isUseCapsule = !isUseCapsule;
			GameScr.info1.addInfo("Sử Dụng Capsule\n" + (isUseCapsule ? "[ON]" : "[OFF]"), 0);
			if (isSaveData)
			{
				Rms.saveRMSInt("AutoMapIsUseCsb", isUseCapsule ? 1 : 0);
			}
			break;
		case 5:
			//isSaveData = !isSaveData;
			//GameScr.info1.addInfo("Lưu Cài Đặt Auto Map\n" + (isSaveData ? "[ON]" : "[OFF]"), 0);
			//Rms.saveRMSInt("AutoMapIsSaveRms", isSaveData ? 1 : 0);
			//if (isSaveData)
			//{
			//	SaveData();
			//}
			break;
		case 6:
			ShowMapsMenu((int[])p);
			break;
		case 7:
			IdMapEnd = (int)p;
			if (!isNRDMap((int)p))
			{
				isXmaping = true;
				//GameScr.info1.addInfo("Go to " + TileMap.mapNames[IdMapEnd], 0);
			}
			else
			{
				MainMod.mapIdNRD = RequestMapnrsd((int)p);
                MainMod.isAutoEnterNRDMap = true;
                MainMod.isOpenMenuNPC = MainMod.isAutoEnterNRDMap;
                new Thread(new ThreadStart(MainMod.EnterNRDMap)).Start();
            }
			break;
		}
	}

	public static void ShowMenu()
	{
		LoadData();
		MyVector myVector = new MyVector();
		myVector.addElement(new Command("Load Map", getInstance(), 1, null));
		myVector.addElement(new Command(string.Format("Ăn Đùi Gà\n{0}", isEatChicken ? "[ON]" : "[ON]"), getInstance(), 2, null));
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
			if ((Char.myCharz().cgender != 0 || (mapIDs[i] != 22 && mapIDs[i] != 23)) && (Char.myCharz().cgender != 1 || (mapIDs[i] != 21 && mapIDs[i] != 23)) && (Char.myCharz().cgender != 2 || (mapIDs[i] != 21 && mapIDs[i] != 22)))
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

	private static string NamePlane(int mapID)
	{
		if (mapID >= 21 && mapID <= 23) return "Về nhà";
		return TileMap.mapNames[mapID];
    }

	private static void AddLinkMapOnly() 
	{
        if (linkMaps.ContainsKey(84))
        {
            linkMaps.Remove(84);
        }
        linkMaps.Add(84, new List<NextMap>());
        linkMaps[84].Add(new NextMap(24 + Char.myCharz().cgender, 10, new int[] { 0 }, Type.npc_move, null));
    }

    private static void UpdateXmap(int mapID)
	{
        int[] array = FindWay(mapID);
        if (array == null)
		{
            array=FindWayNull(mapID);
            if (array == null) return;
		}
        if (isUseCapsule)
		{
			if (!isUsingCapsule && array.Length >2)
			{
				for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
				{
					Item item = Char.myCharz().arrItemBag[i];
					if (item != null && (item.template.id == 194 || (item.template.id == 193 && item.quantity > 10)))
					{
						isUsingCapsule = true;
						isOpeningPanel = false;
						lastTimeOpenedPanel = mSystem.currentTimeMillis();
						if(GameCanvas.panel.mapNames != null) GameCanvas.panel.mapNames = null;
						Service.gI().useItem(0, 1, -1, item.template.id);
						return;
					}
				}
			}

            if (isUsingCapsule && !isOpeningPanel && (GameCanvas.panel.mapNames == null || mSystem.currentTimeMillis() - lastTimeOpenedPanel < 1000L)) return;
            if (isUsingCapsule && !isOpeningPanel)
			{
                for (int num = array.Length - 1; num >= 1; num--)
				{
					for (int j = 0; j < GameCanvas.panel.mapNames.Length; j++)
					{
                        if (GameCanvas.panel.mapNames[j].Replace("Về chỗ cũ: ", "").Contains(NamePlane(array[num])))
						{
							lastTimeOpenedPanel = mSystem.currentTimeMillis();
                            isUsingCapsule = false;
                            Service.gI().requestMapSelect(j);
							return;
                        }
					}
                    if (TileMap.mapID == array[num])
                    {
                        isOpeningPanel = true;
                        isUsingCapsule = false;
                    }
                }
            }
		}
        if (TileMap.mapID == array[0] && !Char.ischangingMap && !Controller.isStopReadMessage)
		{
            Goto(array[1]);
		}
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
				"Yêu cầu sức mạnh lớn hơn",
            };
			bool flag = list.Contains(text);
			if (flag)
			{
				FinishXmap();
				GameScr.info1.addInfo("Đã huỷ Xmap", 0);
			}
		}
    }
    public static void LoadMapLeft()
	{
		LoadMap(0);
	}

	public static void LoadMapCenter()
	{
		LoadMap(2);
	}

	public static void LoadMapRight()
	{
		LoadMap(1);
	}

	private static void LoadData()
	{
		isSaveData = Rms.loadRMSInt("AutoMapIsSaveRms") == 1;
		if (isSaveData)
		{
			if (Rms.loadRMSInt("AutoMapIsEatChicken") == -1)
			{
				isEatChicken = true;
			}
			else
			{
				isEatChicken = Rms.loadRMSInt("AutoMapIsEatChicken") == 1;
			}
			if (Rms.loadRMSInt("AutoMapIsUseCsb") == -1)
			{
				isUseCapsule = true;
			}
			else
			{
				isUseCapsule = Rms.loadRMSInt("AutoMapIsUseCsb") == 1;
			}
			isHarvestPean = Rms.loadRMSInt("AutoMapIsHarvestPean") == 1;
		}
	}

	private static void SaveData()
	{
		Rms.saveRMSInt("AutoMapIsEatChicken", isEatChicken ? 1 : 0);
		Rms.saveRMSInt("AutoMapIsHarvestPean", isHarvestPean ? 1 : 0);
		Rms.saveRMSInt("AutoMapIsUseCsb", isUseCapsule ? 1 : 0);
	}

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
        // 1. Tạo liên kết đi xuôi (Forward): từ mapA -> mapB dùng tọa độ fw
        if (!linkMaps.ContainsKey(mapA))
        {
            linkMaps.Add(mapA, new List<NextMap>());
        }
        // Thay new int[] { -1 } bằng tọa độ fwX, fwY
        linkMaps[mapA].Add(new NextMap(mapB, -1, new int[] { -1 }, Type.moveto, new object[] { fwX, fwY }));

        // 2. Tạo liên kết đi ngược (Backward): từ mapB -> mapA dùng tọa độ bw
        if (!linkMaps.ContainsKey(mapB))
        {
            linkMaps.Add(mapB, new List<NextMap>());
        }
        // Thay new int[] { -1 } bằng tọa độ bwX, bwY
        linkMaps[mapB].Add(new NextMap(mapA, -1, new int[] { -1 }, Type.moveto, new object[] { bwX, bwY }));
    }

    private static void AddNPCLinkMapsXmap(int currentMapID, int nextMapID, int npcID, int[] select,string captions)
	{
		if (!linkMaps.ContainsKey(currentMapID))
		{
			linkMaps.Add(currentMapID, new List<NextMap>());
		}
		linkMaps[currentMapID].Add(new NextMap(nextMapID, npcID, select,Type.npc_move, new object[] { captions }));
	}

    private static void AddLinkMapsUseItemXmap(int nextMapID, int IDitem) 
	{
        if (!linkMaps.ContainsKey(-1))
        {
            linkMaps.Add(-1, new List<NextMap>());
        }
        linkMaps[-1].Add(new NextMap(nextMapID, -1, new int[] {-1}, Type.useitem_move, new object[] { IDitem }));
    }

	private static void Goto(int mapID)
	{
        if (!linkMaps.ContainsKey(TileMap.mapID))
        {
            GameScr.info1.addInfo("Không có đường đi từ map hiện tại", 0);
            return;
        }
        foreach (NextMap item in linkMaps[TileMap.mapID])
		{
			if (item.MapID == mapID)
			{
				item.GotoMap();
				return;
			}
		}
		GameScr.info1.addInfo("Không thể thực hiện", 0);
	}

	private static int[] FindWayNull(int mapIDEnd)
	{
		if (!isUsingCapsule)
		{
			for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
			{
				Item item = Char.myCharz().arrItemBag[i];
				if (item != null && (item.template.id == 194 || item.template.id == 193))
				{
					isUsingCapsule = true;
					isOpeningPanel = false;
					//lastTimeOpenedPanel = mSystem.currentTimeMillis();
					//GameCanvas.panel.mapNames = null;
					Service.gI().useItem(0, 1, -1, item.template.id);
					//if (Char.myCharz().statusMe == 14)
					//{
					//	isUsingCapsule = false;
					//	isOpeningPanel = true;
					//}
					break;
				}
			}
		}
		else if (!isOpeningPanel)
		{
			if (GameCanvas.panel.mapNames == null || mapNameLookup == null ) return null;
            // Chuyển danh sách tên từ Capsule sang ID thực tế
            List<int> startIDs = new List<int>();
            for (int j = 0; j < GameCanvas.panel.mapNames.Length; j++)
			{
                if (string.IsNullOrEmpty(GameCanvas.panel.mapNames[j])) continue;
                string cleanName = GameCanvas.panel.mapNames[j].Replace("Về chỗ cũ: ", "");
                if (mapNameLookup.TryGetValue(cleanName, out var ids))
				{
                    startIDs.AddRange(ids);
                }
			}
            // 3. Tìm đường từ toàn bộ danh sách ID này
            int[] finalPath = FindWay(mapIDEnd, startIDs.ToArray());
            for (int k = 0; k < GameCanvas.panel.mapNames.Length; k++)
            {
                if (string.IsNullOrEmpty(GameCanvas.panel.mapNames[k])) continue;
                string cleanName = GameCanvas.panel.mapNames[k].Replace("Về chỗ cũ: ", "");
                if (finalPath != null)
				{
					if(finalPath.Length > 0 && cleanName.Contains(NamePlane(finalPath[0])))
					{
                        isOpeningPanel = true;
                        isUsingCapsule = false;
                        Service.gI().requestMapSelect(k);
						return finalPath;
                    }
                }
				else
				{
                    if (cleanName.Contains(NamePlane(Char.myCharz().cgender+21)))
                    {
                        isOpeningPanel = true;
                        isUsingCapsule = false;
                        Service.gI().requestMapSelect(k);
                        return finalPath;
                    }
                }

            }
		}
        GameScr.info1.addInfo("Không thể tìm thấy đường đi", 0);
        return null;
	}
	private static int[] FindWay(int mapID)
	{
		return FindWay(mapID, new int[] { TileMap.mapID });
	}
    private static int[] FindWay(int mapIDEnd, int[] mapIDStarts)
    {
        if (mapIDStarts == null || mapIDStarts.Length == 0) return null;

        // Hàng đợi lưu trữ các con đường (List<int>)
        Queue<List<int>> queue = new Queue<List<int>>();

        // Tập hợp các map đã đi qua để tránh vòng lặp vô tận
        HashSet<int> visited = new HashSet<int>();
        // BƯỚC 1: Khởi tạo - Đưa tất cả điểm xuất phát vào Queue
        foreach (int startID in mapIDStarts)
        {
            if (startID == mapIDEnd) return new int[] { startID };

            queue.Enqueue(new List<int> { startID });
            visited.Add(startID);
        }
        while (queue.Count > 0)
        {
            List<int> currentPath = queue.Dequeue();
            int lastMap = currentPath[currentPath.Count - 1];

            // Duyệt các map kế cận
            if (!linkMaps.ContainsKey(lastMap)) continue;

            foreach (NextMap next in linkMaps[lastMap])
            {
                if (visited.Contains(next.MapID)) continue;

                List<int> newPath = new List<int>(currentPath) { next.MapID };

                // Kiểm tra các điều kiện đặc thù của game nro
                if (hasWayGoFutureAndBack(newPath.ToArray()) ||
                   (Char.myCharz().taskMaint.taskId <= 30 && hasWayGoToColdMap(newPath.ToArray())))
                    continue;

                if (next.MapID == mapIDEnd)
                    return newPath.ToArray();

                visited.Add(next.MapID);
                queue.Enqueue(newPath);
            }
        }
        return null;
    }

    private static bool hasWayGoFutureAndBack(int[] ways)
	{
		for (int i = 1; i < ways.Length - 1; i++)
		{
			if (ways[i] == 102 && ways[i + 1] == 24 && (ways[i - 1] == 27 || ways[i - 1] == 28 || ways[i - 1] == 29))
			{
				return true;
			}
		}
		return false;
	}

	private static bool hasWayGoToColdMap(int[] ways)
	{
		for (int i = 0; i < ways.Length; i++)
		{
			if (ways[i] >= 105 && ways[i] <= 110)
			{
				return true;
			}
		}
		return false;
	}

	private static string GetMapName(int mapID)
	{
		if (1 == 0)
		{
		}
		string result = mapID switch
		{
			113 => string.Concat(new object[3] { "Siêu hạng\n[", mapID, "]" }), 
			129 => TileMap.mapNames[mapID] + " 23\n[" + mapID + "]", 
			_ => TileMap.mapNames[mapID] + "\n[" + mapID + "]", 
		};
		if (1 == 0)
		{
		}
		return result;
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
				bool specical = TileMap.mapID==155;

				if (waypoint.maxX < 60)
				{
                    wayPointMapLeft[0] = waypoint.minX + 15;
					wayPointMapLeft[1] = waypoint.maxY;
				}
				else if (waypoint.maxX > TileMap.pxw - 60)
				{
                    wayPointMapRight[0] = waypoint.maxX - 15;
					wayPointMapRight[1] = waypoint.maxY-(specical?24:0);
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

	private static bool isNRDMap(int MapID)
	{
		if (MapID >= 85)
		{
			return MapID <= 91;
		}
		return false;
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
		if (mapID.template.id >= 372)
		{
			return mapID.template.id <= 378;
		}
		return false;
	}

	private static void LoadMap(int position)
	{
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
			{
				TeleportTo(wayPointMapLeft[0], wayPointMapLeft[1]);
			}
			else
			{
				TeleportTo(60, GetYGround(60));
			}
			break;
		case 1:
			if (wayPointMapRight[0] != 0 && wayPointMapRight[1] != 0)
			{
				TeleportTo(wayPointMapRight[0], wayPointMapRight[1]);
			}
			else
			{
				TeleportTo(TileMap.pxw - 60, GetYGround(TileMap.pxw - 60));
			}
			break;
		case 2:
			if (wayPointMapCenter[0] != 0 && wayPointMapCenter[1] != 0)
			{
				TeleportTo(wayPointMapCenter[0], wayPointMapCenter[1]);
			}
			else
			{
				TeleportTo(TileMap.pxw / 2, GetYGround(TileMap.pxw / 2));
			}
			break;
		}
		if (TileMap.mapID != 7 && TileMap.mapID != 14 && TileMap.mapID != 0)
		{
			Service.gI().requestChangeMap();
			return;
        }
		Service.gI().getMapOffline();
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
			default: break;
		}
	}

	static AutoMap()
	{
        LongWaitNextMap = 2000L;
        linkMaps = new Dictionary<int, List<NextMap>>();
		planetDictionary = new Dictionary<string, int[]>();
		isEatChicken = true;
		isUseCapsule = true;
		idMapsTuongLai = new int[10] { 102, 92, 93, 94, 96, 97, 98, 99, 100, 103 };
		InitializeXmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "map_data.json"));
        LoadData();
		AddLinkMapOnly();
    }

    public static sbyte FindCT(int id)
	{
		for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
		{
			if (Char.myCharz().arrItemBag[i] != null && Char.myCharz().arrItemBag[i].template.id ==id)
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
        if (FindCT(592+Char.myCharz().cgender) == -1 && (!arrItemBody[5].template.name.Contains("Yardrat") || arrItemBody[5] == null))
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
            case 85:
                return 1;
            case 86:
                return 2;
            case 87:
                return 3;
            case 88:
                return 4;
            case 89:
                return 5;
            case 90:
                return 6;
            case 91:
                return 7;
            default:
                MainMod.isAutoEnterNRDMap = false;
                MainMod.isOpenMenuNPC = MainMod.isAutoEnterNRDMap;
                return -1;
        }
    }
    public static void InitMapLookup(string name, int id)
    {
        mapNameLookup.Clear();
        if (!mapNameLookup.ContainsKey(name))
		{
            mapNameLookup[name] = new List<int>();
            mapNameLookup[name].Add(id);
        }
    }
	private static void InitializeXmap(string jsonPath)
	{
		try
		{
			string jsonContent = File.ReadAllText(jsonPath);
			MapData data = JsonSerializer.Deserialize(jsonContent, AppConfigJsonContext.Default.MapData);
			if (data == null) return;

			//0.
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
			// 1. Xử lý link_maps (Các map thông nhau trực tiếp)
			if (data.link_maps != null)
			{
				foreach (var link in data.link_maps)
				{
					AddLinkMapsXmap(link);
				}
			}

			// 2. Xử lý npc_maps (Chuyển map qua NPC)
			if (data.npc_maps != null)
			{
				foreach (var nMap in data.npc_maps)
				{
					int mapStart = nMap.map[0];
					int mapEnd = nMap.map[1];

					if (nMap.fw != null) AddNPCLinkMapsXmap(mapStart, mapEnd, nMap.fw.npcId, nMap.fw.selectIndex, nMap.fw.captions);

					if (nMap.bw != null) AddNPCLinkMapsXmap(mapEnd, mapStart, nMap.bw.npcId, nMap.bw.selectIndex, nMap.fw.captions);
				}
			}
			// 3. Xử lý useitem_maps (Sử dụng vật phẩm để bay/chuyển map)
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

			//4. Xử lý thêm item menu
			if (data.menu_maps != null)
			{
				foreach (var MenuMap in data.menu_maps)
				{
					planetDictionary.Add(MenuMap.menu, MenuMap.IDmap);
				}
			}
		}
		catch (Exception ex)
		{
			//Console.WriteLine("Error loading map data: " + ex.Message);
		}
	}
}
