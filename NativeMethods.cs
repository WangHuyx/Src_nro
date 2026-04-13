using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace NRO
{
    public static partial class NativeMethods
    {
        #region Windows Constants
        public const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WM_DESTROY = 0x0002;
        public const uint WM_CLOSE = 0x0010;
        public const uint WM_QUIT = 0x0012;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_CHAR = 0x0102;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_MOUSEWHEEL = 0x020A;
        public const uint WS_FIXEDWINDOW = 0x00CA0000;
        #endregion

        #region Structs
        // Pack = 8 đảm bảo đúng layout Win32 trên cả 32-bit lẫn 64-bit
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct WNDCLASSEX
        {
            public uint cbSize;           // offset  0  (4 bytes)
            public uint style;            // offset  4  (4 bytes)
            public nint lpfnWndProc;      // offset  8  (8 bytes)
            public int cbClsExtra;       // offset 16  (4 bytes)
            public int cbWndExtra;       // offset 20  (4 bytes)
            public nint hInstance;        // offset 24  (8 bytes)
            public nint hIcon;            // offset 32  (8 bytes)
            public nint hCursor;          // offset 40  (8 bytes)
            public nint hbrBackground;    // offset 48  (8 bytes)
            public nint lpszMenuName;     // offset 56  (8 bytes)
            public nint lpszClassName;    // offset 64  (8 bytes)
            public nint hIconSm;          // offset 72  (8 bytes)
            // Tổng = 80 bytes ✓ (khớp Win32 WNDCLASSEXW 64-bit)
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public nint hwnd;
            public uint message;
            public nint wParam;
            public nint lParam;
            public uint time;
            public int ptX;
            public int ptY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT { public int Left, Top, Right, Bottom; }
        #endregion

        #region User32.dll

        [DllImport("user32.dll", EntryPoint = "RegisterClassExA",
            CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", EntryPoint = "CreateWindowExA",
            ExactSpelling = true, SetLastError = true)]
        public static extern nint CreateWindowEx(
            int dwExStyle,
            nint lpClassName,
            nint lpWindowName,
            uint dwStyle,
            int x, int y,
            int nWidth, int nHeight,
            nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

        //[DllImport("user32.dll", EntryPoint = "SetWindowTextA",
        //    CharSet = CharSet.Ansi, ExactSpelling = true)]
        //public static extern bool SetWindowText(nint hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern nint GetModuleHandle(string? lpModuleName);

        // Các hàm không có string — giữ nguyên
        [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
        [DllImport("user32.dll")] public static extern nint LoadCursor(nint hInstance, nint lpCursorName);
        //[DllImport("user32.dll")] public static extern bool ShowWindow(nint hWnd, int nCmdShow);
        //[DllImport("user32.dll")] public static extern bool UpdateWindow(nint hWnd);

        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out MSG lpMsg, nint hWnd,
            uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll")] public static extern bool TranslateMessage([In] ref MSG lpMsg);
        [DllImport("user32.dll")] public static extern nint DispatchMessage([In] ref MSG lpmsg);
        [DllImport("user32.dll")] public static extern nint DefWindowProc(nint hWnd, uint uMsg, nint wParam, nint lParam);
        [DllImport("user32.dll")] public static extern void PostQuitMessage(int nExitCode);
        [DllImport("user32.dll")] public static extern nint GetDC(nint hWnd);
        [DllImport("user32.dll")] public static extern int ReleaseDC(nint hWnd, nint hDC);
        [DllImport("user32.dll")] public static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRectEx(ref RECT lpRect,
            uint dwStyle, bool bMenu, uint dwExStyle);
        #endregion

        #region Gdi32.dll
        [DllImport("gdi32.dll")]
        public static extern int StretchDIBits(nint hdc,
            int xDest, int yDest, int nDestWidth, int nDestHeight,
            int xSrc, int ySrc, int nSrcWidth, int nSrcHeight,
            nint lpBits, [In] ref BITMAPINFO lpBitsInfo,
            uint iUsage, uint dwRop);
        #endregion
    }
}