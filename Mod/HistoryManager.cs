using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static NRO.AutoMap;

namespace NRO;
public class HistoryManager
{
    private static readonly Queue<string> _logs = new Queue<string>();
    private const int MaxRecords = 100;

    public static void AddLog(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        _logs.Enqueue($"[{timestamp}] {message}");

        // Tự động đẩy bản ghi cũ nhất nếu vượt quá giới hạn
        while (_logs.Count > MaxRecords)
        {
            _logs.Dequeue();
        }
    }
    public static string GetSerializedLogs()
    {
        return JsonSerializer.Serialize(_logs.ToList(), AppConfigJsonContext.Default.ListString);
    }
}
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AutoMap.MapData))] // Thêm Class mà AutoMap đang dùng vào đây
[JsonSerializable(typeof(List<string>))]
internal partial class AppConfigJsonContext : JsonSerializerContext { }