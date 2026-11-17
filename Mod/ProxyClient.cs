using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NRO;
public class ProxyNode
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string User { get; set; }
    public string Pass { get; set; }

    public static ProxyNode Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        string[] parts = raw.Trim().Split(':');
        if (parts.Length < 2) return null;

        var node = new ProxyNode
        {
            Host = parts[0],
            Port = int.Parse(parts[1])
        };

        if (parts.Length >= 4)
        {
            node.User = parts[2];
            node.Pass = parts[3];
        }
        return node;
    }
}
public class ProxyClient
{
    public static string ProxyConfig = "";

    public static string host;

    public static int port;

    public static string user;

    public static string pass;

    public static bool HasProxy = false;
    private static bool SendHttpConnect(Socket socket, string targetHost, int targetPort, string user, string pass)
    {
        var sb = new StringBuilder();
        sb.AppendFormat("CONNECT {0}:{1} HTTP/1.1\r\n", targetHost, targetPort);
        sb.AppendFormat("Host: {0}:{1}\r\n", targetHost, targetPort);

        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
        {
            string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{pass}"));
            sb.AppendFormat("Proxy-Authorization: Basic {0}\r\n", auth);
        }

        sb.Append("Proxy-Connection: Keep-Alive\r\n");
        sb.Append("Connection: Keep-Alive\r\n\r\n");

        socket.Send(Encoding.ASCII.GetBytes(sb.ToString()));

        byte[] buffer = new byte[1024];
        int received = socket.Receive(buffer);
        if (received <= 0) return false;

        string response = Encoding.ASCII.GetString(buffer, 0, received);
        return response.Contains("200");
    }
    public static TcpClient Connect(string targetHost, int targetPort)
    {
        if (!HasProxy || string.IsNullOrWhiteSpace(ProxyConfig))
        {
            return new TcpClient(targetHost, targetPort);
        }
        Socket socket = null;
        try
        {
            // Tách danh sách proxy theo dấu phẩy
            string[] rawProxies = ProxyConfig.Split(',');
            var proxyList = new List<ProxyNode>();
            foreach (var item in rawProxies)
            {
                var node = ProxyNode.Parse(item);
                if (node != null) proxyList.Add(node);
            }

            if (proxyList.Count == 0)
            {
                return new TcpClient(targetHost, targetPort);
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.Connect(proxyList[0].Host, proxyList[0].Port);

            // 2. Nối chuỗi qua các Proxy trung gian
            for (int i = 0; i < proxyList.Count - 1; i++)
            {
                var currentProxy = proxyList[i];
                var nextProxy = proxyList[i + 1];

                bool success = SendHttpConnect(socket, nextProxy.Host, nextProxy.Port, currentProxy.User, currentProxy.Pass);
                if (!success)
                {
                    socket.Close();
                    return null;
                }
            }

            // 3. Yêu cầu Proxy cuối cùng kết nối tới Game Server đích
            var lastProxy = proxyList[proxyList.Count - 1];
            bool finalSuccess = SendHttpConnect(socket, targetHost, targetPort, lastProxy.User, lastProxy.Pass);
            if (!finalSuccess)
            {
                socket.Close();
                return null;
            }

            return new TcpClient { Client = socket };
        }
        catch (Exception ex)
        {
            socket?.Close();
            return null;
        }
    }
}