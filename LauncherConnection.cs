using QLTK;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public class LauncherConnection
{
    private TcpClient? _client;

    private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

    public async System.Threading.Tasks.Task ConnectAsync(int port, string index)
    {
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync("127.0.0.1", port);

            using var stream = _client.GetStream();
            //using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            using var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
            using var binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
            //using var reader = new StreamReader(stream, Encoding.UTF8);

            // BƯỚC 1: Handshake - Gửi Username ngay khi kết nối
            if (int.TryParse(index, out int id))
            {
                await _sendLock.WaitAsync();
                binaryWriter.Write(id);
                binaryWriter.Flush();
                _sendLock.Release();
            }

            _ = Task.Run(async () =>
            {
                while (_client.Connected)
                {
                    await _sendLock.WaitAsync();
                    try
                    {
                        if (XuLiSettings.TrangThai != 0)
                        {
                            binaryWriter.Write((byte)4);
                            binaryWriter.Flush();
                        }
                        if (GameScr.imgCapcha != null)
                        {
                            byte[] imageBytes = GameScr.imgCapcha.texture.Bytes;
                            binaryWriter.Write((byte)1);
                            binaryWriter.Write(imageBytes.Length);
                            binaryWriter.Write(imageBytes);
                            binaryWriter.Flush();
                        }
                        if (GameCanvas.currentScreen is GameScr)
                        {
                            string send = $"{Char.myCharz().cName}-[{TileMap.zoneID}]{TileMap.mapNames[TileMap.mapID]}-{NinjaUtil.getMoneys(Char.myCharz().cPower)} SM/{NinjaUtil.getMoneys(Char.myCharz().xu)} Vàng";
                            byte[] data = Encoding.UTF8.GetBytes(send);
                            binaryWriter.Write((byte)3);
                            binaryWriter.Write(data.Length);
                            binaryWriter.Write(data);
                            binaryWriter.Flush();
                        }
                    }
                    finally
                    {
                        _sendLock.Release();
                    }
                    await Task.Delay(2500);
                }
            });
            // BƯỚC 2: Vòng lặp hứng dữ liệu từ Launcher
            while (_client.Connected)
            {
                if (_client.Available < 0 || !stream.CanRead)
                {
                    continue;
                }
                byte command = binaryReader.ReadByte();
                int length = binaryReader.ReadInt32();
                // 3. Đọc đúng 'length' byte dữ liệu ảnh
                byte[] payload = binaryReader.ReadBytes(length);
                string message = Encoding.UTF8.GetString(payload);
                if (string.IsNullOrEmpty(message)) continue;
                switch (command)
                {
                    case 0:
                        // Bước 2: Giải mã JSON thành Object AccountSetting
                        AccountSetting? settings = JsonSerializer.Deserialize(
                            message,
                            ClientJsonContext.Default.AccountSetting
                        );

                        if (settings != null)
                        {
                            //Console.WriteLine($"[SUCCESS] Đã nhận cấu hình");
                            MainMod.CurrentSetting = settings;
                        }
                        break;
                    case 1:
                        await Phacap.solvecap(message);
                        break;
                    case 2:
                        await _sendLock.WaitAsync();
                        string rawLogs = HistoryManager.GetSerializedLogs();
                        string data = rawLogs.Replace("\r", "").Replace("\n", "");
                        byte[] bytes = Encoding.UTF8.GetBytes(data);
                        binaryWriter.Write((byte)2);
                        binaryWriter.Write(bytes.Length);
                        binaryWriter.Write(bytes);
                        binaryWriter.Flush();
                        _sendLock.Release();
                        break;
                     case 4:
                        Program.Cleanup();
                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Program.Debug($"Socket Error: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            Program.Cleanup();
        }
    }

    //public string writeStatus()
    //{
    //    if (XuLiSettings.TrangThai != 0)
    //    {
    //        string str = "STATE:";
    //        switch (XuLiSettings.TrangThai)
    //        {
    //            case 1:
    //                return str + "Offline";
    //            case 2:
    //                return str + "Offline";
    //            default: 
    //                return "Offline";
    //        }

    //    }
    //    return "Offline";
    //}
}