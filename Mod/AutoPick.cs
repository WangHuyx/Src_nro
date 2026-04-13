using QLTK;
using System;
using System.Collections.Generic;
using static NRO.AutoItem;

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
    private static ItemMap _cachedNearestItem;
    private static long _nextFocusScanAt;
    private static short _lastPickItemMapId = -1;
    private static long _lastPickAttemptAt;
    private const int MinPickDelayMs = 80;
    private const int FocusScanIntervalMs = 60;
    private const int RetrySameItemGapMs = 120;

	public static int DelayPickItem = 540;
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
        long now = mSystem.currentTimeMillis();
		if (isNRDMap())
		{
			PickVip();
		}
		else
		{
			if (now - lastTimePickedItem >= GetEffectivePickDelay())
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
        if (GameScr.vItemMap == null || GameScr.vItemMap.size() == 0)
        {
            _cachedNearestItem = null;
            return;
        }

        long now = mSystem.currentTimeMillis();
        if (now < _nextFocusScanAt && _cachedNearestItem != null && IsTargetPickable(_cachedNearestItem))
        {
            if (Char.myCharz().itemFocus != _cachedNearestItem)
            {
                Char.myCharz().itemFocus = _cachedNearestItem;
            }
            return;
        }

        ItemMap nearest = null;
        int bestScore = int.MaxValue;
		for (int i = 0; i < GameScr.vItemMap.size(); i++)
		{
			ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
            if (!IsTargetPickable(itemMap))
            {
                continue;
            }

			int dx = Math.abs(Char.myCharz().cx - itemMap.x);
			int dy = Math.abs(Char.myCharz().cy - itemMap.y);
            if (dx > maximumPickDistance || dy > maximumPickDistance)
            {
                continue;
            }

            int score = dx + dy;
            if (score < bestScore)
			{			
                bestScore = score;
                nearest = itemMap;
            }
		}

        _cachedNearestItem = nearest;
        _nextFocusScanAt = now + FocusScanIntervalMs;
        if (nearest != null && Char.myCharz().itemFocus != nearest)
        {
            Char.myCharz().itemFocus = nearest;
        }
	}

	public static void PickIt()
	{
        ItemMap focus = Char.myCharz().itemFocus;
        if (focus == null || !IsTargetPickable(focus))
        {
            return;
        }

        long now = mSystem.currentTimeMillis();
		if (now - lastTimePickedItem < GetEffectivePickDelay())
		{
			return;
		}
        if (focus.itemMapID == _lastPickItemMapId && now - _lastPickAttemptAt < RetrySameItemGapMs)
        {
            return;
        }

        int num = Math.abs(Char.myCharz().cx - focus.x);
        int num2 = Math.abs(Char.myCharz().cy - focus.y);
        if (isTeleportToItem && !Char.isLockKey)
		{
            if (Math.abs(focus.x - Char.myCharz().cx) > 60)
			{
				TeleportTo(focus.x, MainMod.GetYGround(focus.x));
			}
            if (Char.myCharz().cx < focus.x)
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
                TryPickNow(focus, now);
            }
			return;
        }
		if (Char.myCharz().cx < focus.x)
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
            TryPickNow(focus, now);
		}
		else
		{
			Char.myCharz().currentMovePoint = new MovePoint(focus.x, focus.y);
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
	public static void PickVip()
	{
		if (Char.myCharz().itemFocus == null || (Char.myCharz().itemFocus != null && !isNRD(Char.myCharz().itemFocus)))
		{
			int count = GameScr.vItemMap.size();
			for (int i = 0; i < count; i++)
			{
				ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
				if (itemMap != null && isNRD(itemMap))
				{
					Char.myCharz().charFocus = null;
					Char.myCharz().mobFocus = null;
					Char.myCharz().npcFocus = null;
					Char.myCharz().itemFocus = itemMap;
					break;
				}
			}
		}
        if (Char.myCharz().itemFocus != null && isNRD(Char.myCharz().itemFocus))
        {
            int num = Math.abs(Char.myCharz().cx - Char.myCharz().itemFocus.x);
            int num2 = Math.abs(Char.myCharz().cy - Char.myCharz().itemFocus.y);
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if ((num > 60 || num2 > 60))
            {
                if (now - LastTimeTeleToItem > 300L)
                {
                    TeleportTo(Char.myCharz().itemFocus.x, Char.myCharz().itemFocus.y);
                    TryPickNow(Char.myCharz().itemFocus, now);
                    LastTimeTeleToItem = now;
                }
            }
            else if (now - lastTimePickedItem >= GetEffectivePickDelay())
            {
                TryPickNow(Char.myCharz().itemFocus, now);
            }
        }
    }
    private static int GetEffectivePickDelay()
    {
        return (DelayPickItem < MinPickDelayMs) ? MinPickDelayMs : DelayPickItem;
    }

    private static bool IsTargetPickable(ItemMap item)
    {
        return item != null && item.template != null && item.template.id != 673 && isPickIt(item);
    }

    private static void TryPickNow(ItemMap focus, long now)
    {
        if (focus == null || focus.template == null || focus.template.id == 673)
        {
            return;
        }

        lastTimePickedItem = now;
        _lastPickAttemptAt = now;
        _lastPickItemMapId = focus.itemMapID;
        Service.gI().pickItem(focus.itemMapID);
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
    public static bool isNRDMap()
	{
        return TileMap.mapID >= 85 && TileMap.mapID <= 91;
    }

	public static bool isNRD(ItemMap item)
	{
		if (item.template.id >= 372)
		{
			return item.template.id <= 378;
		}
		return false;
	}

    public static bool TryFixItemPosition(int x, int y, out int validX, out int validY)
    {
        // Mặc định trả về tọa độ gốc
        validX = x;
        validY = y;
        // Bước 1: Kiểm tra xem vị trí hiện tại có phải vật cản không
        // Nếu không phải vật cản (false), trả về kết quả ngay lập tức
        if (!AutoTrain.isTileTypeMap(x, y))
        {
            return true;
        }
        // Bước 2: Quét 8 vị trí xung quanh với bước nhảy 60
        // Thứ tự quét: Ưu tiên 4 hướng thẳng (ngang/dọc) trước, 4 hướng chéo sau
        // vì trong cấu trúc map, hướng thẳng thường có xác suất trống cao hơn.
        if (!AutoTrain.isTileTypeMap(x - 60, y)) { validX = x - 60; return true; }
        if (!AutoTrain.isTileTypeMap(x + 60, y)) { validX = x + 60; return true; }
        if (!AutoTrain.isTileTypeMap(x, y - 60)) { validY = y - 60; return true; }
        if (!AutoTrain.isTileTypeMap(x, y + 60)) { validY = y + 60; return true; }


        // 5. Kiểm tra chéo: Trên-Trái
        if (!AutoTrain.isTileTypeMap(x - 60, y - 60)) { validX = x - 60; validY = y - 60; return true; }
        // 6. Kiểm tra chéo: Trên-Phải
        if (!AutoTrain.isTileTypeMap(x + 60, y - 60)) { validX = x + 60; validY = y - 60; return true; }
        // 7. Kiểm tra chéo: Dưới-Trái
        if (!AutoTrain.isTileTypeMap(x - 60, y + 60)) { validX = x - 60; validY = y + 60; return true; }
        // 8. Kiểm tra chéo: Dưới-Phải
        if (!AutoTrain.isTileTypeMap(x + 60, y + 60)) { validX = x + 60; validY = y + 60; return true; }

        // Không tìm thấy vị trí nào trống trong phạm vi 60
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
