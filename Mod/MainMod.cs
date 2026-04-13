using NRO;
using QLTK;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


namespace NRO;

public class MainMod : IActionListener, IChatable
{
	public static MainMod _Instance;

	public static string account;

	public static string password;

    public static List<int> list_0;

	public static List<int> list_1;

    public static int runSpeed;

	public static bool isAutoRevive;

	public static bool isLockFocus;

	public static int charIDLock;

	public static string[] inputLockFocusCharID;

	public static bool isConnectToAccountManager;

	public static int zoneIdNRD;

	public static int mapIdNRD;

	public static bool isOpenMenuNPC;

	public static bool isAutoEnterNRDMap;

	public static string[] nameMapsNRD;

	public static int limitFPS;

    public static string[] inputHPPercentFusionDance;

	public static string[] inputHPFusionDance;

    public static int minumumHPPercentFusionDance;

	public static bool isPaintBackground;

	public static int minimumHPFusionDance;

	public static long longnhapthe;

	public static List<int> listCharIDs;

	public static string[] inputCharID;

	public static bool isAutoLockControl;

	public static bool isAutoTeleport;

	public static bool isAutoAttackBoss;

	public static int HPLimit;

	public static string[] inputHPLimit;

	public static string[] Fps;

    public static string[] inputNameBoss;

    public static bool isHuntingBoss;

	public static List<Boss> listBosses;

	public static Image logoServerListScreen;

	public static Image logoGameScreen;

	public static List<Image> listBackgroundImages;

	public static List<SKColor> listFlagColor;

	public static int widthRect;

	public static int heightRect;

	public static List<Char> listCharsInMap;

    private static List<string> ListNameBossAttack;

    public static bool isShowCharsInMap;

	public static bool isReduceGraphics;

	public static bool isUsingSkill;

	public static long lastTimeConnected;
	
	public static bool isUsingCapsule;

	public static int delay;

	public static int indexBackgroundImages;

	public static long lastTimeChangeBackground;

	public static int server;

	public static string APIKey;

	public static string APIServer;

	public static bool isSlovingCapcha;

	public static long LongBuyKigui;

	public static bool isAutoT77;

	public static bool isAutoBomPicPoc;

	public static bool AutoProtect;

	public static bool autotele;

	public static long TelePorts;

	public static bool Gohome;

	public static bool isHoiskill;

	public static int HPmax;

	public static string NameClan;

	public static bool AutoBuffHuytSao;

	public static bool BgrGoc;

	public static bool CoverSao;

	public static int IDtelePha;

	public static long Longtele;

	public static long SleepChangeZone;

	public static bool isTele;

	public static int Limitmvbt;

	public static bool isReduceGraphics1;

	public static int Hpcombo;

	public static bool autokm;

	public static readonly string[] NameClanBreak;

    public static List<int> NameClanreturn;

	public static bool Start;

    public static bool WaitLogin;

    public static long hopthe;

	public static long useSkill8;

	public static bool isNhapCodeTanThu;

	public static bool isGoHome;

	public static long isLastPickItemHome;

	public static long longGet;

	public static string GetMessServer;

	public static int CheckOutRuong;

	public static long longWaitLS;

	public static int typeUseSkill;

	public static int mapNext;

	public static long isOpentask;

	public static int IDtelegsm;

	public static bool isGSM;

	public static int optionxhp;

    public static bool OneSave;

    public static TcpConnect connectServer;

    public static Dictionary<sbyte, int> skillCooldowns = new Dictionary<sbyte, int>();
    public static Dictionary<sbyte, int> skillManause = new Dictionary<sbyte, int>();
    public static Dictionary<sbyte, int> skillUseTrain = new Dictionary<sbyte, int>();

    public static int longDelayLogin;

    public static bool telePersonNRD;

    public static bool hideChar;

    public static bool hideMob;


    public static bool isTeleBossattack;

    private static byte[] f_huynguyen721221;
    public static AccountSetting CurrentSetting { get; set; }

    public static MainMod getInstance()
	{
		if (_Instance == null)
		{
			_Instance = new MainMod();
		}
        return _Instance;
	}

    public static void Update()
	{
		if (GameScr.gI().mobCapcha != null)
		{
			return;
		}
		if (!OneSave && GameCanvas.gameTick % 50 == 0)
		{
			SaveCoolDownNew();
			OneSave = true;
		}
		if (isShowCharsInMap)
		{
			listCharsInMap.Clear();
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				Char @char = (Char)GameScr.vCharInMap.elementAt(i);
				if (isChar(@char))
				{
					listCharsInMap.Add(@char);
				}
			}
			RefreshListCharsInMap();
		}
		if (Start)
		{
			if (typeUseSkill == 2)
				AutoLuyenSkill9L2();
			if (typeUseSkill == 1)
				AutoLuyenSkill9L1();
		}
		if (isAutoAttackBoss) AutoAttackBoss();
		if (isAutoRevive)
		{
			Revive();
		}
		if (isTele || isGSM)
		{
			new Thread(new ThreadStart(TelePha)).Start();
		}
		if (isLockFocus)
		{
			FocusToChar(charIDLock);
		}
		if (AutoBuffHuytSao)
		{
			BuffHuytSao();
		}
		//if (isConnectToAccountManager)
		//{
		//	try
		//	{
		//		new Thread(new ThreadStart(ConnectToAccountManager)).Start();
		//		if (!TcpConnect.StartConnect)
		//		{
		//			TcpConnect.StartConnect = true;
		//			connectServer.Connect();
		//		}
		//	}
		//	catch { }
		//}
		//else
		//{
		//	if (TcpConnect.StartConnect)
		//	{
		//		connectServer.CloseClient();
		//		TcpConnect.StartConnect = false;
		//	}
		//}

