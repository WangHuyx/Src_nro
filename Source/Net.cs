using System.Net.Http;
using System.Threading.Tasks;
internal class Net
{
    // Sử dụng static readonly HttpClient để tránh lỗi Socket Exhaustion (cạn kiệt cổng kết nối)
    private static readonly HttpClient _httpClient = new HttpClient();

    // Task này thay thế cho đối tượng WWW cũ
    private static Task<string> www;

    public static Command h;
    private static string _errorMessage = string.Empty;

    public static void update()
	{
		if (www != null && www.IsCompleted)
		{
			string str = string.Empty;
			if (www.Status == TaskStatus.RanToCompletion)
			{
				str = www.Result;
			}
            else if (www.IsFaulted)
            {
                // Lấy lỗi từ Exception nếu có (tương đương www.error)
                _errorMessage = www.Exception?.InnerException?.Message ?? "Unknown Error";
                //Console.WriteLine($"[NET ERROR]: {_errorMessage}");
            }
            www = null;
			if (h != null)
			{
				h.perform(str);
			}
		}
	}

	public static void connectHTTP(string link, Command h)
	{
		if (www != null && !www.IsCompleted)
        {
			Cout.LogError("GET HTTP BUSY");
			return;
		}
		Net.h = h;
        _errorMessage = string.Empty;
        www = _httpClient.GetStringAsync(link);
	}

	public static void connectHTTP2(string link, Command h)
	{
		Net.h = h;
		if (link != null)
		{
			h.perform(link);
		}
	}
}
