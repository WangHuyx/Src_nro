
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using static NRO.AutoItem;
using static System.Reflection.Metadata.BlobBuilder;

namespace NRO;

public class AutoTrain : IActionListener, IChatable
{
	private static readonly int[][] Directions = new int[][]
	{
		new int[]{1,0}, new int[]{-1,0}, new int[]{0,1}, new int[]{0,-1}
	};

	public static PathPoint[] FindPath(int instartX, int instartY, int intargetX, int intargetY)
	{
		try
		{
			const int TILE_SIZE = 24;
			int startX = instartX / TILE_SIZE;
			int startY = (instartY) / TILE_SIZE;
			int targetX = intargetX / TILE_SIZE;
			int targetY = (intargetY) / TILE_SIZE;
			List<Node> openList = new List<Node>();
			Dictionary<Coord, int> costMap = new Dictionary<Coord, int>();

			int heuristic = Math.abs(targetX - startX) + Math.abs(targetY - startY);
			Node startNode = new Node(startX, startY, 0, heuristic, null);
			openList.Add(startNode);
			costMap[new Coord(startX, startY)] = 0;

			Node targetNode = null;
			int steps = 0;

			while (openList.Count > 0 && steps < 1000)
			{
				steps++;
				// Lấy node F thấp nhất
				Node current = openList[0];

				foreach (Node n in openList)
					if (n.F < current.F) current = n;
				openList.Remove(current);

				if (current.X == targetX && current.Y == targetY)
				{
					targetNode = current;
					break;
				}

				foreach (var dir in Directions)
				{
					int nx = current.X + dir[0];
					int ny = current.Y + dir[1];

					if (isTileTypeMap(nx, ny))
					{
						continue;
					}
					if (nx < 0 || ny < 0)
					{
						continue;
					}
					int newG = current.G + 1;

					int oldG;
					if (!costMap.TryGetValue(new Coord(nx, ny), out oldG) || newG < oldG)
					{
						costMap[new Coord(nx, ny)] = newG;
						int h = Math.abs(targetX - nx) + Math.abs(targetY - ny);
						openList.Add(new Node(nx, ny, newG, h, current));
					}
				}
			}

			if (targetNode == null)
			{
				//Console.WriteLine("no path found");
				return new PathPoint[0];
			}
			Stack<PathPoint> path = new Stack<PathPoint>();
			for (Node n = targetNode; n != null; n = n.Parent)
			{
				int px = n.X * TILE_SIZE;
				int py = n.Y * TILE_SIZE;
				path.Push(new PathPoint(px, py));
			}
			return path.ToArray();
		}
		catch (Exception ex)
		{
			//Console.WriteLine("error pathpoint:" + ex.Message);
			return new PathPoint[0];
		}
	}

	public class Node
	{
		public int X, Y;        // tọa độ
		public int G;           // chi phí từ start
		public int H;           // heuristic (Manhattan)
		public Node Parent;     // parent node để reconstruct path

		public int F { get { return G + H; } }

		public Node(int x, int y, int g, int h, Node parent = null)
		{
			X = x;
			Y = y;
			G = g;
			H = h;
			Parent = parent;
		}
	}
	public struct Coord
	{
		public int X, Y;
		public Coord(int x, int y) { X = x; Y = y; }

