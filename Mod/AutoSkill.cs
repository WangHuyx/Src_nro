using Assets.src.e;
using System;
using System.Collections.Generic;
using System.IO;

namespace NRO;

public class AutoSkill : IActionListener, IChatable
{
	private static AutoSkill _Instance;

	public static bool isLoadKeySkill;

	public static bool isAutoSendAttack;

    public static bool TSdetu;
    public static bool DoSatInMapnrsd;
    public static long islastTele;
    public static long Delaypem;
    public static bool isKickSuper;
    public static bool isAttack;

    public static long[] lastTimeSendAttack;

	public static bool isTrainPet;

    public static bool isAutoAttackOtherChars;

    public static bool isPetAskedForUseSkill;

	public static bool[] isAutoUseSkills;

	private static long[] lastTimeUseSkill;

	private static long[] timeAutoSkills;

    public static string[] inputHPChar;

    public static string[] inputClanExclude;

    public static string[] inputSymbolClanExclude;

    public static string[] inputClanTS;

    public static string[] inputClanSymbol;

    private static int indexSkillAuto;

    public static int limitHPChar;

    public static int boolTD;

    public static int boolXD;

    public static int boolNM;

    private static bool isAutoChangeFocus;

    public static long delayUseSkill;

    private static long cooldownAutoChangeFocus;

	private static long lastTimeChangeFocus;

	private static List<Char> listTargetAutoChangeFocus;

    public static List<int> cGender;

    public static List<string> clanExclude;

    public static List<string> kihieuclanExclude;

    public static List<string> clanTS;

    public static List<string> kihieuclanTS;

    private static int targetIndex;

    public static int HPattack;

    private static bool isAutoShield;

    private static string[] inputDelay;

	private static bool isSaveSkill;

	private static long lastTimeAutoUseSkill;

    private static string[] inputHPBossMobAttack;

    public static long timeUseProtect;

    public static double speedFactor=0.57;

    public static AutoSkill getInstance()
	{
		if (_Instance == null)
		{
			_Instance = new AutoSkill();
		}
		return _Instance;
	}

    static AutoSkill()
    {
        boolTD = 0;
        boolXD = 1;
        boolNM = 2;
        limitHPChar = -1;
        isTrainPet = true;
        isLoadKeySkill = true;
        lastTimeSendAttack = new long[10];
        isAutoUseSkills = new bool[10];
        lastTimeUseSkill = new long[10];
        timeAutoSkills = new long[10];
        listTargetAutoChangeFocus = new List<Char>();
        cGender = new List<int>(new int[3]) { 0, 1, 2 };
        clanExclude = new List<string>();
        kihieuclanExclude = new List<string>();
        clanTS = new List<string>();
        kihieuclanTS = new List<string>();
        inputHPChar = new string[2] { "Nhập HP Char:", "HP" };
        inputHPBossMobAttack = new string[2] { "Nhập mức HP", "HP" };
        inputDelay = new string[2] { "Nhập delay", "mili giây" };
        inputClanExclude = new string[2] { "Nhập Danh Sách ClanID Không Tàn Sát", "ID Clan loại trừ" };
        inputSymbolClanExclude = new string[2] { "Nhập Danh Sách Kí Hiệu Clan Không Tàn Sát", "Kí Hiệu Clan loại trừ" };
        inputClanTS = new string[2] { "Nhập Danh Sách ID Clan Tàn Sát", "ID Clan tấn công" };
        inputClanSymbol = new string[2] { "Nhập Danh Sách Kí Hiệu Clan Tàn Sát", "Kí Hiệu Clan tấn công" };
        //LoadData();
    }

