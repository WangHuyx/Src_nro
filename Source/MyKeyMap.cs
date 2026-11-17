using System.Collections;

public class MyKeyMap
{
    private static Hashtable h;

    static MyKeyMap()
    {
        h = new Hashtable();

        // 1. Phím chữ cái (Win32: 0x41 - 0x5A) -> Game Code (97 - 122)
        for (int i = 0; i < 26; i++)
        {
            h.Add(0x41 + i, 97 + i);
        }

        // 2. Phím số (Win32: 0x30 - 0x39) -> Game Code (48 - 57)
        for (int i = 0; i < 10; i++)
        {
            h.Add(0x30 + i, 48 + i);
        }

        // 3. Phím chức năng và phím đặc biệt
        h.Add(0x20, 32);  // VK_SPACE
        h.Add(0x70, -21); // VK_F1
        h.Add(0x71, -22); // VK_F2
        h.Add(0x72, -23); // VK_F3

        h.Add(0xBB, -25); // VK_OEM_PLUS (+)
        h.Add(0xBD, 45);  // VK_OEM_MINUS (-)
        h.Add(0xBE, 46);  // VK_OEM_PERIOD (.)

        // --- Cụm phím di chuyển ---
        // 37 (0x25): VK_LEFT  -> Game code -3
        // 38 (0x26): VK_UP    -> Game code -1
        // 39 (0x27): VK_RIGHT -> Game code -4
        // 40 (0x28): VK_DOWN  -> Game code -2

        h.Add(38, -1); // VK_UP
        h.Add(40, -2); // VK_DOWN
        h.Add(37, -3); // VK_LEFT
        h.Add(39, -4); // VK_RIGHT

        h.Add(0x08, -8);  // VK_BACK
        h.Add(0x0D, -5);  // VK_RETURN (Enter)
        h.Add(0x09, -26); // VK_TAB
    }

    // Nhận vào wParam từ tin nhắn WM_KEYDOWN
    public static int map(int vkCode)
    {
        object obj = h[vkCode];
        return (obj == null) ? 0 : (int)obj;
    }
}