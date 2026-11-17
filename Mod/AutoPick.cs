
using System;
using System.Collections.Generic;

namespace NRO;


public class AutoPick : IActionListener, IChatable
{
	private static AutoPick _Instance;

	public static bool isAutoPick;

	public static long lastTimePickedItem;

	public static int maximumPickDistance;

	public static bool isTeleportToItem;

	public static int pickByList;

	private static List<int> listItemAutoPick;

	private static string[] inputMaximumPickDistance;

	private static string[] inputItemID;

	private static long LastTimeTeleToItem;

    public static AutoPick getInstance()
	{
		if (_Instance == null)
		{
			_Instance = new AutoPick();
		}
		return _Instance;
	}


	public static void Update()
	{
		if ((GameScr.isAutoPlay && (GameScr.canAutoPlay || AutoTrain.isAutoTrain)) || !isAutoPick||Char.myCharz().meDead)
		{
			return;
		}
		if (isNRDMap(TileMap.mapID))
		{
			try
			{
				for (int i = 0; i < GameScr.vItemMap.size(); i++)
				{
					ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
                    int num = Math.abs(Char.myCharz().cx - itemMap.x);
                    int num2 = Math.abs(Char.myCharz().cy - itemMap.y);
                    if (itemMap != null && isNRD(itemMap))
					{
						Char.myCharz().charFocus = null;
						Char.myCharz().mobFocus = null;
						if((num > 60|| num2 > 60) && mSystem.currentTimeMillis() - LastTimeTeleToItem >300L)
						{
                            LastTimeTeleToItem = mSystem.currentTimeMillis();
                            TeleportTo(itemMap.x, itemMap.y);
                        }
						if ((num <= 60 || num2 <= 60) && mSystem.currentTimeMillis() - lastTimePickedItem >= 340L)
						{
							GameCanvas.clearKeyHold();
							GameCanvas.clearKeyPressed();
							Service.gI().pickItem(itemMap.itemMapID);
						}
                        break;
					}
				}
			}
			catch (Exception)
			{
			}
		}
		if (!isNRDMap(TileMap.mapID))
		{
			if (mSystem.currentTimeMillis() - lastTimePickedItem >= 340L)
			{                
                GameCanvas.keyPressed[17] = true;
            }			
        }
	}