		AutoHT();
        //AutoTrain.Update();
        AutoItem.Update();
        AutoPean.Update();
        AutoSkill.Update();
        AutoPick.Update();
        AutoMap.Update();
		AutoChat.Update();
        //XuLiSettings.Update();
        Char.myCharz().cspeed = runSpeed;
	}

    public static bool isChar(Char @char)
	{
		return @char.cName != null && @char.cName != "" && !@char.isPet && !@char.isMiniPet && !@char.cName.StartsWith("#") && !@char.cName.StartsWith("$") && @char.cName != "Trọng tài";
    }

    public static void SaveCoolDownNew()
    {
        for (int i = 0; i <= 8; i++)
        {
            SkillTemplate skillTemplate = global::Char.myCharz().nClass.skillTemplates[i];
            Skill skill = global::Char.myCharz().getSkill(skillTemplate);
            if (skill != null)
            {
                sbyte id = skillTemplate.id;
                int cooldowns = skill.coolDown;
                int manaUse = skill.manaUse;
                MainMod.skillCooldowns[id] = cooldowns;
                MainMod.skillManause[id] = manaUse;
            }
        }
    }
    public static void PaintHPMP(mGraphics g)
	{
        mFont.tahoma_7_white.drawString(g,NinjaUtil.getMoneys(Char.myCharz().cHP)+"/"+ NinjaUtil.getMoneys(Char.myCharz().cHPFull), 110, 5, mGraphics.RIGHT | mGraphics.TOP);
        mFont.tahoma_7_white.drawString(g, NinjaUtil.getMoneys(Char.myCharz().cMP) + "/" + NinjaUtil.getMoneys(Char.myCharz().cMPFull), 110, 20, mGraphics.RIGHT | mGraphics.TOP);
    }

    public static void Paint(mGraphics g)
	{
        mFont.tahoma_7_white.drawString(g, TileMap.mapID + "." + TileMap.mapNames[TileMap.mapID] + " K" + TileMap.zoneID + " [X:" + Char.myCharz().cx + " Y:" + Char.myCharz().cy + "]", 20, GameCanvas.h / 3 + 10, 0);
		mFont.tahoma_7_white.drawString(g, DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), 20, GameCanvas.h / 3 + 20, 0);
        if (isShowCharsInMap)
        {
            paintListCharsInMap(g);
        }
        //paintListBosses(g);
        PaintItemMap(g);
		if (!isMeInNRDMap())
		{
            if (Char.myCharz().mobFocus != null)
            {
                PaintFocusMob(g, Char.myCharz().mobFocus);
            }
            if (Char.myCharz().charFocus != null)
            {
                statusChar(g, Char.myCharz().charFocus);
            }
        }
		int num = GameCanvas.h / 3 + 30;
		int numx = 20;
		if (isConnectToAccountManager)
		{
			mFont.tahoma_7b_white.drawString(g, "Đã kết nối!", numx, num, 0);
			num += 10;
		}
		if (AutoSkill.isAutoSendAttack)
		{
			mFont.tahoma_7b_white.drawString(g, "Tự đánh: on", numx, num, 0);
			num += 10;
		}
		if (isAutoRevive)
		{
			mFont.tahoma_7b_white.drawString(g, "Hồi sinh: on", numx, num, 0);
			num += 10;
		}
		if (AutoPick.isAutoPick)
		{
			mFont.tahoma_7b_white.drawString(g, "Auto nhặt: on ", numx, num, 0);
			num += 10;
		}
		if (isLockFocus)
		{
			mFont.tahoma_7b_white.drawString(g, "Khóa: " + charIDLock, numx, num, 0);
			num += 10;
		}
		if (isAutoEnterNRDMap)
		{
			mFont.tahoma_7b_white.drawString(g, "Đang auto nrd: " + mapIdNRD + "sk" + zoneIdNRD, numx, num, 0);
			num += 10;
		}
		if (AutoBuffHuytSao)
		{
			mFont.tahoma_7b_white.drawString(g, "Cover sáo", numx, num, 0);
			num += 10;
		}
		//if (autokm)
		//{
		//	mFont.tahoma_7b_white.drawString(g, "Auto Kame: " + NinjaUtil.getMoneys(Hpcombo) + " HP", numx, num, 0);
		//	num += 10;
		//}
		if (AutoTrain.chongmkn)
		{
			mFont.tahoma_7b_white.drawString(g, "Chống mkn: on", numx, num, 0);
			num += 10;
		}
		PaintAction(g, 25, num);
	}
    public static bool isMeInNRDMap()
    {
        if (TileMap.mapID >= 85)
        {
            return TileMap.mapID <= 91;
        }
        return false;
    }

    public static void PaintLogo(mGraphics g)
	{
        g.drawImage(logoGameScreen, GameCanvas.w / 3 + 5, 10 - logoGameScreen.getHeight() / 3, 0);
    }

	public static string convertHP(long HP)
	{
		float num = (float)HP;
		if (num >= 1000000000)
		{
			float num3 = num / 1000000000;
			return ($"{num3:F2}" + "B");
		}
		if (num < 1000000 && num >= 1000)
		{
			return num / 1000 + "K";
		}
		if (num < 1000)
		{
			return num.ToString();
		}
		float num2 = num / 1000000;
		return ($"{num2:F2}" + "M");
	}

    // Helper vẽ nối đuôi các đoạn chữ 
    private static void DrawSegments(mGraphics g, int x, int y, params (string text, mFont font)[] segments)
    {
        int currX = x;
        foreach (var seg in segments)
        {
            if (string.IsNullOrEmpty(seg.text)) continue;
            seg.font.drawString(g, seg.text, currX, y, 0);
            currX += seg.font.getWidth(seg.text); // Tự động tính khoảng cách cho đoạn tiếp theo [cite: 2026-04-09]
        }
    }
    public static void paintListCharsInMap(mGraphics g)
    {
        int startY = (!isHuntingBoss || isMeInNRDMap()) ? 35 : 95;
        widthRect = (mGraphics.zoomLevel == 1) ? 170 : 160;
        heightRect = 9;
        int lineYOffset = (mGraphics.zoomLevel == 1) ? 0 : 1;

        for (int i = 0; i < listCharsInMap.Count; i++)
        {
            Char @char = listCharsInMap[i];
            int drawX = GameCanvas.w - widthRect + 16;

            // 1. Vẽ nền [cite: 2026-04-10]
            g.setColor(2721889, 0.5f);
            g.fillRect(drawX - 1, startY + 2, widthRect - 2, heightRect);

            if (!isChar(@char)) continue;

            // 2. Định nghĩa kịch bản vẽ (Segments) [cite: 2026-04-06]
            string prefix = $"{i + 1}.{@char.getGender()}:";
            string name = @char.cName;
            string hpInfo = $"[{MainMod.convertHP(@char.cHP)}/{MainMod.convertHP(@char.cHPFull)}]";

            // Chọn kịch bản màu sắc dựa trên điều kiện [cite: 2026-04-10]
            if (Char.myCharz().charFocus == @char) // Đang focus
            {
                DrawSegments(g, drawX, startY,
                    (@char.getGender() + ":" + name, mFont.tahoma_7_red),
                    (hpInfo, mFont.tahoma_7_white));
            }
            else if (MainMod.isBoss(@char)) // Là Boss
            {
                DrawSegments(g, drawX, startY,
                    ((i + 1) + ".", mFont.tahoma_7),
                    (name, mFont.tahoma_7_red));
            }
            else if (@char.cHPFull >= 100000000 && isMeInNRDMap()) // Siêu nhân HP cao
            {
                mFont.tahoma_7_yellow.drawString(g, prefix + name, drawX, startY, 0);
            }
            else // Người chơi bình thường
            {
                DrawSegments(g, drawX, startY + lineYOffset,
                    (prefix, mFont.tahoma_7),
                    (name, mFont.tahoma_7_white),
                    ($"[{MainMod.convertHP(@char.cHP)}]", mFont.tahoma_7));
            }

            // 3. Vẽ cờ (Flag) [cite: 2026-04-10]
            if (@char.cFlag > 0 && @char.cFlag < listFlagColor.Count)
            {
                g.fillRect(GameCanvas.w - widthRect + 8, startY + 3, heightRect - 2, heightRect - 2);
            }
            startY += heightRect + 1;
        }
    }

    public static void paintListBosses(mGraphics g)
	{
		if (isHuntingBoss && !isMeInNRDMap())
		{
			int num = 42;
			for (int i = 0; i < listBosses.Count; i++)
			{
				g.setColor(2721889, 0.5f);
				g.fillRect(GameCanvas.w - 23, num + 2, 21, 9);
				listBosses[i].Paint(g, GameCanvas.w - 2, num, mFont.RIGHT);
				num += 10;
			}
		}
	}

    public void onChatFromMe(string text, string to)
	{
		if (ChatTextField.gI().tfChat.getText() == null || ChatTextField.gI().tfChat.getText().Equals(string.Empty) || text.Equals(string.Empty) || text == null)
		{
			ChatTextField.gI().isShow = false;
			ResetChatTextField();
		}
		if (ChatTextField.gI().strChat.Equals(MainMod.Fps[0]))
		{
			try
			{
				MainMod.limitFPS = int.Parse(ChatTextField.gI().tfChat.getText());
				string text3 = ChatTextField.gI().tfChat.getText();
				GameScr.info1.addInfo("FPS: " + text3, 0);
			}
			catch
			{
			}
			MainMod.ResetChatTextField();
			return;
		}
		else if (ChatTextField.gI().strChat.Equals(inputLockFocusCharID[0]))
		{
			try
			{
				int num = (charIDLock = int.Parse(ChatTextField.gI().tfChat.getText()));
				isLockFocus = true;
				GameScr.info1.addInfo("Đã Thêm: " + num, 0);
			}
			catch
			{
				GameScr.info1.addInfo("CharID Không Hợp Lệ, Vui Lòng Nhập Lại", 0);
			}
			ResetChatTextField();
		}
		else if (ChatTextField.gI().strChat.Equals(inputHPFusionDance[0]))
		{
			try
			{
				int num2 = (minimumHPFusionDance = int.Parse(ChatTextField.gI().tfChat.getText()));
				GameScr.info1.addInfo("Hợp Thể Khi HP Dưới: " + Res.formatNumber2(num2), 0);
			}
			catch
			{
				GameScr.info1.addInfo("HP Không Hợp Lệ, Vui Lòng Nhập Lại!", 0);
			}
			ResetChatTextField();
		}
        else if (ChatTextField.gI().strChat.Equals(inputNameBoss[0]))
        {
            try
            {
                string num4 = ChatTextField.gI().tfChat.getText();
                ListNameBossAttack.Add(num4.ToLower());
                GameScr.info1.addInfo("Đã Thêm clan: " + num4, 0);
            }
            catch (Exception ex)
            {
                GameScr.info1.addInfo("Tên Boss Không Hợp Lệ, Vui Lòng Nhập Lại ", 0);                
            }
            ResetChatTextField();
        }
        else
		{
			if (!ChatTextField.gI().strChat.Equals(inputHPPercentFusionDance[0]))
			{
				return;
			}
			try
			{
				int num5 = int.Parse(ChatTextField.gI().tfChat.getText());
				if (num5 > 99)
				{
					num5 = 99;
				}
				minumumHPPercentFusionDance = num5;
				GameScr.info1.addInfo("Hợp Thể Khi HP Dưới: " + num5 + "%", 0);
			}
			catch
			{
				GameScr.info1.addInfo("%HP Không Hợp Lệ, Vui Lòng Nhập Lại!", 0);
			}
			ResetChatTextField();
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
				if((int)p == 1)
					AutoMap.ShowMenu();
                if ((int)p == 2)
                    AutoSkill.ShowMenu();
                if ((int)p == 3)
                    AutoPean.ShowMenu();
                if ((int)p == 4)
                    AutoPick.ShowMenu();
                if ((int)p == 5)
                    AutoTrain.ShowMenu();
                if ((int)p == 6)
                    AutoChat.ShowMenu();
                if ((int)p == 7)
                    AutoPoint.ShowMenu();
                if ((int)p == 8)
                    ShowMenuBoss();
                if ((int)p == 9)
                    ShowMenuMore();
                break;
            case 2:
                isAutoAttackBoss = !isAutoAttackBoss;
                GameScr.info1.addInfo("Auto tấn công boss", 0);
                break;
            case 3:
				isTeleBossattack = !isTeleBossattack;
				GameScr.info1.addInfo("Auto dịch chuyển tới boss", 0);
                break;
			case 4:
                ChatTextField.gI().strChat = inputNameBoss[0];
                ChatTextField.gI().tfChat.name = inputNameBoss[1];
                ChatTextField.gI().startChat2(getInstance(), string.Empty);
                break;
			case 5:
				ShowMenuListBoss();
                WriteListData_File("Data\\Boss.ini", ListNameBossAttack);
                break;
			case 6:
				ListNameBossAttack.RemoveAt((int)p);
				WriteListData_File("Data\\Boss.ini", ListNameBossAttack);
                ShowMenuListBoss();
                break;
			case 7:

				break;
			case 8:

				break;
			case 9:
				if (minumumHPPercentFusionDance > 0)
				{
					minumumHPPercentFusionDance = 0;
					GameScr.info1.addInfo("Hợp thể khi HP dưới: 0% HP", 0);
				}
				else
				{
					ChatTextField.gI().strChat = inputHPPercentFusionDance[0];
					ChatTextField.gI().tfChat.name = inputHPPercentFusionDance[1];
					ChatTextField.gI().startChat2(getInstance(), string.Empty);
				}
				break;
			case 10:
				if (minimumHPFusionDance > 0)
				{
					minimumHPFusionDance = 0;
					GameScr.info1.addInfo("Hợp thể khi HP dưới: 0", 0);
				}
				else
				{
					ChatTextField.gI().strChat = inputHPFusionDance[0];
					ChatTextField.gI().tfChat.name = inputHPFusionDance[1];
					ChatTextField.gI().startChat2(getInstance(), string.Empty);
				}
				break;
			case 11:
				break;
			case 12:
				AutoTrain.chongmkn = !AutoTrain.chongmkn;
				GameScr.info1.addInfo("Chống mkn\n" + (AutoTrain.chongmkn ? "[ON] " : "[OFF]"), 0);
				break;
			case 16:
				{
					int num = (int)p;
					if (num != 0)
					{
						listCharIDs.Add(num);
						GameScr.info1.addInfo("Đã Thêm: " + num, 0);
					}
					break;
				}
			case 17:
				{
					int num2 = (int)p;
					if (num2 != 0)
					{
						listCharIDs.Remove(num2);
						GameScr.info1.addInfo("Đã Xóa: " + num2, 0);
					}
					break;
				}
			case 19:

                break;
			case 20:
				ChatTextField.gI().strChat = inputHPLimit[0];
				ChatTextField.gI().tfChat.name = inputHPLimit[1];
				ChatTextField.gI().startChat2(getInstance(), string.Empty);
				break;
			case 24:
				GameScr.info1.addInfo("Tính năng đang xây dựng!", 0);
				break;

			case 26:
				GameScr.info1.addInfo("Tính năng đang xây dựng!", 0);
				break;
			case 27:
				isReduceGraphics1 = !isReduceGraphics1;
				//Application.targetFrameRate = 30;
				Rms.saveRMSInt("IsReduceGraphics1", isReduceGraphics1 ? 1 : 0);				
				break;
			case 28:
				isPaintBackground = !isPaintBackground;
				Rms.saveRMSInt("isPaintBgr", isPaintBackground ? 1 : 0);
				break;
			case 29:
				isHuntingBoss = !isHuntingBoss;
				Rms.saveRMSInt("sanboss", isHuntingBoss ? 1 : 0);
				break;
			case 30:
				isShowCharsInMap = !isShowCharsInMap;
				Rms.saveRMSInt("showchar", isShowCharsInMap ? 1 : 0);
				break;
			case 31:
				ChatTextField.gI().strChat = MainMod.Fps[0];
				ChatTextField.gI().tfChat.name = MainMod.Fps[1];
				ChatTextField.gI().startChat2(MainMod.getInstance(), string.Empty);
				break;

			case 37:
				ShowMenuGDL();
				break;
			case 38:
				ShowMenupotara();
				break;

            case 40:
                AutoItem.useSet(0);
                GameScr.info1.addInfo("mặc set 1", 0);
                break;
            case 41:
                AutoItem.useSet(1);
                GameScr.info1.addInfo("mặc set 2", 0);
                break;
            case 42:
                //xhp bom
                optionxhp = 1;
                GameScr.info1.addInfo("đang xhp bom!", 0);
                Xhp();
                break;
            case 43:
                //xhp ôm
                optionxhp = 0;
                GameScr.info1.addInfo("đang xhp ôm!", 0);
                Xhp();
                break;
			case 44:
                telePersonNRD =! telePersonNRD;
				GameScr.info1.addInfo("dịch chuyển đến người ôm\n" + (MainMod.telePersonNRD ? "[ON] " : "[OFF]"), 0);
                break;

		}

	}
    public static bool UpdatekeywithControl(int unused)
    {
        if (unused == Hotkeys.A)
        {
            AutoSkill.isAutoSendAttack = !AutoSkill.isAutoSendAttack;
            GameScr.info1.addInfo("Tự Đánh\n" + (AutoSkill.isAutoSendAttack ? "[ON]" : "[OFF]"), 0);
            return true;
        }
        if (unused == Hotkeys.B)
        {
            Service.gI().friend(0, -1);
            return true;
        }
        if (unused == Hotkeys.U)
        {
            isHoiskill = true;
            new Thread(new ThreadStart(MainMod.Hoiskill)).Start();
            return true;
        }
        if (unused == Hotkeys.C)
        {
            for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
            {
                Item item = Char.myCharz().arrItemBag[i];
                if (item != null && (item.template.id == 194 || item.template.id == 193))
                {
                    Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
                    break;
                }
            }
            return true;
        }
        if (unused == Hotkeys.D)
        {
            AutoSkill.FreezeSelectedSkill();
            return true;             
        }
        if (unused == Hotkeys.E)
        {
            isAutoRevive = !isAutoRevive;
            GameScr.info1.addInfo("Auto Hồi Sinh\n" + (isAutoRevive ? "[ON]" : "[OFF]"), 0);
            return true;
        }
        if (unused == Hotkeys.F)
        {
            UsePorata();
            Service.gI().petStatus(3);
            return true;
        }
        if (unused == Hotkeys.G)
        {
			if (isMeInNRDMap())
			{
                Service.gI().requestChangeZone(TileMap.zoneID, -1);

				return true;
            }
            if (Char.myCharz().charFocus == null)
            {
                GameScr.info1.addInfo("Vui Lòng Chọn Mục Tiêu!", 0);
            }
            else
            {
                Service.gI().giaodich(0, Char.myCharz().charFocus.charID, -1, -1);
                GameScr.info1.addInfo("Đã Gửi Lời Mời Giao Dịch Đến: " + Char.myCharz().charFocus.cName, 0);
            }
            return true;
        }
        if (unused == Hotkeys.I)
        {
            isLockFocus = !isLockFocus;
            if (!isLockFocus)
            {
                GameScr.info1.addInfo("Khoá Mục Tiêu\n[OFF]", 0);
            }
            else if (Char.myCharz().charFocus == null)
            {
                isLockFocus = false;
                ChatTextField.gI().strChat = inputLockFocusCharID[0];
                ChatTextField.gI().tfChat.name = inputLockFocusCharID[1];
                ChatTextField.gI().startChat2(getInstance(), string.Empty);
            }
            else
            {
                charIDLock = Char.myCharz().charFocus.charID;
                GameScr.info1.addInfo("Đã Khóa: " + Char.myCharz().charFocus.cName, 0);
            }
            return true;
        }
        if (unused == Hotkeys.V)
        {
            teleVFocus();
            return true;
        }
        if (unused == Hotkeys.J)
        {
            AutoMap.LoadMapLeft();
            return true;
        }
        if (unused == Hotkeys.K)
        {
            AutoMap.LoadMapCenter();
            return true;
        }
        if (unused == Hotkeys.L)
        {
            AutoMap.LoadMapRight();
            return true;
        }
        if (unused == Hotkeys.M)
        {
            //GameCanvas.panel.isShowZone = true;
            Service.gI().openUIZone();
            return true;
        }
        if (unused == Hotkeys.N)
        {
            if (isMeInNRDMap())
            {
                AutoPick.isAutoPick = !AutoPick.isAutoPick;
                GameScr.info1.addInfo("Auto Nhặt\n" + (AutoPick.isAutoPick ? "[ON]" : "[OFF]"), 0);
            }
            else
            {
                AutoPick.ShowMenu();
            }
            return true;
        }
        if (unused == Hotkeys.H)
        {
			ShowMenuXHP();
            return true;
        }
        if (unused == Hotkeys.O)
        {
            isAutoEnterNRDMap = !isAutoEnterNRDMap;
            isOpenMenuNPC = isAutoEnterNRDMap;
            new Thread(new ThreadStart(EnterNRDMap)).Start();
            GameScr.info1.addInfo("Auto Vào NRD\n" + (isAutoEnterNRDMap ? "[ON]" : "[OFF]"), 0);
            return true;
        }
        if (unused == Hotkeys.W)
        {
            AutoProtect = !AutoProtect;
            GameScr.info1.addInfo("Auto Khiên" + (AutoProtect ? "[ON]" : "[OFF]"), 0);
            return true;
        }
        if (unused == Hotkeys.P)
        {
            isConnectToAccountManager = !isConnectToAccountManager;
            GameScr.info1.addInfo("Kết Nối\n" + (isConnectToAccountManager ? "[ON]" : "[OFF]"), 0);
            return true;
        }
        if (unused == Hotkeys.T)
        {
            UseItem(521);
            return true;
        }
        if (unused == Hotkeys.Z)
        {
            if (!isMeInNRDMap() && !AutoBuffHuytSao)
            {
                GameScr.info1.addInfo("Chỉ dùng trong map ngọc rồng sao đen", 0);
                return false;
            }
            AutoBuffHuytSao = !AutoBuffHuytSao;
            GameScr.info1.addInfo("Auto Cover sáo", 0);
            return true;
        }
        if (unused == Hotkeys.X)
        {
            ShowMenu();
            return true;
        }
        if (unused == Hotkeys.S)
        {
			FocusOn();
            return true;
        }
        return false;
    }

    public static bool UpdateKey(int unused)
	{
        if (GameCanvas.keyAsciiPress == Hotkeys._)
        {
            MainMod.ShowMenuChangeSet();
            return true;
        }
        if (GameCanvas.keyAsciiPress == Hotkeys.A)
		{
			AutoSkill.isAutoSendAttack = !AutoSkill.isAutoSendAttack;
			GameScr.info1.addInfo("Tự Đánh\n" + (AutoSkill.isAutoSendAttack ? "[ON]" : "[OFF]"), 0);
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.B)
		{
			Service.gI().friend(0, -1);
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.C)
		{
			for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
			{
				Item item = Char.myCharz().arrItemBag[i];
				if (item != null && (item.template.id == 194 || item.template.id == 193))
				{
					Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
					break;
				}
			}
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.D)
		{
            AutoSkill.FreezeSelectedSkill();
            return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.E)
		{
			isAutoRevive = !isAutoRevive;
			GameScr.info1.addInfo("Auto Hồi Sinh\n" + (isAutoRevive ? "[ON]" : "[OFF]"), 0);
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.F)
		{
			UsePorata();
			Service.gI().petStatus(3);
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.G)
		{
			if (Char.myCharz().charFocus == null)
			{
				GameScr.info1.addInfo("Vui Lòng Chọn Mục Tiêu!", 0);
			}
			else
			{
				Service.gI().giaodich(0, Char.myCharz().charFocus.charID, -1, -1);
				GameScr.info1.addInfo("Đã Gửi Lời Mời Giao Dịch Đến: " + Char.myCharz().charFocus.cName, 0);
			}
			return true;
		}
        if (GameCanvas.keyAsciiPress == Hotkeys.H)
        {
			ShowMenuXHP();
            return true;
        }
        if (GameCanvas.keyAsciiPress == Hotkeys.I)
		{
			isLockFocus = !isLockFocus;
			if (!isLockFocus)
			{
				GameScr.info1.addInfo("Khoá Mục Tiêu\n[OFF]", 0);
			}
			else if (Char.myCharz().charFocus == null)
			{
				isLockFocus = false;
				ChatTextField.gI().strChat = inputLockFocusCharID[0];
				ChatTextField.gI().tfChat.name = inputLockFocusCharID[1];
				ChatTextField.gI().startChat2(getInstance(), string.Empty);
			}
			else
			{
				charIDLock = Char.myCharz().charFocus.charID;
				GameScr.info1.addInfo("Đã Khóa: " + Char.myCharz().charFocus.cName, 0);
			}
			return true;
		}
        if (GameCanvas.keyAsciiPress == Hotkeys.V)
		{
			teleVFocus();
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.J)
		{
			AutoMap.LoadMapLeft();
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.K)
		{
			AutoMap.LoadMapCenter();
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.L)
		{
			AutoMap.LoadMapRight();
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.M)
		{
			//GameCanvas.panel.isShowZone = true;
			Service.gI().openUIZone();
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.N)
		{
			if (isMeInNRDMap())
			{
				AutoPick.isAutoPick = !AutoPick.isAutoPick;
				GameScr.info1.addInfo("Auto Nhặt\n" + (AutoPick.isAutoPick ? "[ON]" : "[OFF]"), 0);
			}
			else
			{
				AutoPick.ShowMenu();
			}
			return true;
		}
        if (GameCanvas.keyAsciiPress == Hotkeys.U)
        {
            isHoiskill = true;
            new Thread(new ThreadStart(MainMod.Hoiskill)).Start();
            return true;
        }
        if (GameCanvas.keyAsciiPress == Hotkeys.O)
		{
			isAutoEnterNRDMap = !isAutoEnterNRDMap;
			isOpenMenuNPC = isAutoEnterNRDMap;
            new Thread(new ThreadStart(EnterNRDMap)).Start();
            GameScr.info1.addInfo("Auto Vào NRD\n" + (isAutoEnterNRDMap ? "[ON]" : "[OFF]"), 0);
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.W)
		{
			AutoProtect = !AutoProtect;
			GameScr.info1.addInfo("Auto Khiên" + (AutoProtect ? "[ON]" : "[OFF]"), 0);
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.P)
		{
			isConnectToAccountManager = !isConnectToAccountManager;
			GameScr.info1.addInfo("Kết Nối\n" + (isConnectToAccountManager ? "[ON]" : "[OFF]"), 0);
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.T)
		{
			UseItem(521);
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.Z)
		{
			if (!isMeInNRDMap() && !AutoBuffHuytSao)
			{
				GameScr.info1.addInfo("Chỉ dùng trong map ngọc rồng sao đen", 0);
				return false;
			}
			AutoBuffHuytSao = !AutoBuffHuytSao;
			GameScr.info1.addInfo("Auto Cover sáo", 0);
            return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.X)
		{
			ShowMenu();
			return true;
		}
		if (GameCanvas.keyAsciiPress == Hotkeys.S)
		{
            FocusOn();
            return true;
		}
		return false;
	}

	private static void AutoAttackBoss()
	{
		if (!AutoSkill.isAutoSendAttack)
		{
			AutoSkill.isAutoSendAttack = true;
		}
		for (int k = 0; k < GameScr.vCharInMap.size(); k++)
		{
			Char char2 = (Char)GameScr.vCharInMap.elementAt(k);
			if (char2 != null && isBoss(char2) && ListNameBossAttack.Contains(char2.cName.ToLower()))
			{
				if(Char.myCharz().charFocus!= char2) Char.myCharz().charFocus = char2;
				if(isTeleBossattack&& (Math.abs(Char.myCharz().cx - Char.myCharz().charFocus.cx)>60|| Math.abs(Char.myCharz().cy - Char.myCharz().charFocus.cy) > 60 )&& GameCanvas.gameTick %50==0) TeleportToFocus();
				break;
			}
		}
	}
    public static void ShowMenuBoss()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command("Auto Boss\n" + (isAutoAttackBoss ? "[ON]" : "[OFF]"), getInstance(), 2, null));
        myVector.addElement(new Command("Tele Boss\n" + (isTeleBossattack ? "[ON]" : "[OFF]"), getInstance(), 3, null));
        myVector.addElement(new Command("Thêm Boss", getInstance(), 4, null));
        myVector.addElement(new Command("Danh sách\n Boss", getInstance(), 5, null));
        GameCanvas.menu.startAt(myVector, 3);
    }
    public static void ShowMenuListBoss()
    {
        MyVector myVector = new MyVector();
        for (int i = 0; i < ListNameBossAttack.Count; i++)
        {
            myVector.addElement(new Command("Xóa Boss:\n" + ListNameBossAttack[i], getInstance(), 6, i));
        }
        GameCanvas.menu.startAt(myVector, 3);
    }

    public static void ShowMenu()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command("Auto Map", getInstance(), 1, 1));
		myVector.addElement(new Command("Auto Skill", getInstance(), 1, 2));
        myVector.addElement(new Command("Auto Pean", getInstance(), 1, 3));
		myVector.addElement(new Command("Auto Pick", getInstance(), 1, 4));
		myVector.addElement(new Command("Auto Train", getInstance(), 1, 5));
		myVector.addElement(new Command("Auto Chat", getInstance(), 1, 6));
		myVector.addElement(new Command("Auto Point", getInstance(), 1, 7));
        myVector.addElement(new Command("Auto Boss", getInstance(), 1, 8));
        myVector.addElement(new Command("More", getInstance(), 1, 9));
		GameCanvas.menu.startAt(myVector,3);
	}

    public static void ShowMenuMore()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command("Auto Hợp Thể", getInstance(), 38, null));
        myVector.addElement(new Command("Dịch Theo\nNRD\n" + (telePersonNRD ? "[ON] " : "[OFF]"), getInstance(), 44, null));
        myVector.addElement(new Command("Chống Mkn\n" + (AutoTrain.chongmkn ? "[ON] " : "[OFF]"), getInstance(), 12, null));
		myVector.addElement(new Command("Background\n" + (isPaintBackground ? "[ON] " : "[OFF]"), getInstance(), 28, null));
		myVector.addElement(new Command("Thông Báo\nBoss\n" + (isHuntingBoss ? "[ON] " : "[OFF]"), getInstance(), 29, null));
		myVector.addElement(new Command("Người Trong Map\n" + (isShowCharsInMap ? "[ON] " : "[OFF]"), getInstance(), 30, null));
		myVector.addElement(new Command("Giảm\nCấu Hình", getInstance(), 37, null));
		GameCanvas.menu.startAt(myVector, 3);
	}

    public static void ShowMenuChangeSet()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command("Mặc set 1\n", getInstance(), 40, null));
        myVector.addElement(new Command("Mặc set 2\n", getInstance(), 41, null));
        GameCanvas.menu.startAt(myVector, 3);
    }

    public static void ShowMenuXHP()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command("Xhp bom\n", getInstance(), 42, null));
        myVector.addElement(new Command("Xhp ôm\n", getInstance(), 43, null));
        GameCanvas.menu.startAt(myVector, 3);
    }

    public static void showListCoolDown()
    {
        MyVector myVector = new MyVector();
        for (int i = 0; i < GameScr.keySkill.Length; i++)
        {
            myVector.addElement(new Command(((GameScr.keySkill[i] != null) ? GameScr.keySkill[i].template.name : "null") + "\n[" + GameScr.keySkill[i].coolDown + "]\n", getInstance(), 44, i));
        }
        GameCanvas.menu.startAt(myVector, 3);
    }

    public static void ResetChatTextField()
	{
		ChatTextField.gI().strChat = "Chat";
		ChatTextField.gI().tfChat.name = "chat";
		ChatTextField.gI().isShow = false;
	}

	public static void UseItem(int templateId)
	{
		for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = Char.myCharz().arrItemBag[i];
			if (item != null && item.template.id == templateId)
			{
				Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
				break;
			}
		}
	}

	public static void UseItemporata()
	{
		for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = Char.myCharz().arrItemBag[i];
			if (item != null && (item.template.id == 454 || item.template.id == 921))
			{
				Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
				break;
			}
		}
	}

	public static void TeleportTo(int x, int y)
	{
		if ((Char.myCharz().isLockMove && Char.myCharz().npcFocus == null ) || Char.myCharz().stone || Char.myCharz().isCharge || Char.myCharz().isWaitMonkey || Char.myCharz().isStandAndCharge || Char.myCharz().holder)
		{
			GameScr.info1.addInfo("Đang không thể di chuyển!", 0);
			return;
		}
		Char.myCharz().cx = x;
		Char.myCharz().cy = y;
		Service.gI().charMove();
        Char.myCharz().cx = x;
        Char.myCharz().cy = y + 1;
        Service.gI().charMove();
        Char.myCharz().cx = x;
        Char.myCharz().cy = y;
        Service.gI().charMove();
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

	static MainMod()
	{
		account = "";
		limitFPS = 50;
		listFlagColor = new List<SKColor>();
		listCharsInMap = new List<Char>();
        connectServer = new TcpConnect();
		ListNameBossAttack = new List<string>();
        isShowCharsInMap = true;
		isHuntingBoss = true;
		listBosses = new List<Boss>();
		listBackgroundImages = new List<Image>();
       
		Fps = new string[]
		{
				"Nhập FPS ",
				"fps: 6 -> 120"
		};
		inputHPLimit = new string[2] { "Nhập HP:", "HP" };
        inputNameBoss = new string[2] { "Nhập Tên boss:", "Name Boss" };
        listCharIDs = new List<int>();
		inputCharID = new string[2] { "Nhập charID:", "charID" };
		inputHPPercentFusionDance = new string[2] { "Nhập %HP ", "%HP" };
		inputHPFusionDance = new string[2] { "Nhập HP", "HP" };

        nameMapsNRD = new string[7] { "Hành tinh M-2", "Hành tinh Polaris", "Hành tinh Cretaceous", "Hành tinh Monmaasu", "Hành tinh Rudeeze", "Hành tinh Gelbo", "Hành tinh Tigere" };
		inputLockFocusCharID = new string[2] { "Nhập charID lock", "charID" };
		list_0 = new List<int>();
		list_1 = new List<int>();
        MainMod.f_huynguyen721221 = new byte[]
		{
            (byte)0xD5U, (byte)0x75U, (byte)0xD4U, (byte)0x82U, (byte)0xFDU, (byte) 0x27U, (byte) 0x95U, (byte) 0x61U,
            (byte) 0xCBU, (byte)0x53U, (byte)0x95U, (byte)0x2DU, (byte)0xD8U, (byte)0xBFU, (byte)0xA9U, (byte)0x52U,
            (byte)0xD5U, (byte)0x75U, (byte)0xD4U, (byte)0x82U, (byte)0xD8U, (byte)0xBFU, (byte)0xA9U, (byte)0x52U,
        };
        runSpeed = 8;
	}

	public static void Revive()
	{
		if (Char.myCharz().luong + Char.myCharz().luongKhoa > 0 && Char.myCharz().meDead && Char.myCharz().cHP <= 0 && GameCanvas.gameTick % 20 == 0)
		{
			Service.gI().wakeUpFromDead();
			Char.myCharz().meDead = false;
			Char.myCharz().statusMe = 1;
			Char.myCharz().cHP = Char.myCharz().cHPFull;
			Char.myCharz().cMP = Char.myCharz().cMPFull;
			Char @char = Char.myCharz();
			Char char2 = Char.myCharz();
			Char.myCharz().cp3 = 0;
			char2.cp2 = 0;
			@char.cp1 = 0;
			ServerEffect.addServerEffect(109, Char.myCharz(), 2);
			GameScr.gI().center = null;
			GameScr.isHaveSelectSkill = true;
		}
	}

	public static void FocusToChar(int charId)
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			Char @char = (Char)GameScr.vCharInMap.elementAt(i);
			if (!@char.isMiniPet && !@char.isPet && @char.charID == charId)
			{
				Char.myCharz().mobFocus = null;
				Char.myCharz().npcFocus = null;
				Char.myCharz().itemFocus = null;
				Char.myCharz().charFocus = @char;
				break;
			}
		}
	}

	public static bool smethod_6(int int_0)
	{
		return list_0.Contains(int_0);
	}

	public static void TeleportToFocus()
	{
		if (Char.myCharz().charFocus != null)
		{
			TeleportTo(Char.myCharz().charFocus.cx, Char.myCharz().charFocus.cy);
		}
		else if (Char.myCharz().itemFocus != null)
		{
			TeleportTo(Char.myCharz().itemFocus.x, Char.myCharz().itemFocus.y);
		}
		else if (Char.myCharz().mobFocus != null)
		{
			TeleportTo(Char.myCharz().mobFocus.x, Char.myCharz().mobFocus.y);
		}
		else if (Char.myCharz().npcFocus != null)
		{
			TeleportTo(Char.myCharz().npcFocus.cx, Char.myCharz().npcFocus.cy - 3);
		}
		else
		{
			GameScr.info1.addInfo("Không Có Mục Tiêu!", 0);
		}
	}

	public static bool isBoss(Char ch)
	{
		if (ch.cName != null && ch.cName != "" && !ch.isPet && !ch.isMiniPet && char.IsUpper(char.Parse(ch.cName.Substring(0, 1))) && ch.cName != "Trọng tài" && !ch.cName.StartsWith("#"))
		{
			return !ch.cName.StartsWith("$");
		}
		return false;
	}

	public static void EnterNRDMap()
	{
		while (isAutoEnterNRDMap)
		{
			if (TileMap.mapID != Char.myCharz().cgender + 24 && TileMap.mapID != mapIdNRD + 84)
			{
				AutoMap.StartRunToMapId(Char.myCharz().cgender + 24);
			}
			if (isOpenMenuNPC && (TileMap.mapID >= 24 && TileMap.mapID <= 26) && GameCanvas.gameTick % 20 == 0)
			{
				if (!GameCanvas.menu.showMenu)
				{
					Service.gI().openMenu(29);
				}
                Thread.Sleep(20);
                Service.gI().confirmMenu(29, (sbyte)(GameCanvas.menu.menuItems.size() - 2));
                Thread.Sleep(20);
                if (GameCanvas.panel.mapNames != null && GameCanvas.panel.mapNames.Length > 6 && GameCanvas.panel.mapNames[mapIdNRD - 1] == TileMap.mapNames[mapIdNRD + 84])
				{
                    Service.gI().requestMapSelect(mapIdNRD - 1);
				}
			}
			if (isMeInNRDMap() && TileMap.mapID == mapIdNRD + 84)
			{
                if (TileMap.zoneID != zoneIdNRD)
				{
					if (!Char.isLoadingMap && !global::Char.ischangingMap && !Controller.isStopReadMessage)
					{
						Thread.Sleep(400);
						Service.gI().requestChangeZone(zoneIdNRD, -1);
					}
				}
				else
				{
					isAutoEnterNRDMap = false;
					isOpenMenuNPC = false;
					break;
				}
			}
		}
	}

	public static bool Chat(string text)
	{
		if (text.Equals(""))
		{
			return false;
		}
		if (text.StartsWith("ls_"))
		{
			try
			{
				Start = !Start;
				typeUseSkill = int.Parse(text.Split(' ')[1]);
				GameScr.info1.addInfo("Auto Luyện Skill\n" + (Start ? "[ON]" : "[OFF]"), 0);
			}
			catch
			{
			}
			return true;
		}
        if (text.StartsWith("kicksp"))
        {
            try
            {
                AutoSkill.isKickSuper = !AutoSkill.isKickSuper;
                GameScr.info1.addInfo("Auto kick super broly\n" + (AutoSkill.isKickSuper ? "[ON]" : "[OFF]"), 0);
            }
            catch
            {
            }
            return true;
        }
        if (text.StartsWith("k "))
		{
			try
			{
				int zoneId = int.Parse(text.Split(' ')[1]);
				Service.gI().requestChangeZone(zoneId, -1);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("tl "))
		{
			try
			{
				IDtelePha = int.Parse(text.Split(' ')[1]);
				isTele = !isTele;
				GameScr.info1.addInfo("Tele Char\n" + (isTele ? "[ON]" : "[OFF]"), 0);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("gsm "))
		{
			try
			{
				IDtelegsm = int.Parse(text.Split(' ')[1]);
				isGSM = !isGSM;
				GameScr.info1.addInfo("Tele gsm\n" + (isGSM ? "[ON]" : "[OFF]"), 0);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("kk "))
		{
			try
			{
				int num = int.Parse(text.Split(' ')[1]);
				TeleportTo(Char.myCharz().cx, Math.abs(Char.myCharz().cy - num));
				GameScr.info1.addInfo("king kong: " + num, 0);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("dt "))
		{
			try
			{
				int num2 = int.Parse(text.Split(' ')[1]);
				TeleportTo(Char.myCharz().cx, Math.abs(Char.myCharz().cy + num2));
				GameScr.info1.addInfo("độn thổ: " + num2, 0);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("tr "))
		{
			try
			{
				int num3 = int.Parse(text.Split(' ')[1]);
				Char.myCharz().cx = Char.myCharz().cx - num3;
				Service.gI().charMove();
				GameScr.info1.addInfo("dịch trái: " + num3, 0);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("ph "))
		{
			try
			{
				int num4 = int.Parse(text.Split(' ')[1]);
                Char.myCharz().cx = Char.myCharz().cx + num4;
                Service.gI().charMove();
                GameScr.info1.addInfo("dịch phải: " + num4, 0);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("s "))
		{
			try
			{
				int num5 = (runSpeed = int.Parse(text.Split(' ')[1]));
				GameScr.info1.addInfo("Tốc độ di chuyển: " + num5, 0);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("akok"))
		{
			try
			{
				AutoTrain.gobackX = Char.myCharz().cx;
				AutoTrain.gobackY = Char.myCharz().cy;
				AutoTrain.IsKok = !AutoTrain.IsKok;
				GameScr.info1.addInfo("Khoá vị trí + akok\n" + (AutoTrain.IsKok ? "[ON]" : "[OFF]"), 0);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("delayp "))
		{
			try
			{
				delay = int.Parse(text.Split(' ')[1]);
			}
			catch
			{
			}
			return true;
		}
		if (text.StartsWith("nrd "))
		{
			try
			{
				int num6 = int.Parse(text.Split(' ')[1]);
				int num7 = int.Parse(text.Split(' ')[2]);
				mapIdNRD = num6;
				zoneIdNRD = num7;
				GameScr.info1.addInfo("NRD: " + mapIdNRD + "sk" + zoneIdNRD, 0);
			}
			catch
			{
			}
			return true;
		}
        if (text.StartsWith("delay "))
        {
            try
            {
                int num6 = int.Parse(text.Split(' ')[1]);
                AutoPick.DelayPickItem = num6;
                GameScr.info1.addInfo("Delay Pick Item: " + AutoPick.DelayPickItem, 0);
            }
            catch
            {
            }
            return true;
        }
        if (text.StartsWith("hide"))
		{
			hideChar = !hideChar;
			hideMob = !hideMob;
            GameScr.vCharInMap.removeAllElements();
            GameScr.info1.addInfo("Ẩn Char và Mob\n" + (hideChar ? "[ON]" : "[OFF]"), 0);
        }

            //if (text.StartsWith("cheat "))
            //{
            //	try
            //	{
            //		float num9 = (Time.timeScale = float.Parse(text.Split(' ')[1]));
            //		GameScr.info1.addInfo("Cheat: " + num9, 0);
            //	}
            //	catch
            //	{
            //	}
            //	return true;
            //}
            //if (text.StartsWith("cheatf "))
            //{
            //	try
            //	{
            //		float num11 = (Time.timeScale = float.Parse(text.Split(' ')[1]) / 10f);
            //		GameScr.info1.addInfo("Cheat: " + num11, 0);
            //	}
            //	catch
            //	{
            //	}
            //	return true;
            //}
            //if (text.StartsWith("dctt "))
            //{
            //	try
            //	{
            //		int num12 = (Hpcombo = int.Parse(text.Split(' ')[1]));
            //		new Thread(new ThreadStart(AutoKameBoss)).Start();
            //		GameScr.info1.addInfo("Limit: " + NinjaUtil.getMoneys(num12) + "HP", 0);
            //	}
            //	catch
            //	{
            //	}
            //	return true;
            //}
            return false;
	}

	public static int MyHPPercent()
	{
		return (int)((long)Char.myCharz().cHP * 100L / Char.myCharz().cHPFull);
	}

	public static bool isMyHPLowerThan(int percent)
	{
		if (Char.myCharz().cHP > 0)
		{
			return MyHPPercent() < percent;
		}
		return false;
	}

	public static void AutoHT()
	{

		if (!Char.myCharz().meDead && mSystem.currentTimeMillis() - longnhapthe > 2000)
		{
            if (!Char.myCharz().isNhapThe && minumumHPPercentFusionDance != 0 && isMyHPLowerThan(minumumHPPercentFusionDance))
			{
				UsePorata();
				longnhapthe = mSystem.currentTimeMillis();
			}
			else if (!Char.myCharz().isNhapThe && minimumHPFusionDance != 0 && Char.myCharz().cHP < minimumHPFusionDance)
			{
                UsePorata();
                longnhapthe = mSystem.currentTimeMillis();
            }
		}
	}

	public static void ClearFocus()
	{
		Char.myCharz().charFocus = null;
		Char.myCharz().mobFocus = null;
        Char.myCharz().npcFocus = null;
        Char.myCharz().itemFocus = null;
    }

	public static void FocusOn()
	{
        if (isMeInNRDMap())
        {
            for (int j = 0; j < GameScr.vCharInMap.size(); j++)
            {
                Char @char = (Char)GameScr.vCharInMap.elementAt(j);
                if (@char.isNRD)
                {
					if (Char.myCharz().charFocus != @char || Char.myCharz().charFocus == null)
					{
						ClearFocus();
						Char.myCharz().charFocus = @char;
					}
					if (Char.myCharz().charFocus == @char)
					{
                        TeleportToFocus();
                    }
                    break;
                }
            }
        }
        else
        {
			bool FocusBigMob = true;
            for (int k = 0; k < GameScr.vMob.size(); k++)
            {
                Mob SuperMob = (Mob)GameScr.vMob.elementAt(k);
                if (SuperMob.isBigBoss())
                {
					if (Char.myCharz().mobFocus == null|| !Char.myCharz().mobFocus.isBigBoss())
					{
                        ClearFocus();
                        Char.myCharz().mobFocus = SuperMob;
                        TeleportToFocus();
                        FocusBigMob = false;
                    }
                    break;
                }
            }
			if(FocusBigMob)
			{
                for (int k = 0; k < GameScr.vCharInMap.size(); k++)
                {
                    Char char2 = (Char)GameScr.vCharInMap.elementAt(k);
                    if (!char2.cName.Equals("") && isBoss(char2) && (Char.myCharz().charFocus == null || (Char.myCharz().charFocus != null && Char.myCharz().charFocus.cName != char2.cName)))
                    {
                        ClearFocus();
                        Char.myCharz().charFocus = char2;
                        TeleportToFocus();
                        break;
                    }
                }
            }

        }
    }
	public static string GetFlagName(int flagId)
	{
		if (flagId == -1 || flagId == 0)
		{
			return "";
		}
		string text = "";
		switch (flagId)
		{
			case 1:
				text = "Cờ xanh";
				break;
			case 2:
				text = "Cờ đỏ";
				break;
			case 3:
				text = "Cờ tím";
				break;
			case 4:
				text = "Cờ vàng";
				break;
			case 5:
				text = "Cờ lục";
				break;
			case 6:
				text = "Cờ hồng";
				break;
			case 7:
				text = "Cờ cam";
				break;
			case 8:
				text = "Cờ đen";
				break;
			case 9:
				text = "Cờ Kaio";
				break;
			case 10:
				text = "Cờ Mabu";
				break;
			case 11:
				text = "Cờ xanh dương";
				break;
		}
		if (!text.Equals(""))
		{
			return "(" + text + ") ";
		}
		return text;
	}
    private static void LoadListData_File(string Path, List<string> text)
    {
        string[] InPath = File.ReadAllText(Path).Split(',');
        for (int i = 0; i < InPath.Length; i++)
        {
            text.Add(InPath[i]);
        }
    }
    private static void WriteListData_File(string Path, List<string> text)
    {
        string includeFile = "";
        File.AppendAllText(Path, string.Empty); // Clear the file before writing
        for (int i = 0; i < text.Count; i++)
        {
            includeFile = includeFile + text[i] + ",";
        }
        File.WriteAllText(Path, includeFile);
    }
    public static void LoadData()
	{
        try
        {
            LoadListData_File("Data\\Boss.ini", ListNameBossAttack);
			isHuntingBoss = Rms.loadRMSInt("sanboss") == 1;
			isPaintBackground = Rms.loadRMSInt("isPaintBgr") == 1;
			isShowCharsInMap = Rms.loadRMSInt("showchar") == 1;
			//isReduceGraphics = Rms.loadRMSInt("IsReduceGraphics") == 1;
			isReduceGraphics1 = Rms.loadRMSInt("IsReduceGraphics1") == 1;		
 
			APIKey = File.ReadAllText("Data\\keyAPI.ini");
			APIServer = File.ReadAllText("Data\\serverAPI.ini");
		}
		catch
		{
		}
        //      if (mGraphics.zoomLevel == 1)
        //{
        //	try
        //	{
        //		logoGameScreen = Image.__createImage(Convert.FromBase64String(LogoMod.logoX1Base64));
        //	}
        //	catch
        //	{
        //	}
        //}
        //else if (mGraphics.zoomLevel == 2)
        //{
        //	try
        //	{
        //              logoGameScreen = Image.__createImage(Convert.FromBase64String(LogoMod.logoX2Base64));
        //	}
        //	catch
        //	{
        //	}
        //}
        //else
        //{
        //	try
        //	{
        //              logoGameScreen = Image.__createImage(Convert.FromBase64String(LogoMod.logoX1Base64));
        //	}
        //	catch
        //	{
        //	}
        //}
        new Thread(new ThreadStart(Login)).Start();

        //GetLoginDataFromAccountManager();

    }

	public static void LoadFlagColor()
	{
		listFlagColor.Add(SKColors.Black);
		listFlagColor.Add(new SKColor(0, (int)(0.99609375f*255), (int)(0.99609375f*255)));
		listFlagColor.Add(SKColors.Red);
		listFlagColor.Add(new SKColor((int)(0.54296875f), 0, (int)(0.54296875f*255)));
		listFlagColor.Add(SKColors.Yellow);
		listFlagColor.Add(SKColors.Green);
		listFlagColor.Add(new SKColor((int)(0.99609375f*255), (int)(0.51171875f*255), (int)(125f*255/ 128f)));
		listFlagColor.Add(new SKColor((int)(0.80078125f*255), (int)(51f*255 / 128f), 0));
		listFlagColor.Add(SKColors.Black);
		listFlagColor.Add(SKColors.Blue);
		listFlagColor.Add(SKColors.Red);
		listFlagColor.Add(SKColors.Blue);
	}

	public static void RefreshListCharsInMap()
	{
		if (listCharsInMap.Count <= 2)
		{
			return;
		}
		List<Char> list = new List<Char>();
		while (listCharsInMap.Count != 0)
		{
			Char @char = listCharsInMap[0];
			list.Add(@char);
			string nameWithoutClanTag = @char.GetNameWithoutClanTag();
			listCharsInMap.RemoveAt(0);
			for (int i = 0; i < listCharsInMap.Count; i++)
			{
				Char char2 = listCharsInMap[i];
				if (nameWithoutClanTag == char2.GetNameWithoutClanTag())
				{
					list.Add(char2);
					listCharsInMap.RemoveAt(i);
                    i--;
				}
			}
		}
		listCharsInMap.Clear();
		listCharsInMap = list;
	}

	public static void ConnectToAccountManager()
	{
		if (mSystem.currentTimeMillis() - lastTimeConnected < 100L || GetMessServer == "" || GetMessServer == string.Empty)
		{
			return;
		}
		lastTimeConnected = mSystem.currentTimeMillis();
		string[] array = GetMessServer.Split('|');
		if (array[0] == "0")
		{
			try
			{
				new Thread((ThreadStart)delegate
				{
					UseSkill(int.Parse(array[1]));
				}).Start();
			}
			catch
			{
			}
			GetMessServer = "";
		}
		else if (array[0] == "1")
		{
			if (TileMap.zoneID != int.Parse(array[1]) && !global::Char.ischangingMap && !Controller.isStopReadMessage)
            {
				Service.gI().requestChangeZone(int.Parse(array[1]), -1);
			}
			GetMessServer = "";
		}
		else if (array[0] == "2")
		{
			if (Char.myCharz().charID != int.Parse(array[1]) && GameScr.findCharInMap(int.Parse(array[1])) == null)
			{
				try
				{
					AutoMap.TeleVip(int.Parse(array[1]));
                    GetMessServer = "";
                }
				catch (Exception ex)
                {
                    GameScr.info1.addInfo($"Lỗi control[{array[0]}]:" + ex.Message, 0);
                    GetMessServer = "";
                }
			}
		}
		else if (array[0] == "3")
		{
			if (array[1] == "0" && Char.myCharz().meDead)
			{
				Service.gI().wakeUpFromDead();
			}
			if (array[1] == "1" && Char.myCharz().meDead)
			{
				Service.gI().returnTownFromDead();
			}
			if (array[1].StartsWith("2") && !Gohome)
			{
				Gohome = true;
				new Thread(new ThreadStart(MainMod.GoBackMoveSave)).Start();
			}
			GetMessServer = "";
		}
		else if (array[0] == "4")
		{
			if (!isHoiskill)
			{
				isHoiskill = true;
				new Thread(new ThreadStart(Hoiskill)).Start();
			}
			GetMessServer = "";
		}
		else if (array[0] == "5")
		{
			MainMod.mapIdNRD = int.Parse(array[1]);
			MainMod.zoneIdNRD = int.Parse(array[2]);
			MainMod.isAutoEnterNRDMap = true;
			isOpenMenuNPC = isAutoEnterNRDMap;
            new Thread(new ThreadStart(EnterNRDMap)).Start();
            GetMessServer = "";
		}
		else if (array[0] == "6")
		{
            try
            {
                if (array[1] == "0")
				{
					if(UpdatekeywithControl(int.Parse(array[2])))
					{
						GetMessServer = "";
					}
				}
				if (array[1] == "1")
				{
					Chat(array[2].ToLower());
					GetMessServer = "";
				}
			}
            catch (Exception ex)
            {
                GameScr.info1.addInfo($"Lỗi control[{array[0]}]:" + ex.Message, 0);
                GetMessServer = "";
            }
        }
		else if (array[0] == "7")
		{
            if (TileMap.mapID != int.Parse(array[1]))
            {
                AutoMap.StartRunToMapId(int.Parse(array[1]));
            }
            GetMessServer = "";
        }
		else if (array[0] == "8")
		{
			try
			{
				FocusOn();
			}
            catch (Exception ex)
            {
                GameScr.info1.addInfo($"Lỗi control[{array[0]}]:" + ex.Message, 0);
                GetMessServer = "";
            }
            GetMessServer = "";
        }
		else if (array[0] == "9")
		{
			if (TileMap.mapID != int.Parse(array[1]))
			{
				AutoMap.StartRunToMapId(int.Parse(array[1]));
			}
			GetMessServer = "";
		}
		else if (array[0] == "10")
		{
			try
			{
				UseItem(int.Parse(array[1]));
                GetMessServer = "";
            }
            catch (Exception ex)
            {
                GameScr.info1.addInfo($"Lỗi control[{array[0]}]:" + ex.Message, 0);
                GetMessServer = "";
            }
        }
		else if (array[0] == "11")
		{
			if (array[1] == "0")
			{
				if (Char.myCharz().cFlag != 0)
				{
					Service.gI().getFlag(1, 0);
				}
			}
			else
			{
				if (Char.myCharz().cFlag != 8)
				{
					Service.gI().getFlag(1, 8);
				}
			}
			GetMessServer = "";
		}
		else if (array[0] == "12")
		{
			limitFPS = int.Parse(array[1]);
			GetMessServer = "";
		}
		else if (array[0] == "13")
		{
			isConnectToAccountManager = false;
			GetMessServer = "";
		}
		else if (array[0] == "14")
		{
			try
			{
				optionxhp = int.Parse(array[1]);
                Xhp();
            }
			catch (Exception ex)
            {
                optionxhp = 0;
                GameScr.info1.addInfo($"Lỗi control[{array[0]}]:" + ex.Message, 0);
                GetMessServer = "";           
			}
		}
		else if (array[0] == "15")
		{
			AutoTrain.chongmkn = !AutoTrain.chongmkn;
			GetMessServer = "";
		}
		else
		{
			GetMessServer = "";
		}
    }

	public static bool isColdImmune(Item item)
	{
		int id = item.template.id;
		if (id != 450 && id != 630 && id != 631 && id != 632 && id != 878 && id != 879)
		{
			if (id >= 386)
			{
				return id <= 394;
			}
			return false;
		}
		return true;
	}

	public static void UseCapsule()
	{
		isUsingCapsule = true;
		for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = Char.myCharz().arrItemBag[i];
			if (item != null && (item.template.id == 194 || item.template.id == 193))
			{
				Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
				break;
			}
		}
		Thread.Sleep(500);
		Service.gI().requestMapSelect(0);
		Thread.Sleep(1000);
		isUsingCapsule = false;
	}

	public static void UseSkill(int skillIndex)
	{
		if (!isUsingSkill)
		{
			isUsingSkill = true;
			if (GameScr.keySkill[skillIndex] != null)
			{
				if (Char.myCharz().myskill != GameScr.keySkill[skillIndex])
				{
					GameScr.gI().doSelectSkill(GameScr.keySkill[skillIndex], isShortcut: true);
					Thread.Sleep(200);
					GameScr.gI().doSelectSkill(GameScr.keySkill[skillIndex], isShortcut: true);
					isUsingSkill = false;
				}
				else
				{
					GameScr.gI().doSelectSkill(GameScr.keySkill[skillIndex], isShortcut: true);
					isUsingSkill = false;
				}
			}
		}
	}

    public static string DecryptString(string base64CipherText)
    {
        byte[] array = Convert.FromBase64String(base64CipherText);
        byte[] key = f_huynguyen721221;
        string @string;
        using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
        {
            tripleDESCryptoServiceProvider.Key = key;
            tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
            tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
            byte[] bytes = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
            @string = Encoding.UTF8.GetString(bytes);
        }
        return @string;
    }
	//public static void GetLoginDataFromAccountManager()
	//{
	//	try
	//	{
	//		string path = Environment.GetCommandLineArgs()[1];
	//		string[] array = path.Split('|');
	//		account = array[1];
	//		server = int.Parse(array[2]);
	//		password = DecryptString(array[3]);
	//		string text4 = File.ReadAllText("Data\\proxy.txt");
	//		if (!string.IsNullOrEmpty(text4))
	//		{
	//			string[] array2 = text4.Split(new char[]
	//			{
	//			':'
	//			});
	//			ProxyClient.addressproxy = array2[0];
	//			ProxyClient.postproxy = int.Parse(array2[1]);
	//			ProxyClient.accountproxy = array2[2];
	//			ProxyClient.passproxy = array2[3];
	//		}
	//		//new Thread(new ThreadStart(SaveCoolDownNew)).Start();
	//	}
	//	catch (Exception ex)
	//	{
	//		GameCanvas.startOKDlg(ex.Message);
	//		account = "";
	//	}
	//}
	public static void Login()
	{
		Thread.Sleep(1000);
		while (!Program._isExiting)
		{
			try
			{
                while (!ServerListScreen.loadScreen)
                {
                    Thread.Sleep(10);
                }
                while (LoginScr.timeLogin > 0)
                {
					Thread.Sleep(1000);
                    GameCanvas.startOKDlg(string.Format("Đăng nhập lại trong {0}s", LoginScr.timeLogin));
                    Thread.Sleep(LoginScr.timeLogin*1000);
					LoginScr.timeLogin = 0;
                }
				if (GameCanvas.currentScreen == GameCanvas.serverScreen && ServerListScreen.testConnect != 0)
				{
					if (GameCanvas.currentScreen == GameCanvas.loginScr) return;
                    Rms.saveRMSString("acc", "Ngọc rồng online");
					Rms.saveRMSString("pass", "nhincaidaubuoi");
                    GameCanvas.startOKDlg(string.Format("Đăng nhập sau {0}s", longDelayLogin));
                    Thread.Sleep(longDelayLogin * 1000);
                    if (Rms.loadRMSInt(ServerListScreen.RMS_svselect) != server - 1)
					{
						//Rms.saveRMSInt(ServerListScreen.RMS_svselect, server - 1);
						//ServerListScreen.ipSelect = server - 1;
						Session_ME.gI().close();
                        ServerListScreen.SetIpSelect(server-1, issave: false);

                        if (GameCanvas.serverScreen == null)
						{
							GameCanvas.serverScreen = new ServerListScreen();
						}
						GameCanvas.serverScreen.selectServer();
						GameCanvas.serverScreen.switchToMe();
					}
					if (!Session_ME.connected || Session_ME.connecting) continue;
                    while (!ServerListScreen.loadScreen)
					{
						Thread.Sleep(10);
					}
					while (!Session_ME.gI().isConnected())
					{
						Thread.Sleep(100);
					}
                    while (ServerListScreen.testConnect != 2) Thread.Sleep(1000);
                    if (GameCanvas.loginScr == null)
					{
						GameCanvas.loginScr = new LoginScr();
					}
					GameCanvas.loginScr.switchToMe();
                    GameCanvas.loginScr.doLogin();
					WaitLogin = true;
                    AutoSkill.isLoadKeySkill = true;
					Thread.Sleep(5000);
                }
            }
			catch
			{
			}
			Thread.Sleep(2000);
		}
	}

	public static bool isMeHasEnoughMP(Skill skillToUse)
	{
		if (skillToUse.template.manaUseType == 2)
		{
			return true;
		}
		if (skillToUse.template.manaUseType != 1)
		{
			return Char.myCharz().cMP >= skillToUse.manaUse;
		}
		return Char.myCharz().cMP >= skillToUse.manaUse * Char.myCharz().cMPFull / 100;
	}

    public static void statusChar(mGraphics gclass30_0, Char gclass131_0)
	{
		mFont.tahoma_7b_red.drawString(gclass30_0, gclass131_0.cName + " [" + NinjaUtil.getMoneys(gclass131_0.cHP) + "/" + NinjaUtil.getMoneys(gclass131_0.cHPFull) + "]", GameCanvas.w / 2, 62, 2);
		mFont.tahoma_7b_red.drawString(gclass30_0, string.Concat(string.Concat(new object[2] { "ClanID:", gclass131_0.clanID })), GameCanvas.w / 2, 72, 2);
		int num = 82;
		int num2 = 10;
		if (gclass131_0.isNRD)
		{
			mFont.tahoma_7_red.drawString(gclass30_0, "Còn: " + gclass131_0.timeNRD + "s", GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass131_0.protectEff)
		{
			mFont.tahoma_7_red.drawString(gclass30_0, "khiên: " + gclass131_0.timeProtect + "s", GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass131_0.isMonkey == 1)
		{
			mFont.tahoma_7_red.drawString(gclass30_0, "khỉ: " + gclass131_0.timeMonkey + "s", GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass131_0.timeHuytSao > 0)
		{
			mFont.tahoma_7_red.drawString(gclass30_0, "huýt sáo: " + gclass131_0.timeHuytSao + "s", GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass131_0.sleepEff)
		{
			mFont.tahoma_7_red.drawString(gclass30_0, "thôi miên: " + gclass131_0.timeSleep + "s", GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass131_0.blindEff && !gclass131_0.Equals(null))
		{
			mFont.tahoma_7_red.drawString(gclass30_0, "choáng: " + gclass131_0.timeBlind + "s", GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass131_0.isFreez)
		{
			mFont.tahoma_7_red.drawString(gclass30_0, "TDHS: " + gclass131_0.freezSeconds + "s", GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass131_0.holdEffID != 0)
		{
			mFont.tahoma_7b_red.drawString(gclass30_0, "Bị trói", GameCanvas.w / 2, num, 2);
			num += num2;
		}
	}

	public static void teleVFocus()
	{
		if (Char.myCharz().charFocus != null)
		{
			TeleportTo(Char.myCharz().charFocus.cx, Char.myCharz().charFocus.cy);
		}
		else if (Char.myCharz().itemFocus != null)
		{
			TeleportTo(Char.myCharz().itemFocus.x, Char.myCharz().itemFocus.y);
		}
		else if (Char.myCharz().mobFocus != null)
		{
			TeleportTo(Char.myCharz().mobFocus.x, Char.myCharz().mobFocus.y);
		}
		else if (Char.myCharz().npcFocus != null)
		{
			TeleportTo(Char.myCharz().npcFocus.cx, Char.myCharz().npcFocus.cy - 3);
		}
		else
		{
			GameScr.info1.addInfo("Không Có Mục Tiêu!", 0);
		}
	}

	public static void PaintAction(mGraphics gclass30_0, int int_20, int int_21)
	{
		int num = int_21;
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			Char @char = (Char)GameScr.vCharInMap.elementAt(i);
			if (@char.charHold != null)
			{
				string s_Hold = string.Format("{0} đang trói: {1}", @char.cName, (@char.charHold.charID == Char.myCharz().charID) ? "bạn" : @char.charHold.cName);
                mFont.tahoma_7_yellow.drawString(gclass30_0, s_Hold, int_20, num, 0);
				num += 10;
			}
            if (@char.isStandAndCharge)
            {
                // 1. Xử lý logic chung cho cả 2 giới tính [cite: 2026-04-06]
                bool isInRange = Math.abs(Char.myCharz().cx - @char.cx) <= 900;
                string areaStatus = isInRange ? "-Trong tầm" : "-Ngoài tầm";
                mFont areaFont = isInRange ? mFont.tahoma_7b_red : mFont.tahoma_7_yellow;

                string flagText = " ";
                if (@char.cFlag != 0)
                {
                    flagText = (@char.cFlag == Char.myCharz().cFlag) ? " (cùng cờ) " : " (khác cờ) ";
                }

                // 2. Vẽ dựa trên giới tính [cite: 2026-04-10]
                if (@char.cgender == 2)
                {
                    string hpInfo = $"[{MainMod.convertHP(@char.cHPFull)}]";
                    DrawSegments(gclass30_0, int_20, num,
                        ($"{@char.cName} đang bom ", mFont.tahoma_7b_yellow),
                        (flagText, mFont.tahoma_7b_red),
                        (hpInfo + areaStatus, areaFont)
                    );
                    num += 10;
                }
                else if (@char.cgender == 1)
                {
                    DrawSegments(gclass30_0, int_20, num,
                        ($"{@char.cName} đang nạp laser", mFont.tahoma_7b_yellow),
                        (flagText, mFont.tahoma_7b_red),
                        (areaStatus, areaFont)
                    );
                    num += 10;
                }
            }
            //if (@char.isStandAndCharge)
            //{
            //	if (@char.cgender == 2)
            //	{
            //		string s_StandAndCharge = string.Concat(@char.cName, " đang bom ");
            //		string s_sFlag = (@char.cFlag != 0) ? ((@char.cFlag == Char.myCharz().cFlag) ? " (cùng cờ) " : " (khác cờ) ") : " ";
            //		string s_Area = string.Format("[{0}]{1}", convertHP(@char.cHPFull), (Math.abs(Char.myCharz().cx - @char.cx) <= 900) ? "-Trong tầm" : "-Ngoài tầm");
            //		int gW = mFont.tahoma_7_yellow.getWidth(s_StandAndCharge);
            //		int gWFlag = mFont.tahoma_7b_red.getWidth(s_sFlag);
            //		mFont.tahoma_7b_yellow.drawString(gclass30_0, s_StandAndCharge, int_20, num, 0);
            //		mFont.tahoma_7b_red.drawString(gclass30_0, s_sFlag, int_20 + gW, num, 0);
            //		(((Math.abs(Char.myCharz().cx - @char.cx) <= 900)) ? mFont.tahoma_7b_red : mFont.tahoma_7_yellow).drawString(gclass30_0, s_Area, int_20 + gWFlag + gW, num, 0);
            //		num += 10;
            //	}
            //	if (@char.cgender == 1)
            //	{
            //		string s_StandAndCharge = string.Concat(@char.cName, " đang nạp lazer");
            //		string s_sFlag = (@char.cFlag != 0) ? ((@char.cFlag == Char.myCharz().cFlag) ? " (cùng cờ) " : " (khác cờ) ") : " ";
            //		string s_Area = (Math.abs(Char.myCharz().cx - @char.cx) <= 900) ? "-Trong tầm" : "-Ngoài tầm";
            //		int gW = mFont.tahoma_7_yellow.getWidth(s_StandAndCharge);
            //		int gWFlag = mFont.tahoma_7b_red.getWidth(s_sFlag);
            //		mFont mFontArea = ((Math.abs(Char.myCharz().cx - @char.cx) <= 900)) ? mFont.tahoma_7b_red : mFont.tahoma_7b_yellow;
            //		mFont.tahoma_7b_yellow.drawString(gclass30_0, s_StandAndCharge, int_20, num, 0);
            //		mFont.tahoma_7b_red.drawString(gclass30_0, s_sFlag, int_20 + gW, num, 0);
            //		mFontArea.drawString(gclass30_0, s_Area, int_20 + gWFlag + gW, num, 0);
            //		num += 10;
            //	}
            //}
        }
	}

	public static void PaintFocusMob(mGraphics gclass30_0, Mob gclass145_0)
	{
		((gclass145_0.isBigBoss()) ? mFont.tahoma_7b_red : mFont.tahoma_7b_yellow).drawString(gclass30_0, gclass145_0.getTemplate().name + " [" + NinjaUtil.getMoneys(gclass145_0.hp) + "/" + NinjaUtil.getMoneys(gclass145_0.maxHp) + "]", GameCanvas.w / 2, 62, 2);
		int num = 72;
		int num2 = 10;
		if (gclass145_0.sleepEff)
		{
			mFont.tahoma_7b_red.drawString(gclass30_0, "thôi miên", GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass145_0.blindEff)
		{
			mFont.tahoma_7b_red.drawString(gclass30_0, "choáng", GameCanvas.w / 2, num, 2);
            num += num2;
		}
		if (gclass145_0.isFreez)
		{
			mFont.tahoma_7b_red.drawString(gclass30_0, "TDHS :" + gclass145_0.seconds, GameCanvas.w / 2, num, 2);
			num += num2;
		}
		if (gclass145_0.holdEffID != 0)
		{
			mFont.tahoma_7b_red.drawString(gclass30_0, "trói", GameCanvas.w / 2, num, 2);
			num += num2;
		}
	}

	public static void Xhp()
	{
		if (TileMap.mapID <= 85 && TileMap.mapID >= 91)
		{
			GameScr.info1.addInfo("Vui Lòng Vào Map Ngọc Rồng Đen!", 0);
			return;
		}
		if (Char.myCharz().cFlag != 8)
		{
			GameScr.info1.addInfo("Vui Lòng Nhặt Ngọc Rồng Đen!", 0);
			return;
		}
		bool flag = false;
		if (Char.myCharz().bag >= 0 && ClanImage.idImages.containsKey(Char.myCharz().bag + string.Empty))
		{
			ClanImage clanImage = (ClanImage)ClanImage.idImages.get(Char.myCharz().bag + string.Empty);
			if (clanImage.idImage != null)
			{
				for (int i = 0; i < clanImage.idImage.Length; i++)
				{
					if (clanImage.idImage[i] == 2322)
					{
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag)
		{
			GameScr.info1.addInfo("Vui Lòng Nhặt Ngọc Rồng Đen!", 0);
			return;
		}
		bool flag2 = true;
        bool flag3 = true;
        Npc npc = (Npc)GameScr.vNpc.elementAt(0);
		for (int j = 0; j < GameScr.vNpc.size(); j++)
		{
			npc = (Npc)GameScr.vNpc.elementAt(j);
			if (npc.template.npcTemplateId > 30 && npc.template.npcTemplateId < 36)
			{
				if (Math.abs(Char.myCharz().cx - npc.cx) > 60)
				{
					flag3 = false;
				}
				if (Char.myCharz().holdEffID != 0 || Char.myCharz().sleepEff || Char.myCharz().blindEff || Char.myCharz().isFreez)
				{
					flag2 = false;
				}
				if (!flag2)
				{
					GameScr.info1.addInfo("Đang Bị Không Chế, Không Thể Bay Đi Xhp", 0);
					return;
				}

				long cHPFull = Char.myCharz().cHPFull;
				short num = 35;
				if (TileMap.mapID == 90)
				{
					num = 34;
				}
				if (TileMap.mapID == 89)
				{
					num = 33;
				}
				if (TileMap.mapID == 88)
				{
					num = 32;
				}
				if (TileMap.mapID == 87)
				{
					num = 31;
				}
				if (TileMap.mapID == 86)
				{
					num = 30;
				}
				if (TileMap.mapID == 85)
				{
					num = 36;
				}
                if (!flag3)
                {
                    TeleportTo(GameScr.findNPCInMap(num).cx, GameScr.findNPCInMap(num).cy - 3);
                }
				Thread.Sleep(200);
                Service.gI().openMenu(num);
				Service.gI().menu(num, 0, 2);
				Service.gI().confirmMenu(num, 2);
				GameScr.info1.addInfo((cHPFull < Char.myCharz().cHPFull) ? "xhp xong!" : "xhp xịt!", 0);
				if (optionxhp == 1)
				{
					if (!global::Char.ischangingMap && !Controller.isStopReadMessage)
					{
						AutoPick.isAutoPick = false;
						Service.gI().requestChangeZone(TileMap.zoneID, -1);
					}
				}
				break;
			}
		}
	}

	//public static bool BoolBoss()
	//{
	//	for (int i = 0; i < GameScr.vItemMap.size(); i++)
	//	{
	//		Item item = (Item)GameScr.vItemMap.elementAt(i);
	//		if (item.template.id == 1220 || item.template.id == 1221 || item.template.id == 1222)
	//		{
	//			return true;
	//		}
	//	}
	//	return false;
	//}

	public static void GoBackMoveSave()
	{
		Gohome = true;
		for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = Char.myCharz().arrItemBag[i];
			if (item != null && (item.template.id == 194 || item.template.id == 193))
			{
				Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
				Thread.Sleep(200);
				Service.gI().requestMapSelect(0);
                Thread.Sleep(1500);
                if (isMeInNRDMap())
                {
                    for (int j = 0; j < GameScr.vCharInMap.size(); j++)
                    {
                        Char @char = (Char)GameScr.vCharInMap.elementAt(j);
                        if (@char.isNRD && !Char.myCharz().isLockMove)
                        {
                            TeleportTo(@char.cx, @char.cy - 3);
                            break;
                        }
                    }
                }
                Thread.Sleep(5000);
				Gohome = false;
				break;
			}
		}
	}

    public static sbyte IDSelect(string StartWith, string thenWith)
	{
		for (int i = 0; i < GameCanvas.menu.menuItems.size(); i++)
		{
			Command command = (Command)GameCanvas.menu.menuItems.elementAt(i);
			if (command.caption.Contains(StartWith) && command.caption.Contains(thenWith))
			{
				return (sbyte)i;
			}
		}
        return -1;
    }

    public static void Hoiskill()
	{
		int mapID = TileMap.mapID;
		if (TileMap.mapID != 14 && TileMap.mapID != 44 && TileMap.mapID != 52 && TileMap.mapID != 129)
		{
			AutoMap.StartRunToMapId(129);
		}
		while (TileMap.mapID != 129)
		{
			Thread.Sleep(500);
		}
		Service.gI().openMenu(23);
		Service.gI().confirmMenu(23, 2);
		if (smethod_6(1))
		{
			int cy = Char.myCharz().cy;
			while (cy == Char.myCharz().cy)
			{
				Thread.Sleep(500);
			}
			Thread.Sleep(10000);
			while (Char.myCharz().charFocus == null)
			{
				Thread.Sleep(500);
			}
		}
		if (TileMap.mapID == 129)
		{
			if (Char.myCharz().cy < 360)
			{
                isHoiskill = false;
                if (AutoSkill.isAutoSendAttack) AutoSkill.isAutoSendAttack = false;
                Thread.Sleep(5000);
				while (Char.myCharz().isLockMove || Char.myCharz().isLockAttack)
				{
					Thread.Sleep(500);
				}
				Service.gI().openMenu(23);
				Service.gI().confirmMenu(23, (sbyte)(GameCanvas.menu.menuItems.size() - 1));
			}
		}
	}



	public static void BuffHuytSao()
	{		
        bool isHaveHuytSao = Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[5]).paintCanNotUseSkill;
        for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			Char @char = (Char)GameScr.vCharInMap.elementAt(i);
			if(Char.myCharz().isLockMove)
			{
				return;
			}
			if(Char.myCharz().clan == null)
			{
                GameScr.info1.addInfo("Không có bang hội! Ấn (phím Z) để tắt auto sáo!", 0);
				return;
            }
			if(Char.myCharz().cgender != 2)
			{
				GameScr.info1.addInfo("Không tìm thấy skill huýt sáo!", 0);
				AutoBuffHuytSao = false;
                return;
			}

			if (isHaveHuytSao)
			{
				if(!isHoiskill)
				{
					isHoiskill = true;
					new Thread(new ThreadStart(Hoiskill)).Start();
				}
				return;
			}
            if (@char.isStandAndCharge && @char.cgender == 2 && !CoverSao)
            {
                if (ItemTime.isExistItem(3781))
                {
                    return;
                }
                CoverSao = true;
            }
            if (@char.isNRD && Char.myCharz().cFlag == 8 && isMeInNRDMap())
			{
                if (Math.abs(Char.myCharz().cx - @char.cx) > 200 && GameCanvas.gameTick % 100 == 0)
                {
                    TeleportTo(@char.cx, @char.cy);
                }
                if (CoverSao && @char.cgender == 2 && @char.charID != Char.myCharz().charID)
                {
                    GameScr.gI().doUseSkillNotFocus(Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[5]));
                    CoverSao = false;
                }
            }
		}
	}

    public static void TelePha()
	{
		if (!isTele && !isGSM)
		{
			return;
		}
        if (Char.myCharz().meDead && GameCanvas.gameTick % 50 == 0)
		{
			Service.gI().returnTownFromDead();
		}
        if (isGSM)
        {
            if (GameScr.findCharInMap(IDtelegsm) == null && mSystem.currentTimeMillis() - Longtele >= 1000)
            {
                Longtele = mSystem.currentTimeMillis();
                AutoMap.TeleVip(IDtelegsm);
                GameScr.info1.addInfo("Đang tele id chỉ định", 0);
            }
            return;
        }
        if ( GameScr.findCharInMap(IDtelePha) == null && mSystem.currentTimeMillis() - Longtele >= 1000)
		{
			Longtele = mSystem.currentTimeMillis();
			AutoMap.TeleVip(IDtelePha);
			GameScr.info1.addInfo("Đang tele id chỉ định", 0);
		}
		if (GameScr.findCharInMap(IDtelePha) != null && GameCanvas.gameTick % 200 == 0)
		{
			TeleportTo(GameScr.findCharInMap(IDtelePha).cx, GameScr.findCharInMap(IDtelePha).cy);
		}
	}

	//public static int QuantityItemBag(int int_20)
	//{
	//	for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
	//	{
	//		Item item = Char.myCharz().arrItemBag[i];
	//		if (item != null && item.template.id == int_20)
	//		{
	//			return item.quantity;
	//		}
	//	}
	//	return 0;
	//}

	public static bool FindItemBag(int int_20)
	{
		for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = Char.myCharz().arrItemBag[i];
			if (item != null && item.template.id == int_20)
			{
				return true;
			}
		}
		return false;
	}

	public static void PaintItemMap(mGraphics graphics)
	{
		for (int i = 0; i < GameScr.vItemMap.size(); i++)
		{
			ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
			int num = 0;
			if (itemMap != null && itemMap.template.type < 5)
			{
				if (Char.myCharz().itemFocus == itemMap)
				{
					mFont.tahoma_7b_red.drawString(graphics,"Item map: " + itemMap.template.name, 20, GameCanvas.h/3+ num, 0);
					num -= 10;
					break;
				}
				mFont.tahoma_7_red.drawString(graphics, "Item map: " + itemMap.template.name , 20, GameCanvas.h/3+num, 0);
				num -= 10;
			}
		}
	}

	public static void AutoKameBoss()
	{
		if (autokm)
		{
			GameScr.info1.addInfo("Đang auto!", 0);
			return;
		}
		autokm = true;
		if (Char.myCharz().charFocus == null)
		{
			GameScr.info1.addInfo("Focus Boss rồi sử dụng", 0);
			autokm = false;
			return;
		}
		while (Char.myCharz().charFocus != null && Char.myCharz().charFocus.cHP > Hpcombo)
		{
			Thread.Sleep(100);
			if (Char.myCharz().charFocus == null)
			{
				autokm = false;
				return;
			}
		}
		GameScr.gI().doSelectSkill(Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[5]), isShortcut: true);
		GameScr.gI().doSelectSkill(Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[5]), isShortcut: true);
		int num = 0;
		//while (!Char.myCharz().charFocus.blindEff)
		//{
		//	Thread.Sleep(500);
		//	num += 10;
		//	if (num > 3000)
		//	{
		//		GameScr.info1.addInfo("Lỗi Dịch chuyển tức thời!", 0);
		//		autokm = false;
		//		return;
		//	}
		//}
		if (Char.myCharz().charFocus.cx < 100)
		{
			Char.myCharz().cx = Char.myCharz().charFocus.cx + 5;
		}
		if (Char.myCharz().charFocus.cx > TileMap.pxw - 120)
		{
			Char.myCharz().cx = Char.myCharz().charFocus.cx - 5;
		}
		while (!Char.myCharz().charFocus.blindEff)
		{
			return;
		}
		GameScr.gI().doSelectSkill(Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[1]), isShortcut: true);
		if (Char.myCharz().charFocus != null)
		{
			try
			{
				MyVector myVector = new MyVector();
				myVector.addElement(Char.myCharz().charFocus);
				Service.gI().sendPlayerAttack(new MyVector(), myVector, 2);
				Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
			}
			catch
			{
			}
		}
		autokm = false;
	}

    public static void ShowMenuGDL()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command("Giảm Đồ\nHọa \n" + (isReduceGraphics1 ? "[ON] " : "[OFF]"), getInstance(), 27, null));
		myVector.addElement(new Command("Thay Đổi \nFPS", getInstance(), 31, null));
		GameCanvas.menu.startAt(myVector, 3);
	}

	public static void ShowMenupotara()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command("Hợp Thể Khi HP \n[" + minumumHPPercentFusionDance + "%]" + ((minumumHPPercentFusionDance == 0) ? "" : ("\n[" + Res.formatNumber2(Char.myCharz().cHPFull * minumumHPPercentFusionDance / 100) + "]")), getInstance(), 9, null));
		myVector.addElement(new Command("Hợp Thể Khi HP \n[" + Res.formatNumber2(minimumHPFusionDance) + "]", getInstance(), 10, null));
		GameCanvas.menu.startAt(myVector, 3);
	}

    public static void AutoLuyenSkill9L1()
    {
        bool isStartUse = Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[7]).paintCanNotUseSkill;
        if (Start)
        {
            if (!isStartUse)
			{
                if (Char.myCharz().cMP < Char.myCharz().cMPFull * 80 / 100 && mSystem.currentTimeMillis() - hopthe > 2000L)
                {
                    hopthe = mSystem.currentTimeMillis();
                    UsePorata();
                    Service.gI().petStatus(3);
                }
                if (Char.myCharz().cMP >= Char.myCharz().cMPFull * 80 / 100 && Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[7]) != null && !isStartUse && mSystem.currentTimeMillis() - useSkill8 > 2000L)
                {
                    useSkill8 = mSystem.currentTimeMillis();
                    GameScr.gI().doSelectSkill(Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[7]), isShortcut: true);
                }
            }
        }
    }

    public static void AutoLuyenSkill9L2()
	{
		bool isStartUse = Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[7]).paintCanNotUseSkill;
        if (Start)
		{
			if (!isStartUse)
			{
				if (TileMap.mapID != 129)
				{
					if (TileMap.mapID != 44)
					{
						AutoMap.StartRunToMapId(44);
						return;
					}
					if (Char.myCharz().cMP < Char.myCharz().cMPFull * 80 / 100 && mSystem.currentTimeMillis() - hopthe > 2000L)
					{
						hopthe = mSystem.currentTimeMillis();
						UsePorata();
						Service.gI().petStatus(3);
					}
					if (Char.myCharz().cMP >= Char.myCharz().cMPFull * 80 / 100 && Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[7]) != null && !isStartUse && mSystem.currentTimeMillis() - useSkill8 > 2000L)
					{
						useSkill8 = mSystem.currentTimeMillis();
						GameScr.gI().doSelectSkill(Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[7]), isShortcut: true);
					}
				}
				else
				{
					Controller.isDisconnected = true;
				}
			}
			else
			{
				if(TileMap.mapID != 129)
				{
					AutoMap.StartRunToMapId(129);
					return;
				}
                if (global::Char.myCharz().cy == 408 && mSystem.currentTimeMillis() - longWaitLS > 6000L)
                {
                    longWaitLS = mSystem.currentTimeMillis();
                    Service.gI().openMenu(23);
                }
            }
		}
	}

	public static void UsePorata()
	{
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

    //public static void gbBom()
    //{
    //    MainMod.Gohome = true;
    //    for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
    //    {
    //        Item item = global::Char.myCharz().arrItemBag[i];
    //        if (item != null && (item.template.id == 194 || item.template.id == 193))f
    //        {
    //            Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
    //            Thread.Sleep(200);
    //            Service.gI().requestMapSelect(0);
    //            Thread.Sleep(1500);
    //            if (isMeInNRDMap())
				//{
				//	UpdateControlChat("s");
				//}
    //            Thread.Sleep(1500);
    //            MainMod.UseSkill(4);
    //            Thread.Sleep(5000);
    //            MainMod.Gohome = false;
    //            return;
    //        }
    //    }
    //}

	public static void FixBlackScreen()
	{
        Controller.gI().loadCurrMap(0);
        Service.gI().finishLoadMap();
        global::Char.isLoadingMap = false;
    }

    //GameScr.updateKeyTouchControl()
    public static void UpdateKeyTouch()
    {
        if (!MainMod.isMeInNRDMap())
        {
            //if (MainMod.isHuntingBoss)
            //{
            //    for (int i = 0; i < MainMod.listBosses.Count; i++)
            //    {
            //        if (!GameCanvas.isPointerHoldIn(GameCanvas.w - 23, 42 + 10 * i, 23, 9))
            //        {
            //            continue;
            //        }
            //        GameCanvas.isPointerJustDown = false;
            //        GameScr.gI().isPointerDowning = false;
            //        if (!GameCanvas.isPointerClick || !GameCanvas.isPointerJustRelease)
            //        {
            //            continue;
            //        }
            //        if (!AutoMap.isXmaping)
            //        {
            //            for (int j = 0; j < TileMap.mapNames.Length; j++)
            //            {
            //                if (TileMap.mapNames[j] == MainMod.listBosses[i].MapName)
            //                {
            //                    AutoMap.StartRunToMapId(j);
            //                    break;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            AutoMap.FinishXmap();
            //        }
            //        Char.myCharz().currentMovePoint = null;
            //        GameCanvas.clearAllPointerEvent();
            //        return;
            //    }
            //}
            if (MainMod.isShowCharsInMap)
            {
                int num = ((!MainMod.isHuntingBoss) ? 35 : 95);
                for (int k = 0; k < MainMod.listCharsInMap.Count; k++)
                {
                    if (!GameCanvas.isPointerHoldIn(GameCanvas.w - MainMod.widthRect + 15, num + (MainMod.heightRect + 1) * k, MainMod.widthRect, MainMod.heightRect))
                    {
                        continue;
                    }
                    GameCanvas.isPointerJustDown = false;
                    GameScr.gI().isPointerDowning = false;
                    if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        Char @char = MainMod.listCharsInMap[k];
                        if (Char.myCharz().charFocus != null && Char.myCharz().charFocus.cName == @char.cName)
                        {
                            MainMod.TeleportTo(Char.myCharz().charFocus.cx, Char.myCharz().charFocus.cy);
                        }
                        else
                        {
                            MainMod.FocusToChar(@char.charID);
                        }
                        Char.myCharz().currentMovePoint = null;
                        GameCanvas.clearAllPointerEvent();
                        return;
                    }
                }
            }
        }
        if (MainMod.isShowCharsInMap && MainMod.isMeInNRDMap())
        {
            for (int l = 0; l < MainMod.listCharsInMap.Count; l++)
            {
                if (!GameCanvas.isPointerHoldIn(GameCanvas.w - MainMod.widthRect, 35 + (MainMod.heightRect + 1) * l, MainMod.widthRect, MainMod.heightRect))
                {
                    continue;
                }
                GameCanvas.isPointerJustDown = false;
                GameScr.gI().isPointerDowning = false;
                if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                {
                    Char char2 = MainMod.listCharsInMap[l];
                    if (Char.myCharz().charFocus != null && Char.myCharz().charFocus.cName == char2.cName)
                    {
                        MainMod.TeleportTo(Char.myCharz().charFocus.cx, Char.myCharz().charFocus.cy);
                    }
                    else
                    {
                        MainMod.FocusToChar(char2.charID);
                    }
                    Char.myCharz().currentMovePoint = null;
                    GameCanvas.clearAllPointerEvent();
                    return;
                }
            }
        }
        //if (!isButton)
        //{
        //    if (!GameCanvas.isPointerHoldIn(GameScr.xHP - 20, GameScr.yHP + 15 - 6 - 40 + 10, GameScr.imgNut.getWidth(), GameScr.imgNut.getHeight()))
        //    {
        //        return;
        //    }
        //    //Debug.Log("Click!");
        //    if (!Char.myCharz().havePet)
        //    {
        //        GameScr.info1.addInfo("Không có đệ tử!", 0);
        //        return;
        //    }
        //    if (!HavePorata())
        //    {
        //        GameScr.info1.addInfo("Không có bông tai!", 0);
        //        return;
        //    }
        //    GameCanvas.isPointerJustDown = false;
        //    GameScr.gI().isPointerDowning = false;
        //    if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
        //    {
        //        for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
        //        {
        //            Item item = Char.myCharz().arrItemBag[i];
        //            if (item != null && item.template.name.Contains("Porata"))
        //            {
        //                Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
        //            }
        //        }
        //        Char.myCharz().currentMovePoint = null;
        //        GameCanvas.clearAllPointerEvent();
        //        return;
        //    }
        //}
    }
}
