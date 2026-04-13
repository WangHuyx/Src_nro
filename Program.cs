using NRO;
using SkiaSharp;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Security.Principal;
using static NRO.NativeMethods;

namespace NRO
{
    public class NativeTimer
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint TimeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint TimeEndPeriod(uint uMilliseconds);
    }
    class Program
    {
        // --- Globals ---
        private static nint _hwnd;
        private static SKBitmap _backBuffer;
        private static SKCanvas _canvas;
        private static NativeMethods.BITMAPINFO _bmi;
        private static Stopwatch _stopwatch = new();
        public static bool _isExiting = false;
        private static long DelayGCC = 0L;
        private static bool GCCfinal = false;
        public static bool toiuu = false;
        public static int mmrPublic = 0;
        public static int mmrPrivate = 0;
        public static bool PaintCanvas = true;
        private static int _tpsCounter = 0;
        private static int _fpsCounter = 0;
        private static long _lastTpsCheck = 0L;
        private static int CurrentTPS = 0;

        [DllImport("psapi.dll")]
        static extern bool EmptyWorkingSet(IntPtr hProcess);
        public static void Main(string[] args)
        {
            if (args.Length < 0) { _isExiting = true; return; }
            string[] Data = args[0].Split('|');
            if (Data.Length < 7) { _isExiting = true; return; }
            int width=600;
            int height=300;
            const string ClassName = "NROLiteWindow";
            string WindowTitle = "NROLite";
            Begin.main = new Begin(); // 1. Khởi tạo Logic Game
            Begin.g = new mGraphics();
            Begin.main.Start();
            NativeTimer.TimeBeginPeriod(1);
            try
            {
                WindowTitle = Data[0];
                MainMod.account = Data[0];
                MainMod.server = int.Parse(Data[1]);
                MainMod.password = MainMod.DecryptString(Data[2]);
                if (!string.IsNullOrEmpty(Data[3]))
                {
                    ProxyClient.HasProxy = true;
                    string[] array2 = Data[3].Split(':');
                    // Kiểm tra tối thiểu phải có IP và Port
                    if (array2.Length >= 2)
                    {
                        ProxyClient.addressproxy = array2[0];
                        int.TryParse(array2[1], out ProxyClient.postproxy);
                    }
                    // Chỉ lấy User/Pass nếu mảng có đủ 4 phần tử
                    if (array2.Length >= 4)
                    {
                        ProxyClient.accountproxy = array2[2];
                        ProxyClient.passproxy = array2[3];
                    }
                }
                if (int.TryParse(Data[4], out int port))
                {
                    System.Threading.Tasks.Task.Run(async () =>
                    {
                        var conn = new LauncherConnection();
                        await conn.ConnectAsync(port, Data[9]);
                    });
            }
                width = SetSize(int.Parse(Data[5]), int.Parse(Data[6]))[0];
                height = SetSize(int.Parse(Data[5]), int.Parse(Data[6]))[1];
                if (long.TryParse(Data[7], out long delayxmap)) AutoMap.LongWaitNextMap = delayxmap;
                if (int.TryParse(Data[8], out int delaylogin)) MainMod.longDelayLogin = delaylogin;
            }
            catch (TypeInitializationException ex)
            {
                //Console.WriteLine("Lỗi gốc rễ là: " + ex.InnerException?.Message);
                //Console.WriteLine("Tại dòng: " + ex.InnerException?.StackTrace);
                _isExiting = true;
                return;
            }
            unsafe
            {
                NativeMethods.SetProcessDPIAware();
                nint hInst = NativeMethods.GetModuleHandle(null);
                delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint> wndProcPtr = &WndProc;// 1. Khai báo con trỏ hàm WndProc
                nint pClassName = Marshal.StringToHGlobalAnsi(ClassName);
                nint pWindowTitle = Marshal.StringToHGlobalAnsi(WindowTitle);
                InitGraphics(width, height);
                var wndClass = new NativeMethods.WNDCLASSEX
                {
                    cbSize = (uint)Marshal.SizeOf<NativeMethods.WNDCLASSEX>(),
                    style = 3, // CS_HREDRAW | CS_VREDRAW
                    lpfnWndProc = (nint)wndProcPtr,
                    hInstance = hInst,
                    lpszClassName = pClassName,
                    lpszMenuName = IntPtr.Zero,
                    hbrBackground = (nint)6, // COLOR_WINDOW + 1
                    hCursor = LoadCursor(IntPtr.Zero, (nint)32512)
                };
                if (NativeMethods.RegisterClassEx(ref wndClass) == 0) return;
                Int32 err = Marshal.GetLastWin32Error();
                RECT rc = new RECT { Left = 0, Top = 0, Right = width, Bottom = height };
                NativeMethods.AdjustWindowRectEx(ref rc, NativeMethods.WS_FIXEDWINDOW, false, 0);

                int fullWidth = rc.Right - rc.Left;
                int fullHeight = rc.Bottom - rc.Top;

                _hwnd = NativeMethods.CreateWindowEx(0, pClassName, pWindowTitle,
                        NativeMethods.WS_FIXEDWINDOW | NativeMethods.WS_VISIBLE,
                        1, 1, fullWidth, fullHeight,
                        IntPtr.Zero, IntPtr.Zero, hInst, IntPtr.Zero);

                if (_hwnd == IntPtr.Zero) return;
                Marshal.FreeHGlobal(pClassName);
                Marshal.FreeHGlobal(pWindowTitle);
            }
            // 3. Game Loop
            double targetTPS = 118.0;
            double msPerTick = 1000.0 / targetTPS;

            // Đặt FPS
            double targetFPS = 60.0;
            double msPerFrame = 1000.0 / targetFPS;

            double accumulator = 0;
            double renderTimer = 0;
            Stopwatch sw = Stopwatch.StartNew();
            double totalElapsed = 0;
            double lastTime = 0;
            //_stopwatch.Start();
            while (!_isExiting)
            {
                unsafe
                {
                    NativeMethods.MSG msg;
                    while (NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 1))
                    {
                        NativeMethods.TranslateMessage(ref msg);
                        NativeMethods.DispatchMessage(ref msg);
                        if (msg.message == 0x0012) _isExiting = true; // WM_QUIT
                    }
                }
                //2.Cứ sau mỗi 1000ms(1 giây), chốt số lượng Tick và reset bộ đếm
                if (mSystem.currentTimeMillis() - _lastTpsCheck >= 1000L)
                {
                    //CurrentTPS = _tpsCounter;
                    _lastTpsCheck = mSystem.currentTimeMillis();
                    // In ra Console để theo dõi
                    //Console.WriteLine($">>> Current TPS Program: {_tpsCounter} {_fpsCounter}");
                    _tpsCounter = 0;
                    _fpsCounter = 0;
                    //if (cpu.GetCurrentProcessCpuUsage() >= 80.0)
                    //{
                    //    Thread.Sleep(10000);
                    //    Cleanup();
                    //}
                }
                double deltaTime = sw.Elapsed.TotalMilliseconds - lastTime;
                lastTime = sw.Elapsed.TotalMilliseconds;

                accumulator += deltaTime;
                renderTimer += deltaTime;

                int updatesThisFrame = 0;
                // 1. LOGIC LUÔN CHẠY ĐÚNG TỐC ĐỘ (118 TPS)
                while (accumulator >= msPerTick & updatesThisFrame < 5)
                {
                    Tick(); // updateCount tăng ở đây
                    accumulator -= msPerTick;
                    updatesThisFrame++;
                }

                // 2. VẼ CÁCH QUÃNG (5 FPS)
                // Chỉ vào hàm Render nếu đã trôi qua 200ms (1000/5)
                if (renderTimer >= msPerFrame)
                {
                    Render(); // OnGUI chạy ở đây
                    renderTimer -= msPerFrame;
                }
                double nextTick = System.Math.Min(msPerTick - accumulator, msPerFrame - renderTimer);
                if (nextTick > 1.0)
                {
                    // Chỉ nghỉ nếu thời gian chờ > 1ms để tránh làm hụt nhịp [cite: 2026-04-09]
                    Thread.Sleep(1);
                }
                else if (nextTick > 0.1)
                {
                    // Chờ ngắn < 2ms: Nhường luồng tạm thời nhưng không Sleep (nhanh hơn Sleep)
                    Thread.Yield();
                }
                Thread.SpinWait(10);
                //MinimizeMemory();
            }
            NativeTimer.TimeEndPeriod(1);
        }
        public static void PreciseSleep(double ms)
        {
            long ticks = (long)(ms * Stopwatch.Frequency / 1000.0);
            long startTicks = Stopwatch.GetTimestamp();

            // 1. Nếu thời gian cần nghỉ đủ dài (> 1.5ms), hãy nhường luồng để tiết kiệm CPU
            if (ms >= 1.5)
            {
                Thread.Sleep((int)(ms - 1.0));
            }
            // 2. Spin-wait: Chạy vòng lặp rỗng cực nhanh để canh thời gian chính xác tuyệt đối
            while (Stopwatch.GetTimestamp() - startTicks < ticks)
            {
                // Thread.Yield() giúp CPU không bị treo nhưng vẫn giữ được độ chính xác cao
                Thread.Yield();
            }
        }
        public static void MinimizeMemory()
        {
            if(GCCfinal) return;
            GCCfinal = true;
            GC.Collect(); // Ép dọn rác Managed
            GC.WaitForPendingFinalizers(); // Đợi dọn dẹp xong
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                EmptyWorkingSet(Process.GetCurrentProcess().Handle);
            }
        }

        public static void ShowRAMUsage()
        {
            // Lấy tiến trình hiện tại (chính là app này)
            using Process currentProcess = Process.GetCurrentProcess();
            // 1. WorkingSet64: Tổng lượng RAM vật lý hệ thống đang cấp cho tiến trình (bao gồm cả thư viện dùng chung)
            long workingSet = currentProcess.WorkingSet64;
            // 2. PrivateMemorySize64: Lượng RAM riêng biệt được cấp cho tiến trình (thường dùng cái này để đo độ "ngốn" RAM của logic code)
            long privateMemory = currentProcess.PrivateMemorySize64;
            mmrPublic = (int)(workingSet / 1024 / 1024);
            mmrPrivate = (int)(privateMemory / 1024 / 1024);

        }
        private static void Tick()
        {
            _tpsCounter++;
            Begin.main.FixedUpdate();
            //_stopwatch.Restart();

        }
        private static void Render()
        {
            if (Begin.g == null) return;
            _fpsCounter++;
            Begin.g._canvas = _canvas;
            _canvas.Clear(SKColors.Black);
            // Vẽ Game
            Begin.main.OnGUI(_canvas);
            // Đẩy lên màn hình qua GDI
            nint hdc = NativeMethods.GetDC(_hwnd);
            nint pixels = _backBuffer.GetPixels();
            NativeMethods.StretchDIBits(hdc, 0, 0, _backBuffer.Width, _backBuffer.Height,
                0, 0, _backBuffer.Width, _backBuffer.Height, pixels, ref _bmi, 0, 0x00CC0020);
            NativeMethods.ReleaseDC(_hwnd, hdc);
        }
        private static void DrawDebugOverlay(SKCanvas canvas)
        {
            // Lấy tên class của Screen hiện tại (Giả sử nằm trong GameCanvas)
            // Thay 'GameCanvas' bằng class chứa biến currentScreen của bạn
            string screenName = GameCanvas.currentScreen?.GetType().Name ?? "Null Screen";

            using var paint = new SKPaint
            {
                Color = SKColors.Yellow,
                IsAntialias = true,
                TextSize = 10,
                Typeface = SKTypeface.FromFamilyName("Arial", SKTypefaceStyle.Bold)
            };

            // Vẽ nền đen mờ phía sau chữ cho dễ đọc
            using var bgPaint = new SKPaint { Color = new SKColor(0, 0, 0, 150) };
            canvas.DrawRect(5, 5, 100, 28, bgPaint);

            // Vẽ tên Screen lên màn hình
            canvas.DrawText($"Current: {screenName} {ServerListScreen.testConnect}", 15, 25, paint);
        }
        private static int[] SetSize(int w, int h)
        {
            int[] result = {w,h};
            if (w * h >= 312000) result =new int[2]{600, 500};
            if (w * h >= 480000) result =new int[2]{1000, 576};
            return result;
            ;
        }
        private static void InitGraphics(int w, int h)
        {
            ScaleGUI.WIDTH = w;
            ScaleGUI.HEIGHT = h;
            _backBuffer = new SKBitmap(w, h, SKColorType.Bgra8888, SKAlphaType.Premul);
            _canvas = new SKCanvas(_backBuffer);
            _bmi = new NativeMethods.BITMAPINFO
            {
                bmiHeader = new NativeMethods.BITMAPINFOHEADER
                {
                    biSize = (uint)Marshal.SizeOf <NativeMethods.BITMAPINFOHEADER>(),
                    biWidth = w,
                    biHeight = -h,
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = 0
                }
            };
        }

        // Đánh dẫu Callback cho Windows (Native AOT cần cái này)
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
        {
           switch (msg)
           {
                case 0x0100: // WM_KEYDOWN
                    if ((int)wParam == 27)
                    {
                        PaintCanvas = !PaintCanvas;
                    }
                    bool isRepeat = ((int)lParam & 0x40000000) != 0;
                    if (!isRepeat)
                    {
                        HandleKeyEvent((int)wParam, true);
                    }
                    break;

                case 0x0101: // WM_KEYUP
                    HandleKeyEvent((int)wParam, false);
                    break;

                case 0x0201: // WM_LBUTTONDOWN
                    HandleMouseEvent((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF, 0);
                    break;

                case 0x0200: // WM_MOUSEMOVE
                    if (((int)wParam & 0x0001) != 0) // Kiểm tra MK_LBUTTON (đang kéo chuột)
                        HandleMouseEvent((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF, 1);
                    else
                        HandleMouseEvent((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF, 3);
                    break;

                case 0x0202: // WM_LBUTTONUP
                    HandleMouseEvent((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF, 2);
                    break;

                case 0x020A: // WM_MOUSEWHEEL
                    short delta = (short)((int)wParam >> 16);
                    GameMidlet.gameCanvas.scrollMouse(delta > 0 ? 10 : -10);
                    break;

                case 0x0010: // WM_CLOSE (Tương đương OnFormClosing)
                    Cleanup();

                    return 0;
            }
            return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private static void HandleKeyEvent(int vkCode, bool isDown)
        {
            int num = MyKeyMap.map(vkCode);

            // Xử lý logic Shift cho @ (64) và _ (95)
            // GetKeyState(0x10) < 0 kiểm tra phím Shift có đang đè hay không
            if (NativeMethods.GetKeyState(0x10) < 0)
            {
                if (vkCode == 0x32) num = 64;      // Key 2 + Shift -> @
                else if (vkCode == 0xBD) num = 95; // Key Minus + Shift -> _
            }

            if (num == 0) return;
            if (isDown)GameMidlet.gameCanvas.keyPressedz(num);
            else GameMidlet.gameCanvas.keyReleasedz(num);
        }

        private static void HandleMouseEvent(int x, int y, int type)
        {
            int gX = x / mGraphics.zoomLevel;
            int gY = (y / mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard;
            try
            {
                if (type == 0) GameMidlet.gameCanvas.pointerPressed(gX, gY);
                else if (type == 1) GameMidlet.gameCanvas.pointerDragged(gX, gY);
                else if (type == 2) GameMidlet.gameCanvas.pointerReleased(gX, gY);
                else if (type == 3) GameMidlet.gameCanvas.pointerMouse(gX, gY);
            }
            catch { }
        }

        public static void Debug(string str)
        {
            Console.WriteLine(str);
            try
            {
                File.AppendAllText("debug.txt", $"{DateTime.Now:HH:mm:ss} - {str}\n");
            }
            catch
            {
                File.Create("debug.txt").Dispose();
            }
        }
        public static void Cleanup()
        {
            GameCanvas.bRun = false;
            if (Session_ME.gI() != null) Session_ME.gI().close();
            if (Session_ME2.gI() != null) Session_ME2.gI().close();
            _isExiting = true;
            NativeMethods.PostQuitMessage(0);
            Environment.Exit(0);
        }
    }
}