		// Cần override Equals và GetHashCode để dùng làm key trong Dictionary
		public override bool Equals(object obj)
		{
			if (!(obj is Coord)) return false;
			Coord other = (Coord)obj;
			return X == other.X && Y == other.Y;
		}
		public override int GetHashCode()
		{
			// Hash đơn giản
			return X * 31 + Y;
		}
	}
	public struct PathPoint
	{
		public int X, Y;
		public PathPoint(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
	static PathPoint[] Way = null;
	static int currentStep = 0;
	static int lastMobX = -1, lastMobY = -1;
    static long TrainLoad;

    private static AutoTrain _Instance;

	public static bool isAvoidSuperMob;

    public static bool isAvoidUnCommonMob;

    public static bool isGoBack;

	public static bool isGobackCoordinate;

	public static int gobackX;

	public static int gobackY;

	public static int gobackMapID;

	private static int gobackZoneID;

	public static bool isAutoTrain;

    public static bool chongmkn;

    private static int minimumMPGoHome;

    private static int minimumHPGoHome;

    private static string[] inputMPPercentGoHome;

	public static List<int> listMobIds;

	private static List<string> ListSkillwithcgender;

	private static long lastTimeTeleportToMob;

	public static long Longakok;

	public static bool IsKok;

	public static long long_101;

	public static bool StartSaveGold;

	public static long GoldHT;

	public static bool isStartUpmvbt;

	public static int SaveZoneUp;

	public static long SleepMenu;

	public static long LoadMapKey;

	public static bool StopMoveAutoTrain;

	public static long ThreadUseptr;

	public static long DelayMove;

    //lấy từ XuLiSettings
    public static long lastTimeGoback;
    public static int SaveSecondGoback;
    public static long TimeDelayChangeZone;
	private static int gobackIndexZone;
	public static long dichdaumap;

    public static AutoTrain getInstance()
	{
		if (_Instance == null)
		{
			_Instance = new AutoTrain();
		}
		return _Instance;
	}

    public static void Move(int cx, int cy)
    {
        global::Char.myCharz().cx = cx;
        global::Char.myCharz().cy = cy;
		if ((cx - Char.myCharz().cxSend != 0 || cy - Char.myCharz().cySend != 0) && (Math.abs(cx - Char.myCharz().cxSend) >= 72 || Math.abs(cy - Char.myCharz().cySend) >= 48))
		{
			Service.gI().charMove();
		}
    }

    public static void Update()
	{
		if (IsKok||chongmkn)
		{
			kok();
		}
		if (isAutoTrain)
		{
			if (!Char.myCharz().meDead)
			{
				if (Controller.isStopReadMessage || Char.myCharz().isTeleport || Char.myCharz().isPaintNewSkill || InfoDlg.isLock) return;
				if ((!isAutoTrain && !GameScr.canAutoPlay) || GameScr.isChangeZone || GameScr.gI().isCharging() || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5 || Char.myCharz().isCharge || Char.myCharz().isFlyAndCharge || Char.myCharz().isUseChargeSkill())
				{
					return;
				}
                DoIt();

				if (MainMod.CurrentSetting.IsTrainMobHP == AccountSetting.TypeHPMob.Less)
				{
                    if (mSystem.currentTimeMillis() - long_101 >= 600L)
                    {
                        bool islock = (Char.myCharz().isLockMove || Char.myCharz().isWaitMonkey || Char.myCharz().isStandAndCharge || Char.myCharz().holder);
                        if (!islock)
                        {
                            long_101 = mSystem.currentTimeMillis();
                            if (Char.myCharz().cx != 90)
                            {
                                Char.myCharz().cx = 90;
                                Char.myCharz().cy = MainMod.GetYGround(90);
                                Service.gI().charMove();
                            }
                            else
                            {
                                Char.myCharz().cx = 80;
                                Char.myCharz().cy = MainMod.GetYGround(90);
                                Service.gI().charMove();
                            }
                        }
                    }
                }
				else
				{
					if (Char.myCharz().currentMovePoint != null) Char.myCharz().currentMovePoint = null;
					if (Char.myCharz().mobFocus != null)
					{
						if (!ItemTime.isExistItem(4387)) ChayBo(Char.myCharz().mobFocus.xFirst, Char.myCharz().mobFocus.yFirst);
						//else
						//{
						//	if (mSystem.currentTimeMillis() - lastTimeTeleportToMob > 1000L)
						//	{
						//		lastTimeTeleportToMob = mSystem.currentTimeMillis();
						//		if (Char.myCharz().mobFocus.status != 1 && (Math.abs(Char.myCharz().mobFocus.x - Char.myCharz().cx) > 100 || Math.abs(Char.myCharz().mobFocus.y - Char.myCharz().cy) > 100)) TeleportTo(Char.myCharz().mobFocus.xFirst, Char.myCharz().mobFocus.yFirst);
						//	}
						//}
					}
				}
			}
        }
		if (Char.myCharz().cStamina <= 5 && GameCanvas.gameTick % 100 == 0)
		{
			UseGrape();
		}
		if (isGoBack)
		{
			if (lastTimeGoback == 0L) lastTimeGoback = mSystem.currentTimeMillis();
			if (mSystem.currentTimeMillis() - lastTimeGoback >= SaveSecondGoback * 1000L)
			{
				if (Char.myCharz().meDead)
				{
					lastTimeGoback = 0L;
					Service.gI().returnTownFromDead();
					return;
                }
				if (MainMod.CurrentSetting.IsGobackCondition && isMeOutOfMPorHP())
				{
					lastTimeGoback = 0L;
					int num = 21 + Char.myCharz().cgender;
					if (TileMap.mapID != num)
					{
						GameScr.isAutoPlay = false;
						Char.myCharz().mobFocus = null;
						AutoMap.StartRunToMapId(num);
					}
                    if ((TileMap.mapID == num))
                    {
                        for (int i = 0; i < GameScr.vItemMap.size(); i++)
                        {
                            ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
                            if ((itemMap.playerId == Char.myCharz().charID || itemMap.playerId == -1) && itemMap.template.id == 74)
                            {
                                //flag = true;
                                Char.myCharz().itemFocus = itemMap;
                                Service.gI().pickItem(Char.myCharz().itemFocus.itemMapID);
                                break;
                            }
                        }
                    }
                    return;
				}
			}
		}
		if (TileMap.mapID != gobackMapID)
		{
			GameScr.isAutoPlay = false;
			isAutoTrain = false;
            //if (Char.myCharz().currentMovePoint != null) Char.myCharz().currentMovePoint = null;
            AutoMap.StartRunToMapId(gobackMapID);
		}
		else
		{
			
			if (!Char.ischangingMap && !Controller.isStopReadMessage && mSystem.currentTimeMillis()- TimeDelayChangeZone>=1000L)
			{
                TimeDelayChangeZone = mSystem.currentTimeMillis();
                var setting = MainMod.CurrentSetting;

                // 1. Kiểm tra điều kiện danh sách khu
                if (setting.ListZoneChange == null || setting.ListZoneChange.Length <= 0) return;

                // 2. Thiết lập mục tiêu mặc định là khu đầu tiên
                int targetZoneID = setting.ListZoneChange[0];
                bool needToSwitch = false;

                // 3. Xử lý logic khi bật né người hoặc hết quái
                if (setting.IsChangeZoneChar || setting.IsChangeZoneMob)
                {
                    // Kiểm tra né người
                    if (setting.IsChangeZoneChar)
                    {
                        for (int i = 0; i < GameScr.vCharInMap.size(); i++)
                        {
                            Char @char = (Char)GameScr.vCharInMap.elementAt(i);
                            if (@char != null && isChar(@char) && setting.ListCharDorge.Contains(@char.GetNameWithoutClanTag()))
                            {
                                needToSwitch = true;
                                break;
                            }
                        }
                    }

                    // Kiểm tra hết quái (nếu chưa bị buộc đổi khu do né người)
                    if (!needToSwitch && setting.IsChangeZoneMob)
                    {
                        Mob mob = GetNextMob();
                        if (mob == null) needToSwitch = true; // Không tìm thấy quái phù hợp để đánh
                    }

                    if (needToSwitch)
                    {
                        // Logic đổi sang khu tiếp theo trong danh sách
                        if(TileMap.zoneID == setting.ListZoneChange[gobackIndexZone]) gobackIndexZone++;
                        if (gobackIndexZone >= setting.ListZoneChange.Length) gobackIndexZone = 0;

                        targetZoneID = setting.ListZoneChange[gobackIndexZone];

                        // Đảm bảo khu mới khác khu đang đứng (nếu danh sách có từ 2 khu trở lên)
                        if (targetZoneID == TileMap.zoneID && setting.ListZoneChange.Length > 1)
                        {
                            gobackIndexZone++;
                            if (gobackIndexZone >= setting.ListZoneChange.Length) gobackIndexZone = 0;
                            targetZoneID = setting.ListZoneChange[gobackIndexZone];
                        }
                    }
                    else
                    {
                        // Có bật tính năng nhưng không gặp điều kiện phải đổi -> quay về khu đầu tiên
                        bool isStandingInValidZone = false;
                        foreach (int z in setting.ListZoneChange)
                        {
                            if (z == TileMap.zoneID) { isStandingInValidZone = true; break; }
                        }
						if (!isStandingInValidZone)
						{
							// Nếu đang đứng ở khu lạ (vừa đổi map/hồi sinh) -> Về khu ưu tiên số 0
							gobackIndexZone = 0;
							targetZoneID = setting.ListZoneChange[0];
						}
						else return;
                    }
                }
                else
                {
                    // Không bật cả 2 tính năng -> luôn về khu đầu tiên
                    gobackIndexZone = 0;
                    targetZoneID = setting.ListZoneChange[0];
                }

                // 4. Thực hiện lệnh đổi khu nếu khác khu hiện tại
                gobackZoneID = targetZoneID;
                if (TileMap.zoneID != gobackZoneID)
                {
                    Service.gI().requestChangeZone(gobackZoneID, -1);
					return;
                }
            }
            if (!isGobackCoordinate)
            {
                AutoMap.isXmaping = false;
                if (!Char.ischangingMap)
                {
                    if (Char.myCharz().currentMovePoint != null) Char.myCharz().currentMovePoint = null;
                    GameScr.isAutoPlay = true;
                    isAutoTrain = true;
                }
            }
            else
            {
                if ((Char.myCharz().cx != gobackX || Char.myCharz().cy != gobackY) && GameCanvas.gameTick % 100 == 0)
                {
                    TeleportTo(gobackX, gobackY);
                }
            }
        }
	}

    public static bool isChar(Char @char)
    {
        return @char.cName != null && @char.cName != "" && !@char.isPet && !@char.isMiniPet && !@char.cName.StartsWith("#") && !@char.cName.StartsWith("$") && @char.cName != "Trọng tài";
    }
    public void onChatFromMe(string text, string to)
	{
		if (ChatTextField.gI().tfChat.getText() != null && !ChatTextField.gI().tfChat.getText().Equals(string.Empty) && !text.Equals(string.Empty) && text != null)
		{
			if (ChatTextField.gI().strChat.Equals(inputMPPercentGoHome[0]))
			{
				try
				{
					int num = (minimumMPGoHome = int.Parse(ChatTextField.gI().tfChat.getText()));
					GameScr.info1.addInfo("Về Nhà Khi MP Dưới\n[" + num + "%]", 0);
				}
				catch
				{
					GameScr.info1.addInfo("%MP Không Hợp Lệ, Vui Lòng Nhập Lại", 0);
				}
				ResetChatTextField();
			}
		}
		else
		{
			ChatTextField.gI().isShow = false;
		}
	}

	public void onCancelChat()
	{
	}

	public void perform(int idAction, object p)
	{
		switch (idAction)
		{
		case 1:
		{
			int num = (int)p;
			listMobIds.Clear();
			for (int oi = 0; oi < GameScr.vMob.size(); oi++)
			{
				Mob mob = (Mob)GameScr.vMob.elementAt(oi);
				if (!mob.isMobMe && mob.templateId == num)
				{
					listMobIds.Add(mob.mobId);
				}
			}
			TurnOnAutoTrain();
			break;
		}
		case 2:
		{
			listMobIds.Clear();
			for (int j = 0; j < GameScr.vMob.size(); j++)
			{
				Mob mob2 = (Mob)GameScr.vMob.elementAt(j);
				if (!mob2.isMobMe)
				{
					listMobIds.Add(mob2.mobId);
				}
			}
			TurnOnAutoTrain();
			break;
		}
		case 3:
			TurnOnAutoTrain();
			break;
		case 4:
			isAvoidSuperMob = !isAvoidSuperMob;
			GameScr.info1.addInfo("Né Siêu Quái\n" + (isAvoidSuperMob ? "[OFF]" : "[ON]"), 0);
			break;
		case 5:
			ShowMenuGoback();
			break;
		case 6:
			listMobIds.Clear();
			isAutoTrain = false;
			GameScr.info1.addInfo("Đã Clear Danh Sách Train!", 0);
			break;
		case 7:
			if (Char.myCharz().mobFocus == null)
			{
				GameScr.info1.addInfo("Vui Lòng Chọn Quái!", 0);
			}
			if (Char.myCharz().mobFocus != null)
			{
				listMobIds.Add(Char.myCharz().mobFocus.mobId);
				GameScr.info1.addInfo("Đã Thêm Quái: " + Char.myCharz().mobFocus.mobId, 0);
			}
			break;
		case 8:
			isAutoTrain = false;
			Char.myCharz().mobFocus = null;
			GameScr.info1.addInfo("Đã Tắt Auto Train!", 0);
			break;
		case 9:
			if (isGoBack)
			{
				isGoBack = false;
				GameScr.info1.addInfo("Goback\n[OFF]", 0);
			}
			else if (!isGoBack)
			{
				isGobackCoordinate = false;
				isGoBack = true;
				gobackMapID = TileMap.mapID;
				gobackZoneID = TileMap.zoneID;
				GameScr.info1.addInfo("Goback\n[" + TileMap.mapNames[gobackMapID] + "]\n[" + gobackZoneID + "]", 0);
			}
			break;
		case 10:
			if (isGoBack)
			{
				isGoBack = false;
				GameScr.info1.addInfo("Goback\n[OFF]", 0);
			}
			else if (!isGoBack)
			{
				isGobackCoordinate = true;
				isGoBack = true;
				gobackMapID = TileMap.mapID;
				gobackZoneID = TileMap.zoneID;
				gobackX = Char.myCharz().cx;
				gobackY = Char.myCharz().cy;
				GameScr.info1.addInfo("Goback Tọa Độ\n[" + gobackX + "-" + gobackY + "]", 0);
			}
			break;
		case 11:
			ChatTextField.gI().strChat = inputMPPercentGoHome[0];
			ChatTextField.gI().tfChat.name = inputMPPercentGoHome[1];
			ChatTextField.gI().startChat2(getInstance(), string.Empty);
			break;
		case 12:
			break;
		case 13:
            break;
		case 14:
            break;
		case 15:
			ShowMenuListMob();
            break;
        case 16:
            ShowMenuListSkill();
            break;
        case 17:
			int i = (int)p;
			if(GameScr.keySkill[i] != null)
			{
				if(ListSkillwithcgender.Contains(GameScr.keySkill[i].template.id.ToString()))
                {
                    ListSkillwithcgender.Remove(GameScr.keySkill[i].template.id.ToString());
                }
                else
                {
                    ListSkillwithcgender.Add(GameScr.keySkill[i].template.id.ToString());
                }
                Rms.saveRMSString("ListSkill_" + Char.myCharz().cgender, string.Join(",", ListSkillwithcgender.ToArray()));
            }
			ShowMenuListSkill();
			break;
        }
	}

    private static void LoadData()
    {
        string data = Rms.loadRMSString("ListSkill_" + Char.myCharz().cgender);
        ListSkillwithcgender = string.IsNullOrEmpty(data) ? new List<string>{ Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[0]).template.id.ToString() } : data.Split(',').ToList();
    }