    public static void Update()
	{
		if (isAutoSendAttack)
		{
			AutoSendAttack();
		}
        if (isAutoAttackOtherChars)
        {
            TanSatPlayer();
        }
        if (isKickSuper)
        {
            KickSuper();
        }
        if (isAttack)
        {
            Anattack();
        }
        if (MainMod.AutoProtect)
		{
			AutoProtect();
		}
		if (isPetAskedForUseSkill && GameCanvas.gameTick % 20 == 0)
		{
			AutoSkillForPet();
		}
		if (!Char.myCharz().meDead)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				if (isAutoUseSkills[i])
				{
					AutoUseSkill(i);
				}
			}
		}
		if (isLoadKeySkill && GameCanvas.gameTick % 20 == 0)
		{
			isLoadKeySkill = false;
            LoadKeySkills();
		}
		if (isAutoChangeFocus)
		{
			AutoChangeFocus();
		}
	}

    public static void Anattack()
    {
        if (Char.myCharz().mobFocus == null && global::Char.myCharz().charFocus == null)
        {
            GameScr.info1.addInfo("Chưa chỉ định mục tiêu!", 0);
            return;
        }
        if (Char.myCharz().mobFocus != null && global::Char.myCharz().charFocus != null)
        {
            GameScr.info1.addInfo("Lỗi! Đang chỉ định 2 mục tiêu!", 0);
            return;
        }
        if (!ItemTime.isExistItem(4387))
        {
            GameScr.info1.addInfo("Khuyến khích bật tdlt", 0);
        }
        if (Char.myCharz().mobFocus != null)
        {
            if (mSystem.currentTimeMillis() - Delaypem > AutoSkill.GetCoolDown(Char.myCharz().myskill))
            {
                Delaypem = mSystem.currentTimeMillis();
                if (!ItemTime.isExistItem(4387))
                {
                    global::Char.myCharz().cx = Char.myCharz().mobFocus.x;
                    Service.gI().charMove();
                    global::Char.myCharz().cx = Char.myCharz().mobFocus.x - 1;
                    Service.gI().charMove();
                }
                MyVector myVector = new MyVector();
                myVector.addElement(global::Char.myCharz().mobFocus);
                Service.gI().sendPlayerAttack(myVector, new MyVector(), -1);
            }
        }
        if (Char.myCharz().charFocus != null)
        {
            if (mSystem.currentTimeMillis() - islastTele < 500L)
            {
                return;
            }
            islastTele = mSystem.currentTimeMillis();
            global::Char.myCharz().cx = global::Char.myCharz().charFocus.cx;
            global::Char.myCharz().cy = global::Char.myCharz().charFocus.cy;
            Service.gI().charMove();
            if (!ItemTime.isExistItem(4387))
            {
                global::Char.myCharz().cx = global::Char.myCharz().charFocus.cx;
                global::Char.myCharz().cy = global::Char.myCharz().charFocus.cy + 1;
                Service.gI().charMove();
                global::Char.myCharz().cx = global::Char.myCharz().charFocus.cx;
                global::Char.myCharz().cy = global::Char.myCharz().charFocus.cy;
                Service.gI().charMove();
            }
            MyVector myVector = new MyVector();
            myVector.addElement(global::Char.myCharz().charFocus);
            Service.gI().sendPlayerAttack(new MyVector(), myVector, -1);
        }
    }

    public static void KickSuper()
    {
        int cdir = 1;
        if (global::Char.myCharz().charFocus == null)
        {
            return;
        }
        if (mSystem.currentTimeMillis() - islastTele < 500L)
        {
            return;
        }
        islastTele = mSystem.currentTimeMillis();
        if (Char.myCharz().cx >= TileMap.pxw / 2)
        {
            cdir = -1;
        }
        else
        {
            cdir = 1;
        }
        if (Math.abs(Char.myCharz().cx - global::Char.myCharz().charFocus.cx) < 90)
        {
            int cxTele = (150 * cdir) + global::Char.myCharz().cx;
            global::Char.myCharz().cx = cxTele;
            global::Char.myCharz().cy = MainMod.GetYGround(cxTele);
            Service.gI().charMove();
            if (!ItemTime.isExistItem(4387))
            {
                global::Char.myCharz().cx = cxTele;
                global::Char.myCharz().cy = MainMod.GetYGround(cxTele) - 1;
                Service.gI().charMove();
                global::Char.myCharz().cx = cxTele;
                global::Char.myCharz().cy = MainMod.GetYGround(cxTele);
                Service.gI().charMove();
            }
        }
    }

    private static void WriteDataTS(string Path, List<string> text)
    {
        string includeFile = "";
        File.AppendAllText(Path, string.Empty); // Clear the file before writing
        for (int i = 0; i < text.Count; i++)
        {
            includeFile = includeFile + text[i] + ",";
        }
        File.WriteAllText(Path, includeFile);
    }
    private static void LoadDataTS(string Path, List<string> text)
    {
        string[] InPath = File.ReadAllText(Path).Split(',');
        for (int i = 0; i < InPath.Length; i++)
        {
            text.Add(InPath[i]);
        }
    }
    public static bool FindListGender(int cgender)
    {
        for (int i = 0; i < cGender.Count; i++)
        {
            if (cGender[i].ToString() != "" && cgender.Equals(cGender[i]))
            {
                return true;
            }
        }
        return false;
    }

    public void onChatFromMe(string text, string to)
	{
		if (ChatTextField.gI().tfChat.getText() != null && !ChatTextField.gI().tfChat.getText().Equals(string.Empty) && !text.Equals(string.Empty) && text != null)
		{
			if (ChatTextField.gI().strChat.Equals(inputDelay[0]))
			{
				try
				{
					long num = long.Parse(ChatTextField.gI().tfChat.getText());
					timeAutoSkills[indexSkillAuto] = num;
					isAutoUseSkills[indexSkillAuto] = true;
					GameScr.info1.addInfo("Auto " + GameScr.keySkill[indexSkillAuto].template.name + ": " + NinjaUtil.getMoneys(num) + " mili giây", 0);
				}
				catch
				{
					GameScr.info1.addInfo("Vui Lòng Nhập Lại Delay!", 0);
				}
				ResetChatTextField();
			}
            else if (ChatTextField.gI().strChat.Equals(inputClanExclude[0]))
            {
                try
                {
                    string num4 = ChatTextField.gI().tfChat.getText();
                    clanExclude.Add(num4);
                    WriteDataTS("Data\\IDclan_Exclude.ini", clanExclude);
                    GameScr.info1.addInfo("Đã Thêm clan: " + num4, 0);
                }
                catch (Exception ex)
                {
                    GameScr.info1.addInfo("IDclan Không Hợp Lệ, Vui Lòng Nhập Lại ", 0);
                    GameCanvas.startOKDlg(ex.Message);
                }
                ResetChatTextField();
            }
            else if (ChatTextField.gI().strChat.Equals(inputSymbolClanExclude[0]))
            {
                try
                {
                    string kihieu = ChatTextField.gI().tfChat.getText().ToUpper();
                    kihieuclanExclude.Add(kihieu);
                    WriteDataTS("Data\\Symbol_Exclude.ini", kihieuclanExclude);
                    GameScr.info1.addInfo("Đã Thêm: " + kihieu, 0);
                }
                catch (Exception ex)
                {
                    GameScr.info1.addInfo("Kí Hiệu Clan Không Hợp Lệ, Vui Lòng Nhập Lại", 0);

                }
                ResetChatTextField();
            }
            else if (ChatTextField.gI().strChat.Equals(inputHPBossMobAttack[0]))
            {
                try
                {
                    int num2 = (HPattack = int.Parse(ChatTextField.gI().tfChat.getText()));
                    GameScr.info1.addInfo("Tấn công mục tiêu HP Dưới: " + Res.formatNumber2(num2), 0);
                }
                catch
                {
                    GameScr.info1.addInfo("HP Không Hợp Lệ, Vui Lòng Nhập Lại!", 0);
                }
                ResetChatTextField();
            }
            else if (ChatTextField.gI().strChat.Equals(inputHPChar[0]))
            {
                try
                {
                    int num2 = (limitHPChar = int.Parse(ChatTextField.gI().tfChat.getText()));                  
                }
                catch
                {
                    GameScr.info1.addInfo("HP Không Hợp Lệ, Vui Lòng Nhập Lại!", 0);
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
			isAutoSendAttack = !isAutoSendAttack;
			GameScr.info1.addInfo("Tự Đánh\n" + (isAutoSendAttack ? "[ON]" : "[OFF]"), 0);
			if (isAutoSendAttack)
			{
				isAutoChangeFocus = false;
			}
			break;
		case 2:
			isTrainPet = !isTrainPet;
			GameScr.info1.addInfo("Đánh Khi Đệ Cần\n" + (isTrainPet ? "[ON]" : "[OFF]"), 0);
			break;
		case 3:
		{
			MyVector myVector = new MyVector();
			myVector.addElement(new Command("Lưu skill\n"+(isSaveSkill ? "hiện tại" : "mặc định"), getInstance(), 5, null));
            for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				myVector.addElement(new Command(((GameScr.keySkill[i] != null) ? GameScr.keySkill[i].template.name : "null") + "\n[" + (i + 1) + "]\n", getInstance(), 10, i));
			}
			GameCanvas.menu.startAt(myVector, 3);
			break;
		}
		case 4:
			isAutoShield = !isAutoShield;
			GameScr.info1.addInfo("Auto Khiên Pro\n" + (isAutoShield ? "[ON]" : "[OFF]"), 0);
			break;
		case 5:
			isSaveSkill = !isSaveSkill; 
            Rms.saveRMSInt("AutoSaveSkill", isSaveSkill ? 1 : 0);
            break;
		case 6:
			isAutoChangeFocus = !isAutoChangeFocus;
			GameScr.info1.addInfo("Đánh Chuyển Mục Tiêu\n" + (isAutoChangeFocus ? "[ON]" : "[OFF]"), 0);
			if (isAutoChangeFocus)
			{
				isAutoSendAttack = false;
			}
			break;
		case 7:
			listTargetAutoChangeFocus.Clear();
			GameScr.info1.addInfo("Đã Xóa Danh Sách Đánh Chuyển Mục Tiêu!", 0);
			break;
		case 8:
			if (Char.myCharz().charFocus != null)
			{
				listTargetAutoChangeFocus.Remove(Char.myCharz().charFocus);
				GameScr.info1.addInfo("Đã Xóa " + Char.myCharz().charFocus.cName + " [" + Char.myCharz().charFocus.charID + "]", 0);
			}
			break;
		case 9:
			if (Char.myCharz().charFocus != null)
			{
				listTargetAutoChangeFocus.Add(Char.myCharz().charFocus);
				GameScr.info1.addInfo("Đã Thêm " + Char.myCharz().charFocus.cName + " [" + Char.myCharz().charFocus.charID + "]", 0);
			}
			break;
		case 10:
			ShowMenuAutoSkill((int)p);
			break;
		case 11:
		{
			int num2 = (int)p;
			isAutoUseSkills[num2] = !isAutoUseSkills[num2];
			if (isAutoUseSkills[num2])
			{
				timeAutoSkills[num2] = -1L;
			}
			GameScr.info1.addInfo("Auto " + GameScr.keySkill[num2].template.name + (isAutoUseSkills[num2] ? (": " + NinjaUtil.getMoneys(timeAutoSkills[num2]) + " mili giây") : "\n[OFF]"), 0);
			break;
		}
		case 12:
			ChatTextField.gI().strChat = inputDelay[0];
			ChatTextField.gI().tfChat.name = inputDelay[1];
			ChatTextField.gI().startChat2(getInstance(), string.Empty);
			indexSkillAuto = (int)p;
			break;
		case 13:
            Skill mySkillreFreeze = GameScr.keySkill[(int)p];
            if (mySkillreFreeze.coolDown != 0)
            {
                mySkillreFreeze.coolDown = 0;
                if (mySkillreFreeze.template.manaUseType != 1)
                {
                    mySkillreFreeze.manaUse = 0;
                }
                GameScr.info1.addInfo("Đóng Băng\n" + mySkillreFreeze.template.name, 0);
            }
            else
            {
                if (MainMod.skillCooldowns.TryGetValue(mySkillreFreeze.template.id, out int myCooldown))
                {
                    mySkillreFreeze.coolDown = myCooldown;
                }
                if (MainMod.skillManause.TryGetValue(mySkillreFreeze.template.id, out int mymanause))
                {
                    mySkillreFreeze.manaUse = mymanause;
                }
                GameScr.info1.addInfo("Hủy đóng băng skill\n" + mySkillreFreeze.template.name, 0);
            }
            break;
		case 14:
			ShowMenuMassacre();
            break;
        case 15:
			ShowAttack();
            break;
        case 16:
            ChatTextField.gI().strChat = inputHPBossMobAttack[0];
            ChatTextField.gI().tfChat.name = inputHPBossMobAttack[1];
            ChatTextField.gI().startChat2(getInstance(), string.Empty);
            break;
        case 17:
            isAttack = !isAttack;
            GameScr.info1.addInfo("Tự động tấn công boss,mob theo ngưỡng máu\n" + ((isAttack) ? "[ON]" : "[OFF]"), 0);
            break;
        case 18:
            isAutoAttackOtherChars = !isAutoAttackOtherChars;
            GameScr.info1.addInfo("Tàn Sát Người\n" + (isAutoAttackOtherChars ? "[ON]" : "[OFF]"), 0);
            break;
        case 19:
            ChatTextField.gI().strChat = inputHPChar[0];
            ChatTextField.gI().tfChat.name = inputHPChar[1];
            ChatTextField.gI().startChat2(getInstance(), string.Empty);
            break;
        case 20:
            ShowMenuExclude();
            break;
        case 21:
            ShowMenuGenderDoSat();
            break;
        case 22:
            ChatTextField.gI().strChat = inputClanExclude[0];
            ChatTextField.gI().tfChat.name = inputClanExclude[1];
            ChatTextField.gI().startChat2(getInstance(), string.Empty);
            break;
        case 23:
            ChatTextField.gI().strChat = inputSymbolClanExclude[0];
            ChatTextField.gI().tfChat.name = inputSymbolClanExclude[1];
            ChatTextField.gI().startChat2(getInstance(), string.Empty);
            break;
        case 24:
            ShowListDoSat1();
            break;
        case 25:
            ShowListDoSat2();
            break;
        case 26:
            TSdetu = !TSdetu;
            GameScr.info1.addInfo("Tàn Sát Đệ\n" + (DoSatInMapnrsd ? "[ON]" : "[OFF]"), 0);
            break;
		case 27:
            DoSatInMapnrsd = !DoSatInMapnrsd;
            GameScr.info1.addInfo("Chỉ Tàn Sát Trong NRSD\n" + (DoSatInMapnrsd ? "[ON]" : "[OFF]"), 0);
            break;
        case 28:
            if (!FindListGender((int)p))
            {
                cGender.Add((int)p);
            }
            else
            {
                cGender.Remove((int)p);
            }
            ShowMenuGenderDoSat();
            break;
        case 29:
            clanExclude.RemoveAt((int)p);
            WriteDataTS("Data\\IDclan_Exclude.ini", clanExclude);
            ShowListDoSat1();
            break;
        case 30:
            kihieuclanExclude.RemoveAt((int)p);
            WriteDataTS("Data\\Symbol_Exclude.ini", kihieuclanExclude);
            ShowListDoSat2();
            break;
        }
    }


	public static void ShowMenu()
	{
		//LoadData();
		MyVector myVector = new MyVector();
		myVector.addElement(new Command("Tự Đánh\n" + (isAutoSendAttack ? "[ON]" : "[OFF]"), getInstance(), 1, null));
        myVector.addElement(new Command("Tấn công\nNgười chơi", getInstance(), 14, null));
        myVector.addElement(new Command("Tấn công\nBoss,Quái", getInstance(), 15, null));
        myVector.addElement(new Command(GameScr.keySkill.Length + " Ô Kỹ Năng", getInstance(), 3, null));
        myVector.addElement(new Command("Đánh Khi Đệ Cần\n" + (isTrainPet ? "[ON]" : "[OFF]"), getInstance(), 2, null));
		myVector.addElement(new Command("Đánh Chuyển Mục Tiêu\n" + (isAutoChangeFocus ? "[ON]" : "[OFF]"), getInstance(), 6, null));
		if (listTargetAutoChangeFocus.Count > 0)
		{
			myVector.addElement(new Command("Clear Danh Sách Chuyển Mục Tiêu", getInstance(), 7, null));
		}
		if (Char.myCharz().charFocus != null)
		{
			if (listTargetAutoChangeFocus.Contains(Char.myCharz().charFocus))
			{
				myVector.addElement(new Command("Xóa Khỏi Danh Sách Chuyển Mục Tiêu", getInstance(), 8, null));
			}
			else
			{
				myVector.addElement(new Command("Thêm Vào Danh Sách Chuyển Mục Tiêu", getInstance(), 9, null));
			}
		}
		GameCanvas.menu.startAt(myVector, 3);
	}

	private static void ShowMenuAutoSkill(int skillIndex)
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command("Auto Sử Dụng\n" + (isAutoUseSkills[skillIndex] ? ("[" + NinjaUtil.getMoneys(timeAutoSkills[skillIndex]) + " mili giây]") : "[OFF]"), getInstance(), 11, skillIndex));
		myVector.addElement(new Command("Nhập Delay\n[mili giây]", getInstance(), 12, skillIndex));
		myVector.addElement(new Command("Đóng Băng\n" + ((GameScr.keySkill[skillIndex].coolDown == 0)?"[ON]":"[OFF]"), getInstance(), 13, skillIndex));
		GameCanvas.menu.startAt(myVector, 3);
	}
    
	public static void ShowAttack()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command((isAttack) ? "[ON]" : "[OFF]", getInstance(), 17, null));
        myVector.addElement(new Command("HP\ntấn công\n[" + HPattack + "]", getInstance(), 16, null));
        GameCanvas.menu.startAt(myVector, 3);
    }

    public static void ShowMenuMassacre()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command("Tàn Sát Người\n" + (isAutoAttackOtherChars ? "[ON]" : "[OFF]"), getInstance(), 18, null));
        myVector.addElement(new Command("Limit\n[" + NinjaUtil.getMoneys(limitHPChar) + "HP]", getInstance(), 19, null));
        myVector.addElement(new Command("Lọc Danh Sách\nTàn Sát", getInstance(), 20, null));

        GameCanvas.menu.startAt(myVector, 3);
    }
    public static void ShowMenuExclude()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command("Lọc \nHành Tinh", getInstance(), 21, null));
        myVector.addElement(new Command("Lọc \nIDClan", getInstance(), 22, null));
        myVector.addElement(new Command("Lọc \nKí Hiệu", getInstance(), 23, null));
        //myVector.addElement(new Command("Tàn Sát\nIDClan", getInstance(), 44, null));
        //myVector.addElement(new Command("Tàn Sát\nKí Hiệu", getInstance(), 45, null));
        myVector.addElement(new Command("Xem Danh Sách\nIDClan Lọc", getInstance(), 24, null));
        myVector.addElement(new Command("Xem Danh Sách\nKí Hiệu Clan Lọc", getInstance(), 25, null));
        myVector.addElement(new Command("Tàn Sát\nĐệ Tử\n" + (TSdetu ? "[ON]" : "[OFF]"), getInstance(), 26, null));
        myVector.addElement(new Command("Tàn Sát\nIn NRSD\n" + (DoSatInMapnrsd ? "[ON]" : "[OFF]"), getInstance(), 27, null));
        GameCanvas.menu.startAt(myVector, 3);
    }

    public static void ShowMenuGenderDoSat()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command("Trái Đất\n" + ((cGender.IndexOf(0) != -1) ? "[ON]" : "[OFF]"), getInstance(), 28, 0));
        myVector.addElement(new Command("Xayda\n" + ((cGender.IndexOf(2) != -1) ? "[ON]" : "[OFF]"), getInstance(), 28, 2));
        myVector.addElement(new Command("Namec\n" + ((cGender.IndexOf(1) != -1) ? "[ON]" : "[OFF]"), getInstance(), 28, 1));
        GameCanvas.menu.startAt(myVector, 3);
    }
    public static void ShowListDoSat1()
    {
        MyVector myVector = new MyVector();
        for (int i = 0; i < clanExclude.Count; i++)
        {
            myVector.addElement(new Command("Xóa ClanID\n" + clanExclude[i], getInstance(), 29, i));
        }
        GameCanvas.menu.startAt(myVector, 3);
    }
    public static void ShowListDoSat2()
    {
        MyVector myVector = new MyVector();
        for (int i = 0; i < kihieuclanExclude.Count; i++)
        {
            myVector.addElement(new Command("Xóa Clan\n" + kihieuclanExclude[i], getInstance(), 30, i));

        }
        GameCanvas.menu.startAt(myVector, 3);
    }
    private static void ResetChatTextField()
	{
		ChatTextField.gI().strChat = "Chat";
		ChatTextField.gI().tfChat.name = "chat";
		ChatTextField.gI().isShow = false;
	}

	private static void LoadData()
	{
        isSaveSkill = Rms.loadRMSInt("AutoSaveSkill") == 1;
        try
        {
            LoadDataTS("Data\\IDclan_Exclude.ini", clanExclude);
            LoadDataTS("Data\\Symbol_Exclude.ini", kihieuclanExclude);
        }
        catch (Exception ex)
        {
            File.WriteAllText("bug.txt", ex.Message);
        }
    }

	private static void LoadKeySkills()
	{
		if (isSaveSkill)
		{
            GameScr.gI().saveKeySkillToRMS();
            return;
		}
		for (int i = 0; i < Char.myCharz().nClass.skillTemplates.Length; i++)
		{
			SkillTemplate skillTemplate = Char.myCharz().nClass.skillTemplates[i];
			Skill skill = Char.myCharz().getSkill(skillTemplate);
			if (skill != null)
			{
				GameScr.keySkill[i] = skill;
			}
			GameScr.gI().saveKeySkillToRMS();
		}
	}
    private static bool isMeInNRDMap()
    {
        if (TileMap.mapID >= 85)
        {
            return TileMap.mapID <= 91;
        }
        return false;
    }
    public static bool ReturnName(Char gclass131_0)
    {
        if (gclass131_0.cName != null && gclass131_0.cName != "" && gclass131_0.charID != -Char.myCharz().charID && gclass131_0.charID != Char.myCharz().charID && !gclass131_0.isPet && !gclass131_0.isMiniPet && char.IsUpper(char.Parse(gclass131_0.cName.Substring(0, 1))) && gclass131_0.cName != "Trọng tài" && !gclass131_0.cName.StartsWith("#"))
        {
            return !gclass131_0.cName.StartsWith("$");
        }
        return false;
    }

    public static string GetFirstNames(string name)
    {
        if (name != null)
        {
            return name.Split(new char[]
            {
            ']'
            })[0].Replace("[", "").Trim().ToUpper();
        }
        return "";
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
        Char.myCharz().cx = x;
        Char.myCharz().cy = y + 1;
        Service.gI().charMove();
        Char.myCharz().cx = x;
        Char.myCharz().cy = y;
        Service.gI().charMove();
    }

    public static int cHpLimit()
    {
        if (limitHPChar != -1)
        {
            return limitHPChar;
        }
        return 2000000000;
    }
    public static bool Player(Char gclass131_0)
    {
        if (TileMap.mapID == 113)
        {
            if (gclass131_0 != null && Char.myCharz().myskill != null)
            {
                if (gclass131_0.cTypePk != 5)
                {
                    return gclass131_0.cTypePk == 3;
                }
                return true;
            }
            return false;
        }
        if (gclass131_0 != null && Char.myCharz().myskill != null)
        {
            if (gclass131_0.statusMe == 14 || gclass131_0.statusMe == 5 || Char.myCharz().myskill.template.type == 2 || ((Char.myCharz().cFlag != 8 || gclass131_0.cFlag == 0) && (Char.myCharz().cFlag == 0 || gclass131_0.cFlag != 8) && (Char.myCharz().cFlag == gclass131_0.cFlag || Char.myCharz().cFlag == 0 || gclass131_0.cFlag == 0) && (gclass131_0.cTypePk != 3 || Char.myCharz().cTypePk != 3) && Char.myCharz().cTypePk != 5 && gclass131_0.cTypePk != 5 && (Char.myCharz().cTypePk != 1 || gclass131_0.cTypePk != 1) && (Char.myCharz().cTypePk != 4 || gclass131_0.cTypePk != 4)))
            {
                if (Char.myCharz().myskill.template.type == 2)
                {
                    return gclass131_0.cTypePk != 5;
                }
                return false;
            }
            return true;
        }
        return false;
    }

    public static void TanSatPlayer()
    {
        if (!AutoSkill.isAutoSendAttack)
        {
            AutoSkill.isAutoSendAttack = true;
        }
        if (DoSatInMapnrsd && !isMeInNRDMap())
        {
            return;
        }
        for (int i = 0; i < GameScr.vCharInMap.size(); i++)
        {
            Char @char = (Char)GameScr.vCharInMap.elementAt(i);
            if (@char != null && !ReturnName(@char) && @char.cHP <= cHpLimit() && (TSdetu || (!TSdetu && @char.charID > 0)) && @char.cHP > 0 && Player(@char) && !@char.protectEff && cGender.Contains(@char.cgender))
            {
                if (@char.clanID == -1 || (@char.clanID != -1 && (!clanExclude.Contains(@char.clanID.ToString())) && (!kihieuclanExclude.Contains(GetFirstNames(@char.cName)))))
                {
                    Char.myCharz().charFocus = @char;
                    if (!Char.myCharz().isLockAttack && !Char.myCharz().isLockMove && Char.myCharz().charFocus != null && ((double)Math.abs(Char.myCharz().charFocus.cx - Char.myCharz().cx) >= (double)Char.myCharz().myskill.dx * 1.5 || (double)Math.abs(Char.myCharz().charFocus.cy - Char.myCharz().cy) >= (double)Char.myCharz().myskill.dy * 1.5) && mSystem.currentTimeMillis() - delayUseSkill >= 1500L)
                    {
                        delayUseSkill = mSystem.currentTimeMillis();
                        TeleportTo(Char.myCharz().charFocus.cx, Char.myCharz().charFocus.cy - 5);
                        break;
                    }
                }
            }
        }
    }

    public static void AutoSendAttack()
	{
		if (Char.myCharz().meDead || Char.myCharz().cHP <= 0 || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5 || Char.myCharz().myskill.template.type == 3 || Char.myCharz().myskill.template.id == 10 || Char.myCharz().myskill.template.id == 11 || (Char.myCharz().myskill.paintCanNotUseSkill && !GameCanvas.panel.isShow))
		{
			return;
		}
		int mySkillIndex = GetMySkillIndex();
        

        if (mSystem.currentTimeMillis() - lastTimeSendAttack[mySkillIndex] >= GetCoolDown(Char.myCharz().myskill))
		{
			if (GameScr.gI().isMeCanAttackMob(Char.myCharz().mobFocus))
			{
				Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
				SendAttackToMobFocus();
				//lastTimeSendAttack[mySkillIndex] = mSystem.currentTimeMillis();
			}
			else if (Char.myCharz().charFocus != null && isMeCanAttackChar(Char.myCharz().charFocus) && (double)Math.abs(Char.myCharz().charFocus.cx - Char.myCharz().cx) < (double)Char.myCharz().myskill.dx * 1.7)
			{
				Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
				SendAttackToCharFocus();
				//lastTimeSendAttack[mySkillIndex] = mSystem.currentTimeMillis();
			}
		}
	}

	private static void AutoSkillForPet()
	{
		if (GameScr.vMob.size() == 0)
		{
			return;
		}
		Mob mobFocus = (Mob)GameScr.vMob.elementAt(0);
		int num = 0;
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = (Mob)GameScr.vMob.elementAt(i);
			int num2 = Math.abs(Char.myCharz().cx - mob.x);
			int num3 = Math.abs(Char.myCharz().cy - mob.y);
			int num4 = num2 * num2 + num3 * num3;
			if (num < num4)
			{
				num = num4;
				mobFocus = mob;
			}
		}
		if (Char.myCharz().myskill != GameScr.keySkill[0])
		{
			GameScr.gI().doSelectSkill(GameScr.keySkill[0], isShortcut: true);
		}
		if (Char.myCharz().mobFocus == null)
		{
			Char.myCharz().mobFocus = mobFocus;
			return;
		}
		SendAttackToMobFocus();
		isPetAskedForUseSkill = false;
	}

	private static void AutoUseSkill(int skillIndex)
	{
		if (TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23)
		{
			return;
		}
		if (skillIndex >= GameScr.keySkill.Length)
		{
			skillIndex = GameScr.keySkill.Length - 1;
		}
		if (skillIndex < 0)
		{
			skillIndex = 0;
		}
		if (GameScr.keySkill[skillIndex].coolDown == 0)
		{
			timeAutoSkills[skillIndex] = -1;
		}      
        if (GameScr.keySkill[skillIndex] == null || GameScr.keySkill[skillIndex].paintCanNotUseSkill|| Char.myCharz().sleepEff || Char.myCharz().holdEffID != 0|| (GameScr.keySkill[skillIndex] == Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[8])&& Char.myCharz().protectEff))
        {
            return;
        }
        else if (isMeHasEnoughMP(GameScr.keySkill[skillIndex]) && !GameScr.gI().isCharging() && mSystem.currentTimeMillis() - lastTimeAutoUseSkill > 150L)
		{
			if (timeAutoSkills[skillIndex] == -1 && GameCanvas.gameTick % 20 == 0)
			{
				lastTimeUseSkill[skillIndex] = mSystem.currentTimeMillis();
				lastTimeAutoUseSkill = mSystem.currentTimeMillis();
				GameScr.gI().doSelectSkill(GameScr.keySkill[skillIndex], isShortcut: true);
			}
			if (mSystem.currentTimeMillis() - lastTimeUseSkill[skillIndex] > timeAutoSkills[skillIndex])
			{
				lastTimeUseSkill[skillIndex] = mSystem.currentTimeMillis();
				lastTimeAutoUseSkill = mSystem.currentTimeMillis();
				GameScr.gI().doSelectSkill(GameScr.keySkill[skillIndex], isShortcut: true);
			}
		}
	}

	public static bool isMeCanAttackChar(Char ch)
	{
		if (TileMap.mapID == 113)
		{
			if (ch != null && Char.myCharz().myskill != null)
			{
				if (ch.cTypePk != 5)
				{
					return ch.cTypePk == 3;
				}
				return true;
			}
			return false;
		}
		if (ch != null && Char.myCharz().myskill != null)
		{
			if (ch.statusMe != 14 && ch.statusMe != 5 && Char.myCharz().myskill.template.type != 2 && ((Char.myCharz().cFlag == 8 && ch.cFlag != 0) || (Char.myCharz().cFlag != 0 && ch.cFlag == 8) || (Char.myCharz().cFlag != ch.cFlag && Char.myCharz().cFlag != 0 && ch.cFlag != 0) || (ch.cTypePk == 3 && Char.myCharz().cTypePk == 3) || Char.myCharz().cTypePk == 5 || ch.cTypePk == 5 || (Char.myCharz().cTypePk == 1 && ch.cTypePk == 1) || (Char.myCharz().cTypePk == 4 && ch.cTypePk == 4)))
			{
				return true;
			}
			if (Char.myCharz().myskill.template.type == 2)
			{
				return ch.cTypePk != 5;
			}
            if (Char.myCharz().myskill.template.type == 4)
            {
				return false;
            }
            return false;
		}

        return false;
	}

	private static bool isMeHasEnoughMP(Skill skillToUse)
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

	private static void SendAttackToCharFocus()
	{
		try
		{
			MyVector myVector = new MyVector();
			myVector.addElement(Char.myCharz().charFocus);
			Service.gI().sendPlayerAttack(new MyVector(), myVector, 2);
		}
		catch
		{
		}
	}

	public static void SendAttackToMobFocus()
	{
		try
		{
			MyVector myVector = new MyVector();
			myVector.addElement(Char.myCharz().mobFocus);
			Service.gI().sendPlayerAttack(myVector, new MyVector(), 1);
		}
		catch
		{
		}
	}

	public static long GetCoolDown(Skill skill)
	{
		//if (skill.template.id != 20 && skill.template.id != 22 && skill.template.id != 7 && skill.template.id != 18 && skill.template.id != 23)
		//{
		//	return skill.coolDown + 100L;
		//}
		return (long)skill.coolDown /*+ 500L*/;
	}

	public static int GetMySkillIndex()
	{
		int num = 0;
		while (true)
		{
			if (num < GameScr.keySkill.Length)
			{
				if (GameScr.keySkill[num] == Char.myCharz().myskill)
				{
					break;
				}
				num++;
				continue;
			}
			return 0;
		}
		return num;
	}

	private static void AutoChangeFocus()
	{
		if (listTargetAutoChangeFocus.Count == 0)
		{
			GameScr.info1.addInfo("Danh sách chuyển mục tiêu trống!", 0);
			isAutoChangeFocus = false;
		}
		else
		{
			if (Char.myCharz().meDead || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5 || Char.myCharz().myskill.template.type == 3 || Char.myCharz().myskill.template.id == 10 || Char.myCharz().myskill.template.id == 11 || Char.myCharz().myskill.paintCanNotUseSkill)
			{
				return;
			}
			cooldownAutoChangeFocus = GetCooldownAutoChangeFocus(Char.myCharz().myskill);
			if (targetIndex >= listTargetAutoChangeFocus.Count)
			{
				targetIndex = 0;
			}
			if (mSystem.currentTimeMillis() - lastTimeChangeFocus >= cooldownAutoChangeFocus)
			{
				lastTimeChangeFocus = mSystem.currentTimeMillis();
				Char.myCharz().charFocus = GameScr.findCharInMap(listTargetAutoChangeFocus[targetIndex].charID);
				targetIndex++;
				if (targetIndex >= listTargetAutoChangeFocus.Count)
				{
					targetIndex = 0;
				}
				if (Char.myCharz().charFocus != null && isMeCanAttackChar(Char.myCharz().charFocus) && (double)Math.abs(Char.myCharz().charFocus.cx - Char.myCharz().cx) < (double)Char.myCharz().myskill.dx * 1.5)
				{
					Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
					SendAttackToCharFocus();
                }
            }
        }
	}

	private static long GetCooldownAutoChangeFocus(Skill skill)
	{
		if (skill.coolDown <= 500)
		{
			return 1000L;
		}
		return (long)((double)skill.coolDown * 1.2 + 200.0);
	}



	public static void FreezeSelectedSkill()
	{
        Skill mySkillreFreeze = Char.myCharz().myskill;
        if (mySkillreFreeze.coolDown != 0)
		{
			mySkillreFreeze.coolDown = 0;
			if (mySkillreFreeze.template.manaUseType != 1)
			{
				mySkillreFreeze.manaUse = 0;
			}
            GameScr.info1.addInfo("Đóng Băng\n" + mySkillreFreeze.template.name, 0);
		}
		else
		{
            if (MainMod.skillCooldowns.TryGetValue(mySkillreFreeze.template.id, out int myCooldown))
            {
                mySkillreFreeze.coolDown = myCooldown;                
            }
            if (MainMod.skillManause.TryGetValue(mySkillreFreeze.template.id, out int mymanause))
            {
                mySkillreFreeze.manaUse = mymanause;
            }
            GameScr.info1.addInfo("Hủy đóng băng skill\n"+ mySkillreFreeze.template.name, 0);
        }
	}

	public static void SendMyCharAttack()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(Char.myCharz());
		Service.gI().sendPlayerAttack(new MyVector(), myVector, 2);
	}

	private static void AutoProtect()
	{
		if (TileMap.mapID != 21+Char.myCharz().cgender && !Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[8]).paintCanNotUseSkill && isMeHasEnoughMP(Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[8])) && !GameScr.gI().isCharging() && !Char.myCharz().protectEff && !Char.myCharz().isBlind && !Char.myCharz().sleepEff && Char.myCharz().holdEffID == 0 && mSystem.currentTimeMillis() - timeUseProtect >= 150)
		{
			timeUseProtect = mSystem.currentTimeMillis();
			GameScr.gI().doSelectSkill(Char.myCharz().getSkill(Char.myCharz().nClass.skillTemplates[8]), isShortcut: true);
		}
	}

	private static int Keyskill()
	{
		for (int i = 0; i < GameScr.keySkill.Length; i++)
		{
			if (GameScr.keySkill[i] == Char.myCharz().myskill)
			{
				return i;
			}
		}
		return 0;
	}
}
