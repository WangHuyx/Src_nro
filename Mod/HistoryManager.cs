using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static NRO.AutoMap;
using static NRO.HistoryManager;

namespace NRO;
public class HistoryManager
{
    // Model lưu trữ trong RAM
    public class LogItemData
    {
        public string Name { get; set; } = string.Empty;
        public SortedDictionary<string, int> History { get; set; } = []; // Khởi tạo C# 12
    }

    // DTO xuất ra JSON cho Client/File
    public class LogExportDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SortedDictionary<string, int> History { get; set; } = [];
    }

    private static readonly Dictionary<int, LogItemData> _groupedLogs = [];
    private static int _totalLogs = 0; // Biến đếm O(1)
    private static int MaxRecords = 100;

    public static void AddLog(int id, int quantity, string name)
    {
        if (quantity == 0) quantity = 1;
        string timestamp = DateTime.Now.ToString("HH:mm");

        // 1. Đảm bảo tồn tại ID
        if (!_groupedLogs.TryGetValue(id, out var logData))
        {
            logData = new LogItemData { Name = name };
            _groupedLogs[id] = logData;
        }

        // 2. Cộng dồn số lượng
        if (logData.History.ContainsKey(timestamp))
        {
            logData.History[timestamp] += quantity;
        }
        else
        {
            logData.History[timestamp] = quantity;
            _totalLogs++; // Chỉ tăng tổng số record khi sinh ra mốc thời gian mới
        }

        // 3. Giới hạn bộ nhớ (Tối ưu)
        if (_totalLogs > MaxRecords)
        {
            var firstId = _groupedLogs.Keys.First();
            var oldestTimestamp = _groupedLogs[firstId].History.Keys.First(); // SortedDictionary đảm bảo Key đầu tiên là giờ nhỏ nhất
        
            _groupedLogs[firstId].History.Remove(oldestTimestamp);
            _totalLogs--;

            // Dọn dẹp Key nếu History rỗng để giải phóng RAM
            //if (_groupedLogs[firstId].History.Count == 0)
            //{
            //    _groupedLogs.Remove(firstId);
            //}
        }
    }
    public static string GetSerializedLogs()
    {
        List<string> exportData = []; // Khởi tạo mảng string

        foreach (var kvp in _groupedLogs)
        {
            int id = kvp.Key;
            string name = kvp.Value.Name;

            // Gom lịch sử (History) thành chuỗi dạng: "12:22: 1, 12:23: 5"
            string timeHistory = string.Join(", ", kvp.Value.History.Select(h => $"{h.Key}:x{h.Value}"));
            //Gom tổng số lượng(quantity)
            //int totalQuantity = kvp.Value.History.Values.Sum();
            // Tạo chuỗi định dạng cho UI và đẩy vào list
            exportData.Add($"[{id}]{name}: [{timeHistory}]");
        }

        // Serialize trực tiếp List<string>, UI cũ sẽ nhận được ngay mảng chuỗi
        return JsonSerializer.Serialize(exportData, AppConfigJsonContext.Default.ListString);
    }
}
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AutoMap.MapData))] // Thêm Class mà AutoMap đang dùng vào đây
[JsonSerializable(typeof(List<string>))]
internal partial class AppConfigJsonContext : JsonSerializerContext { }