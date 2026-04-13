using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NRO;

public class ProxyClient
{
    public static string addressproxy;

    public static int postproxy;

    public static string accountproxy;

    public static string passproxy;

    public static bool HasProxy = false;
    public static TcpClient Connect(string targetHost, int targetPort)
    {
        string httpProxyHost = addressproxy;
        int httpProxyPort = postproxy;
        string proxyUserName = accountproxy;
        string proxyPassword = passproxy;
        TcpClient tcp = (HasProxy) ? null : new TcpClient(targetHost, targetPort);
        try
        {
            if (!HasProxy&&(string.IsNullOrWhiteSpace(httpProxyHost) || httpProxyPort <= 0)) return new TcpClient(targetHost, targetPort);
            Uri uri = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttp,
                Host = httpProxyHost,
                Port = httpProxyPort
            }.Uri;
            Uri uri2 = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttp,
                Host = targetHost,
                Port = targetPort
            }.Uri;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(httpProxyHost, httpProxyPort);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CONNECT {0}:{1} HTTP/1.1\r\n", targetHost, targetPort);
            sb.AppendFormat("Host: {0}:{1}\r\n", targetHost, targetPort);

            // Xử lý xác thực Basic Auth nếu có
            if (!string.IsNullOrEmpty(proxyUserName) && !string.IsNullOrEmpty(proxyPassword))
            {
                string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{proxyUserName}:{proxyPassword}"));
                sb.AppendFormat("Proxy-Authorization: Basic {0}\r\n", auth);
            }

            sb.Append("Proxy-Connection: Keep-Alive\r\n");
            sb.Append("Connection: Keep-Alive\r\n");
            sb.Append("\r\n");
            socket.Send(Encoding.ASCII.GetBytes(sb.ToString()));
            byte[] buffer = new byte[1024];
            int received = socket.Receive(buffer);
            if (received <= 0) return tcp;
            string response = Encoding.ASCII.GetString(buffer, 0, received);
            if (!response.Contains("200"))
            {
                socket.Close();
                return tcp;
            }
            tcp = new TcpClient { Client = socket };
        }
        catch (Exception ex)
        {
            //Console.WriteLine("ex:" + ex.Message);
            return tcp;
        }
        return tcp;
    }
}