	public void onChatFromMe(string text, string to)
	{
		if (ChatTextField.gI().tfChat.getText() != null && !ChatTextField.gI().tfChat.getText().Equals(string.Empty) && !text.Equals(string.Empty) && text != null)
		{
			if (ChatTextField.gI().strChat.Equals(inputMaximumPickDistance[0]))
			{
				try
				{
					int num = (maximumPickDistance = int.Parse(ChatTextField.gI().tfChat.getText()));
					GameScr.info1.addInfo("Khoảng Cách Nhặt: " + num, 0);
				}
				catch
				{
					GameScr.info1.addInfo("Số Không Hợp Lệ, Vui Lòng Nhập Lại!", 0);
				}
				ResetChatTextField();
			}
			else if (ChatTextField.gI().strChat.Equals(inputItemID[0]))
			{
				try
				{
					int item = int.Parse(ChatTextField.gI().tfChat.getText());
					listItemAutoPick.Add(item);
					GameScr.info1.addInfo("Đã Thêm Item " + item, 0);
				}
				catch
				{
					GameScr.info1.addInfo("Số Không Hợp Lệ, Vui Lòng Nhập Lại!", 0);
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
		if (GameScr.isPaintMessage)
		{
			GameScr.isPaintMessage = false;
			ChatTextField.gI().center = null;
		}
	}
	public void perform(int idAction, object p)
	{
		switch (idAction)
		{
		case 1:
			isAutoPick = !isAutoPick;
			pickByList = 0;
			GameScr.info1.addInfo("Auto Nhặt\n" + (isAutoPick ? "[ON]" : "[OFF]"), 0);
			break;
		case 2:
            isAutoPick = !isAutoPick;
            pickByList = 3;
            GameScr.info1.addInfo("Nhặt Tất Cả\n" + (isAutoPick ? "[ON]" : "[OFF]"), 0);
			break;
		case 3:
			isAutoPick = !isAutoPick;
			pickByList = 1;
			GameScr.info1.addInfo("Nhặt Theo Danh Sách\n" + (isAutoPick ? "[ON]" : "[OFF]"), 0);
			break;
		case 4:
			isTeleportToItem = !isTeleportToItem;
			GameScr.info1.addInfo("Dịch Đến Item\n" + (isTeleportToItem ? "[ON]" : "[OFF]"), 0);
			break;
		case 5:
			ChatTextField.gI().strChat = inputMaximumPickDistance[0];
			ChatTextField.gI().tfChat.name = inputMaximumPickDistance[1];
			ChatTextField.gI().startChat2(getInstance(), string.Empty);
			break;
		case 6:
			if (listItemAutoPick.Count == 0)
			{
				GameScr.info1.addInfo("Danh Sách Trống!", 0);
			}
			if (listItemAutoPick.Count > 0)
			{
				string text = "";
				for (int i = 0; i < listItemAutoPick.Count; i++)
				{
					text = text + listItemAutoPick[i] + " ";
				}
				GameScr.info1.addInfo(text, 0);
			}
			break;
		case 7:
			listItemAutoPick.Clear();
			GameScr.info1.addInfo("Đã Clear Danh Sách Nhặt!", 0);
			break;
		case 8:
			ChatTextField.gI().strChat = inputItemID[0];
			ChatTextField.gI().tfChat.name = inputItemID[1];
			ChatTextField.gI().startChat2(getInstance(), string.Empty);
			break;
		case 9:
			listItemAutoPick.Add(Char.myCharz().itemFocus.template.id);
			GameScr.info1.addInfo("Đã thêm " + Char.myCharz().itemFocus.template.name + " [" + Char.myCharz().itemFocus.template.id + "]", 0);
			break;
        case 10:
			ShowMenuPick();
            break;
        }
	}

	public static void ShowMenu()
	{
		MyVector myVector = new MyVector();
        myVector.addElement(new Command("Auto Nhặt\n" + ((!isAutoPick) ? "[OFF]" : "[ON]"), getInstance(), 10, null));
        myVector.addElement(new Command("Dịch Đến Item\n" + (isTeleportToItem ? "[ON]" : "[OFF]"), getInstance(), 4, null));
		myVector.addElement(new Command("Khoảng Cách Nhặt\n[" + maximumPickDistance + "]", getInstance(), 5, null));
		myVector.addElement(new Command("Xem Danh Sách Nhặt", getInstance(), 6, null));
		myVector.addElement(new Command("Clear Danh Sách Nhặt", getInstance(), 7, null));
		myVector.addElement(new Command("Thêm ItemID", getInstance(), 8, null));
		if (Char.myCharz().itemFocus != null)
		{
			myVector.addElement(new Command("Thêm: " + Char.myCharz().itemFocus.template.name + " [" + Char.myCharz().itemFocus.template.id + "] ", getInstance(), 9, null));
		}
		GameCanvas.menu.startAt(myVector, 3);
	}

    private static void ShowMenuPick()
    {
        MyVector myVector = new MyVector();
        myVector.addElement(new Command("Nhặt\n" + ((!isAutoPick || pickByList != 0) ? "[OFF]" : "[ON]"), getInstance(), 1, null));
        myVector.addElement(new Command("Nhặt Tất Cả\n" + (isAutoPick ? "[ON]" : "[OFF]"), getInstance(), 2, null));
        myVector.addElement(new Command("Nhặt Theo Danh Sách\n" + ((!isAutoPick || pickByList != 1) ? "[OFF]" : "[ON]"), getInstance(), 3, null));
        if (Char.myCharz().itemFocus != null)
        {
            myVector.addElement(new Command("Thêm: " + Char.myCharz().itemFocus.template.name + " [" + Char.myCharz().itemFocus.template.id + "] ", getInstance(), 9, null));
        }
        GameCanvas.menu.startAt(myVector, 3);
    }
    private static void ResetChatTextField()
	{
		ChatTextField.gI().strChat = "Chat";
		ChatTextField.gI().tfChat.name = "chat";
		ChatTextField.gI().isShow = false;
	}

	public static void FocusToNearestItem()
	{
		for (int i = 0; i < GameScr.vItemMap.size(); i++)
		{
			ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
			int num = Math.abs(Char.myCharz().cx - itemMap.x);
			int num2 = Math.abs(Char.myCharz().cy - itemMap.y);
			if (num <= maximumPickDistance && num2 <= maximumPickDistance && isPickIt(itemMap) && itemMap.template.id != 673)
			{			
				if (Char.myCharz().itemFocus != itemMap)
				{
					Char.myCharz().itemFocus = itemMap;
					if(Char.myCharz().cxSend - itemMap.x > 60 || Char.myCharz().cySend - itemMap.y > 60)
					{
						Service.gI().charMove();
                    }
                    break;
                }
            }
		}
	}

	public static void PickIt()
	{
		if (mSystem.currentTimeMillis() - lastTimePickedItem < 470L)
		{
			return;
		}
        int num = Math.abs(Char.myCharz().cx - Char.myCharz().itemFocus.x);
        int num2 = Math.abs(Char.myCharz().cy - Char.myCharz().itemFocus.y);
        if (/*isTeleportToItem &&*/ !Char.isLockKey)
		{
            if (Char.myCharz().itemFocus.x != Char.myCharz().cx)
			{
				Char.myCharz().cx = Char.myCharz().itemFocus.x;
				Char.myCharz().cy = MainMod.GetYGround(Char.myCharz().itemFocus.x);
                Service.gI().charMove();
            }
            if (Char.myCharz().cx < Char.myCharz().itemFocus.x)
            {
                Char.myCharz().cdir = 1;
            }
            else
            {
                Char.myCharz().cdir = -1;
            }

            if (num <= 60 && num2 < 60)
            {
                GameCanvas.clearKeyHold();
                GameCanvas.clearKeyPressed();
                if (Char.myCharz().itemFocus.template.id != 673)
                {
                    lastTimePickedItem = mSystem.currentTimeMillis();
                    Service.gI().pickItem(Char.myCharz().itemFocus.itemMapID);
                }
            }
			return;
        }
		if (Char.myCharz().cx < Char.myCharz().itemFocus.x)
		{
			Char.myCharz().cdir = 1;
		}
		else
		{
			Char.myCharz().cdir = -1;
		}
		if (num <= 60 && num2 < 60)
		{
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
			if (Char.myCharz().itemFocus.template.id != 673)
			{
                lastTimePickedItem = mSystem.currentTimeMillis();
                Service.gI().pickItem(Char.myCharz().itemFocus.itemMapID);
			}
		}
		else
		{
			Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().itemFocus.x, Char.myCharz().itemFocus.y);
			Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
		}
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

	public static bool isPickIt(ItemMap item)
	{
		int type = TypePick();
        if (type == 0)
		{
			return false;
		}
		if (type == 1)
		{
			if (item.playerId != Char.myCharz().charID)
			{
				return item.playerId == -1;
			}
			return true;
		}
		if (type == 2 && MainMod.CurrentSetting.ListItem.Contains(item.template.id))
		{
			if (item.playerId != Char.myCharz().charID)
			{
				return item.playerId == -1;
			}
			return true;
		}
		return false;
	}
    private static int TypePick()
    {
        var type = MainMod.CurrentSetting.PickItemType;

		return type switch
		{
			AccountSetting.TypePickItem.NotPick => 0,
			AccountSetting.TypePickItem.PickAll => 1,
			AccountSetting.TypePickItem.PickIndex => 2,
			_ => 0,
		};
    }
    private static bool isNRDMap(int mapID)
	{
		if (mapID >= 85)
		{
			return mapID <= 91;
		}
		return false;
	}

	private static bool isNRD(ItemMap item)
	{
		if (item.template.id >= 372)
		{
			return item.template.id <= 378;
		}
		return false;
	}

	static AutoPick()
	{
		maximumPickDistance = 60;
		listItemAutoPick = new List<int>();
		inputMaximumPickDistance = new string[2] { "Nhập khoảng cách nhặt", "khoảng cách (>50)" };
		inputItemID = new string[2] { "Nhập ID của item", "ID" };
	}
}