    public static void ShowMenu()
	{
		MyVector myVector = new MyVector();
		if (isAutoTrain/* && !GameScr.canAutoPlay*/)
		{
			myVector.addElement(new Command("Tắt Auto Train", getInstance(), 8, null));
		}
        myVector.addElement(new Command("D/S Quái\nTàn Sát", getInstance(), 15, null));
        myVector.addElement(new Command("Tàn Sát Tất Cả", getInstance(), 2, null));
		myVector.addElement(new Command("Tàn Sát Theo Vị Trí", getInstance(), 3, null));
		myVector.addElement(new Command("Né Siêu Quái\n" + (isAvoidSuperMob ? "[OFF]" : "[ON]"), getInstance(), 4, null));
        myVector.addElement(new Command("D/S Skill\nTàn Sát", getInstance(), 16, null));
        myVector.addElement(new Command("Goback", getInstance(), 5, null));
		if(listMobIds.Count != 0) myVector.addElement(new Command("Clear Danh Sách Train", getInstance(), 6, null));
		if (Char.myCharz().mobFocus != null)
		{
			myVector.addElement(new Command("Thêm\n" + Char.myCharz().mobFocus.getTemplate().name + "\n[" + Char.myCharz().mobFocus.mobId + "]", getInstance(), 7, null));
		}
		//myVector.addElement(new Command("Auto Up mvbt\n" + (isStartUpmvbt ? "[ON]" : "[OFF]"), getInstance(), 12, null));
		GameCanvas.menu.startAt(myVector, 3);
	}

