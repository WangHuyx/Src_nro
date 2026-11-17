
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class LauncherConnection
{
    private TcpClient? _client;

    public async System.Threading.Tasks.Task ConnectAsync(int port, string index)
    {
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync("127.0.0.1", port);

            using var stream = _client.GetStream();
            using var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
            using var binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

            // BƯỚC 1: Handshake - Gửi Username ngay khi kết nối
            if (int.TryParse(index, out int id))
            {
                binaryWriter.Write(id);
                binaryWriter.Flush();
            }

            _ = Task.Run(async () =>
            {
                while (_client.Connected)
                {
                    if (XuLiSettings.TrangThai != 0)
                    {
                        string send = "offline";
                        byte[] data = Encoding.UTF8.GetBytes(send);
                        binaryWriter.Write((byte)4);
                        binaryWriter.Write(data.Length);
                        binaryWriter.Write(data);
                        binaryWriter.Flush();
                    }
                    
                    if (GameCanvas.currentScreen is GameScr)
                    {
                        if (GameScr.imgCapcha != null)
                        {
                            byte[] imageBytes = GameScr.imgCapcha.texture.Bytes;
                            binaryWriter.Write((byte)1);
                            binaryWriter.Write(imageBytes.Length);
                            binaryWriter.Write(imageBytes);
                            binaryWriter.Flush();
                        }
                        string send = GetStatusText();
                        byte[] data = Encoding.UTF8.GetBytes(send);
                        binaryWriter.Write((byte)3);
                        binaryWriter.Write(data.Length);
                        binaryWriter.Write(data);
                        binaryWriter.Flush();
                    }
                    await Task.Delay(2500);
                }
            });
            // BƯỚC 2: Vòng lặp hứng dữ liệu từ Launcher
            while (_client.Connected)
            {
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
                            MainMod.CurrentSetting = settings;
                        }
                        break;
                    case 1:
                        await Phacap.solvecap(message);
                        break;
                    case 2:
                        string rawLogs = HistoryManager.GetSerializedLogs();
                        string data = rawLogs/*.Replace("\r", "").Replace("\n", "")*/;
                        byte[] bytes = Encoding.UTF8.GetBytes(data);
                        binaryWriter.Write((byte)2);
                        binaryWriter.Write(bytes.Length);
                        binaryWriter.Write(bytes);
                        binaryWriter.Flush();
                        break;
                     case 4:
                        Program.Cleanup();
                        break;
                    case 5:
                        Program.PaintCanvas = !Program.PaintCanvas;
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

    private static string GetStatusText()
    {
        var setting = MainMod.CurrentSetting;
        string textout = $"{Char.myCharz().cName}:{NinjaUtil.getMoneys(Char.myCharz().cPower)} SM\n";
        textout += $" [{TileMap.zoneID}]{TileMap.mapNames[TileMap.mapID]} (X-Y: {Char.myCharz().cx}-{Char.myCharz().cy}\n";
        textout += $"{Res.formatNumber2(Char.myCharz().xu)} Vàng/{Char.myCharz().luong} ngọc/{Char.myCharz().luongKhoa} ngọc hồng\n";
        textout += $"HP: {Char.myCharz().cHP}/{Res.formatNumber2(Char.myCharz().cHPFull)} - MP: {Char.myCharz().cMP}/{Res.formatNumber2(Char.myCharz().cMPFull)}\n";
        textout += $"Thể lực: {NinjaUtil.getMoneys(Char.myCharz().cStamina)}/{NinjaUtil.getMoneys(Char.myCharz().cMaxStamina)} ({(double)(Char.myCharz().cStamina*100 /Char.myCharz().cMaxStamina)}%)\n";
        textout += $"Vàng nhặt:{NinjaUtil.getMoneys(Char.myCharz().xu-MainMod.save_gold)} trong {NinjaUtil.getTimeAgo((int)XuLiSettings.ThoiGianDaHoatDong.TotalSeconds)}\n";
        textout += $"Trung bình vàng/h: {XuLiSettings.TinhTrungBinhTrenGio(Char.myCharz().xu - MainMod.save_gold)}\n";
        if (setting.IsOutMaxSM)
        {
            textout += $"thoát khi:{Res.formatNumber(Char.myCharz().cPower)}/{Res.formatNumber(setting.MaxSM)}SM";
        }
        if (setting.IsOutMaxTime)
        {
            textout += $"không nhặt được vật phẩm thoát khi {XuLiSettings.CountTimeOut}/{setting.MaxTime}s";
        }
        if (setting.IsOutMaxItem)
        {
            textout += $"Số lượng nhặt vật phẩm:\n{XuLiSettings.Get_listQuantity()}";
        }
        return textout;
    }
}