	private static void ShowMenuListSkill()
	{
		LoadData();
        MyVector myVector = new MyVector();
        for (int i = 0; i < GameScr.keySkill.Length; i++)
        {
			myVector.addElement(new Command(((GameScr.keySkill[i] != null) ? GameScr.keySkill[i].template.name + "\n" + (ListSkillwithcgender.Contains(GameScr.keySkill[i].template.id.ToString()) ? "[ON]" : "[OFF]") : "null") , getInstance(), 17, i));
        }
        GameCanvas.menu.startAt(myVector, 3);
    }

    private static void ShowMenuListMob()
	{
        MyVector myVector = new MyVector();
        List<Mob> list = new List<Mob>();
        for (int i = 0; i < GameScr.vMob.size(); i++)
        {
            Mob mob = (Mob)GameScr.vMob.elementAt(i);
            if (mob.isMobMe)
            {
                continue;
            }
            bool flag = false;
            for (int j = 0; j < list.Count; j++)
            {
                if (mob.templateId == list[j].templateId)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                list.Add(mob);
                myVector.addElement(new Command("Tàn Sát\n" + mob.getTemplate().name + "\n[" + NinjaUtil.getMoneys(mob.maxHp) + "HP]", getInstance(), 1, mob.templateId));
            }
        }
        GameCanvas.menu.startAt(myVector, 3);
    }
	private static void ShowMenuGoback()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command("Goback\n" + (isGoBack ? ("[" + TileMap.mapNames[gobackMapID] + "]\n[" + gobackZoneID + "]") : "[OFF]"), getInstance(), 9, null));
		myVector.addElement(new Command("Goback Tọa Độ\n" + ((!isGoBack || !isGobackCoordinate) ? "[OFF]" : ("[" + gobackX + "-" + gobackY + "]")), getInstance(), 10, null));
		myVector.addElement(new Command("Về Nhà Khi MP Dưới\n[" + minimumMPGoHome + "%]", getInstance(), 11, null));
		GameCanvas.menu.startAt(myVector, 3);
	}

	//private static void MenuSkillTrain()
	//{
	//	MyVector myVector = new MyVector();
	//	for (int i = 0; i < GameScr.keySkill.Length; i++)
	//	{
	//		int Skillid = -1;
	//		for (int j = 0; j < ListSkillwithcgender.Length; j++)
	//		{
	//			Skillid = ListSkillwithcgender[j];
	//			GameScr.info1.addInfo(Skillid.ToString(), 0);
	//		}
	//		myVector.addElement(new Command(((GameScr.keySkill[i] != null) ? GameScr.keySkill[i].template.name + "\n" + ((Skillid == GameScr.keySkill[i].template.id) ? "[CÓ]" : "[KHÔNG]") : "null"), getInstance(), 14, i));
	//	}
	//	GameCanvas.menu.startAt(myVector, 3);
	//}

	private static void ResetChatTextField()
	{
		ChatTextField.gI().strChat = "Chat";
		ChatTextField.gI().tfChat.name = "chat";
		ChatTextField.gI().isShow = false;
	}

    public static int[] RemoveValue(int[] arr, int value)
    {
        return arr.Where(x => x != value).ToArray();
    }

    private static void TeleportTo(int x, int y)
	{
        if (Char.myCharz().isLockMove && Char.myCharz().npcFocus == null)
        {
            GameScr.info1.addInfo("Đang không thể di chuyển!", 0);
            return;
        }
        Char.myCharz().cx = x;
		Char.myCharz().cy = y;
		Service.gI().charMove();
		if (ItemTime.isExistItem(4387)) return;
        Char.myCharz().cx = x;
		Char.myCharz().cy = y + 1;
		Service.gI().charMove();
		Char.myCharz().cx = x;
		Char.myCharz().cy = y;
		Service.gI().charMove();
	}

	private static bool isMeAvoidAttack(Mob mob)
	{
		//if (!GameScr.canAutoPlay)
		//{
		if (mob.checkIsBoss()) return isAvoidSuperMob;			
		else return isAvoidUnCommonMob;
		//}
	}

	private static bool isMeOutOfMPorHP()
	{
		return (Char.myCharz().cMP < Char.myCharz().cMPFull * minimumMPGoHome / 100)||(Char.myCharz().cHP < Char.myCharz().cHPFull * minimumHPGoHome / 100);
	}
    private static bool IsValidMob(Mob mob)
    {
        var type = MainMod.CurrentSetting.IsTrainMobHP;
        long limitHP = MainMod.CurrentSetting.TrainMobHP;

        return type switch
        {
            AccountSetting.TypeHPMob.All => true,
            AccountSetting.TypeHPMob.Less => mob.hp <= limitHP,
            AccountSetting.TypeHPMob.More => mob.hp >= limitHP,
            _ => false,
        };
    }
    private static int TypeTrainMob()
    {
        var type = MainMod.CurrentSetting.TrainMobType;

        return type switch
        {
            AccountSetting.TypeMob.All => 0,
            AccountSetting.TypeMob.Index => 1,
            AccountSetting.TypeMob.Template_id => 2,
            _ => 0,
        };
    }
	private static bool GetMobTemplateId(Mob mob)
	{
		int LengtList = MainMod.CurrentSetting.ListMob.Length;
        for (int i = 0; i < LengtList; i++)
		{
			int.TryParse(MainMod.CurrentSetting.ListMob[i].ToString(), out int templateid);
			if (mob.templateId == templateid)
			{
				return true;
			}
		}
		return false;
    }
	private static Mob GetNextMobFuture()
	{
		long num = mSystem.currentTimeMillis();
        int type = TypeTrainMob();
        if (type == 0)
        {
			Mob result = null;
            int vMobSize = GameScr.vMob.size();
            for (int i = 0; i < vMobSize; i++)
            {
                Mob mob = (Mob)GameScr.vMob.elementAt(i);
                if (mob != null)
                {
                    if (!mob.isMobMe && mob.cTimeDie < num && IsValidMob(mob) && !isMeAvoidAttack(mob))
                    {
                        result = mob;
                        num = mob.cTimeDie;
                        break;
                    }
                }
            }
            return result;
        }
        else if (type == 1)
        {
            Mob result1 = null;
            int vMobSize1 = GameScr.vMob.size();
            int Listmob = MainMod.CurrentSetting.ListMob.Length;
            for (int i = 0; i < Listmob; i++)
            {
                int mobIndex = MainMod.CurrentSetting.ListMob[i];
                if (mobIndex < 0 || mobIndex >= vMobSize1) continue;
                Mob mob1 = (Mob)GameScr.vMob.elementAt(mobIndex);
                if (mob1 != null)
                {
                    if (mob1.status != 0 && mob1.status != 1 && !mob1.isMobMe && IsValidMob(mob1) && !isMeAvoidAttack(mob1))
                    {
                        result1 = mob1;
                        break;
                    }
                }
            }
            return result1;
        }
        Mob result2 = null;
        //int num2 = 9999;
        int vMobSize2 = GameScr.vMob.size();
        for (int j = 0; j < vMobSize2; j++)
        {
            Mob mob2 = (Mob)GameScr.vMob.elementAt(j);
            if (mob2 != null)
            {
                if (mob2.status != 0 && mob2.status != 1 && mob2.hp > 0 && !mob2.isMobMe && IsValidMob(mob2) && GetMobTemplateId(mob2) && !isMeAvoidAttack(mob2))
                {
                    //int num3 = Math.abs(Char.myCharz().cx - mob2.x);
                    //if (num2 > num3 && IsValidMob(mob2))
                    //{
                    result2 = mob2;
                    break;
                    //	num2 = num3;
                    //}
                }
            }
        }
        return result2;
    }
	private static Mob GetNextMob()
	{
		int type = TypeTrainMob();
		if (type == 0)
		{
			Mob result = null;
            int vMobSize = GameScr.vMob.size();
            for (int i = 0; i < vMobSize; i++)
			{
				Mob mob = (Mob)GameScr.vMob.elementAt(i);
				if (mob != null)
				{
					if (!mob.isMobMe && mob.status != 0 && mob.status != 1 && IsValidMob(mob) && !isMeAvoidAttack(mob))
					{
						result = mob;
						break;
					}
				}
			}
			return result;
		}
		else if (type == 1)
		{
			Mob result1 = null;
            int vMobSize1 = GameScr.vMob.size();
			int Listmob = MainMod.CurrentSetting.ListMob.Length;
            for (int i = 0; i < Listmob; i++)
			{
                int mobIndex = MainMod.CurrentSetting.ListMob[i];
                if (mobIndex < 0 || mobIndex >= vMobSize1) continue;
                Mob mob1 = (Mob)GameScr.vMob.elementAt(mobIndex);
				if (mob1 != null)
				{
					if (mob1.status != 0 && mob1.status != 1 && !mob1.isMobMe && IsValidMob(mob1) && !isMeAvoidAttack(mob1))
					{
						result1 = mob1;
						break;
					}
				}
			}
			return result1;
		}
		Mob result2 = null;
		//int num2 = 9999;
        int vMobSize2 = GameScr.vMob.size();
        for (int j = 0; j < vMobSize2; j++)
		{
			Mob mob2 = (Mob)GameScr.vMob.elementAt(j);
			if (mob2 != null)
			{
				if (mob2.status != 0 && mob2.status != 1 && mob2.hp > 0 && !mob2.isMobMe && IsValidMob(mob2) && GetMobTemplateId(mob2) && !isMeAvoidAttack(mob2))
				{
					//int num3 = Math.abs(Char.myCharz().cx - mob2.x);
					//if (num2 > num3 && IsValidMob(mob2))
					//{
					result2 = mob2;
					break;
					//	num2 = num3;
					//}
				}
			}
		}
		return result2;
	}

	public static void TurnOnAutoTrain()
	{
		//if (listMobIds.Count == 0)
		//{
		//	GameScr.info1.addInfo("Danh Sách Tàn Sát Trống!", 0);
		//	isAutoTrain = false;
		//	return;
		//}
		//if (!GameScr.canAutoPlay)
		//{
		//	isAutoTrain = true;
		//}
		//else
		//{
		//	isAutoTrain = false;
		//}
		isAutoTrain =!isAutoTrain;
		GameScr.isAutoPlay = true;
	}

	static AutoTrain()
	{
        listMobIds = new List<int>();
		gobackZoneID = -1;
		gobackMapID = -1;
        lastTimeGoback = 0L;
		SaveSecondGoback = 1;
        minimumMPGoHome = 1;
		minimumHPGoHome = 1;
        inputMPPercentGoHome = new string[2] { "Nhập %MP", "%MP" };
		LoadData();
    }

	private static void DoIt()
	{
		if (MainMod.CurrentSetting.IsUseTDLT && !ItemTime.isExistItem(4387))
		{
			return;
		}
        if (AutoPick.isAutoPick)
        {
            AutoPick.FocusToNearestItem();
            if (Char.myCharz().itemFocus != null)
            {
                AutoPick.PickIt();
				AutoPick.FocusToNearestItem();
            }
        }
		else
		{
			if(Char.myCharz().itemFocus != null) Char.myCharz().itemFocus = null;
        }
        if (Char.myCharz().mobFocus == null || Char.myCharz().mobFocus.isMobMe)
		{
			Mob nextMob = GetNextMob();
			if (nextMob != null)
			{
				Char.myCharz().currentMovePoint = null;
                Char.myCharz().mobFocus = nextMob;
			}
			//else
			//{
			//	nextMob = GetNextMobFuture();
			//	if (nextMob != null)
			//	{
			//		Char.myCharz().currentMovePoint = null;
			//		Char.myCharz().mobFocus = nextMob;
			//	}
			//}
		}
		else
		{
			if (Char.myCharz().mobFocus.hp <= 0 || Char.myCharz().mobFocus.status == 1 || Char.myCharz().mobFocus.status == 0 || (!IsValidMob(Char.myCharz().mobFocus) && !MainMod.CurrentSetting.PeansDonate)) Char.myCharz().mobFocus = null;
		}
		if (Char.myCharz().mobFocus == null || (Char.myCharz().skillInfoPaint() != null && Char.myCharz().indexSkill < Char.myCharz().skillInfoPaint().Length && Char.myCharz().dart != null && Char.myCharz().arr != null))
		{
			return;
		}
        Char me = Char.myCharz();
        Skill skill = null;
        int[] allowedIds = MainMod.CurrentSetting.ListSkill;
        for (int i = 0; i < GameScr.keySkill.Length; i++)
        {
            Skill s = GameScr.keySkill[i];
            if (s == null || !allowedIds.Contains(s.template.id))
                continue;
            if (s.paintCanNotUseSkill)
                continue;

            long manaCost = (s.template.manaUseType == 2)
                ? 1L
                : (s.template.manaUseType == 1
                    ? (s.manaUse * me.cMPFull / 100L)
                    : s.manaUse);
            if (me.cMP < manaCost)
                continue;

            if (skill == null || skill.coolDown < s.coolDown)
            {
                if (s.template.id == 13 && (me.isMonkey == 1 || me.isWaitMonkey))
                    continue;

                skill = s;
            }
        }

		if (skill != null)
		{
			if ((skill.template.id >= 0 && skill.template.id <= 5) || skill.template.id == 7 || skill.template.id == 9 || skill.template.id == 17)
			{
				if (global::Char.myCharz().myskill != skill)
				{
					GameScr.gI().doSelectSkill(skill, true);
				}
			}
			else GameScr.gI().doSelectSkill(skill, true);
			if (skill.template.type >= 3 || skill.template.id == 10 || skill.template.id == 11 || skill.template.id == 14) return;
        }
		bool canUseSkillAttack = (!ItemTime.isExistItem(4387) && (double)Math.abs(Char.myCharz().mobFocus.x - Char.myCharz().cx) < (double)skill.dx * 1.7) || ItemTime.isExistItem(4387);
		if(!canUseSkillAttack || Char.myCharz().myskill.paintCanNotUseSkill)
		{
			return;
		}
		if (mSystem.currentTimeMillis() - Char.myCharz().myskill.lastTimeUseThisSkill >= (long)(Char.myCharz().myskill.coolDown * AutoSkill.speedFactor))
		{
			if (skill.template.id == 7)
			{
				skill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
				MyVector myVector = new MyVector();
				myVector.addElement(Char.myCharz());
				Service.gI().sendPlayerAttack(new MyVector(), myVector, 1);
			}
			else if (GameScr.gI().isMeCanAttackMob(Char.myCharz().mobFocus))
			{
				skill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
				MyVector myVector = new MyVector();
				myVector.addElement(Char.myCharz().mobFocus);
				Service.gI().sendPlayerAttack(myVector, new MyVector(), 1);
			}
		}
    }

	public static void UseGrape()
	{
		for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = Char.myCharz().arrItemBag[i];
			if (item != null && item.template.id == 212)
			{
				Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
				return;
			}
		}
		for (int j = 0; j < Char.myCharz().arrItemBag.Length; j++)
		{
			Item item2 = Char.myCharz().arrItemBag[j];
			if (item2 != null && item2.template.id == 211)
			{
				Service.gI().useItem(0, 1, (sbyte)item2.indexUI, -1);
				break;
			}
		}
	}

	public static void kok()
	{
		if (Char.myCharz().meDead)
		{
			long_101 = mSystem.currentTimeMillis();
			return;
		}
		if (mSystem.currentTimeMillis() - long_101 >= 400L)
		{
            if (Char.myCharz().isLockMove|| Char.myCharz().isWaitMonkey|| Char.myCharz().isStandAndCharge|| Char.myCharz().holder) return;
            if (IsKok)
			{
				chongmkn = false;
                long_101 = mSystem.currentTimeMillis();
                if (Char.myCharz().cx != gobackX)
                {
                    Char.myCharz().cx = gobackX;
                    Char.myCharz().cy = gobackY;
                    Service.gI().charMove();
                }
                else
                {
                    Char.myCharz().cx = gobackX - 10;
                    Char.myCharz().cy = gobackY;
                    Service.gI().charMove();
                }
            }
            if (chongmkn)
            {
                IsKok = false;
                long_101 = mSystem.currentTimeMillis()+3000L;
                Char.myCharz().cx -= 10;
                Service.gI().charMove();
                Char.myCharz().cx += 10;
                Service.gI().charMove();
            }
		}
	}

    public static bool isTileTypeMap(int cx, int cy)
    {
        return TileMap.tileTypeAt(cx * 24, cy * 24, 4) || TileMap.tileTypeAt(cx * 24, cy * 24, 8) || TileMap.tileTypeAt(cx * 24, (cy - 1) * 24, 2) || TileMap.tileTypeAt(cx * 24, (cy - 1) * 24, 8192);
    }

    public static void ChayBo(int cxmob, int cymob)
	{
		int cx = Char.myCharz().cx;
		int cy = Char.myCharz().cy;
		if (cxmob != lastMobX || cymob != lastMobY || Way == null)
		{
			Way = FindPath(cx, cy, cxmob, cymob);
			currentStep = 0;
			lastMobX = cxmob;
			lastMobY = cymob;
			if (Way.Length <= 0 || Way == null)
			{
				//Console.WriteLine("path null");
				currentStep = 0;
				return;
			}
		}
		if (Char.myCharz().cx == Way[Way.Length - 1].X && Char.myCharz().cy == Way[Way.Length - 1].Y)
		{
			currentStep = 0;
			Way = null;
			return;
		}
		if (mSystem.currentTimeMillis() - TrainLoad > 75L)
		{
			TrainLoad = mSystem.currentTimeMillis();
			if (currentStep < Way.Length)
			{
				Move(Way[currentStep].X, Way[currentStep].Y);
				currentStep++;
				return;
			}
			currentStep = Way.Length - 1;
			Way = null;
		}
	}
}
