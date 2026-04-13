using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace NRO
{
    public class TcpConnect
    {
        public static bool StartConnect;
        public static IPEndPoint IP;
        public static Stream stream;
        public static Socket client;
        public int port; 
        public void Connect()
        {
            try
            {
                port = int.Parse(File.ReadAllText("Data\\port.ini"));
            }
            catch
            {
                port = 9050;
            }
            try
            {
                IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IP);
                Thread Listen = new Thread(Recieve);
                Listen.IsBackground = true;
                Listen.Start();
            }
            catch
            {
                GameScr.info1.addInfo("Chưa mở bảng điều khiển", 0);
                MainMod.isConnectToAccountManager = false;
            }

        }
        public void Recieve()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024];
                    client.Receive(data);
                    string Mess = (string)DeSerialize(data);
                    MainMod.GetMessServer = Mess;
                }
            }
            catch
            {
                MainMod.isConnectToAccountManager = false;                
                CloseClient();
            }
        }
        public void CloseClient()
        {
            client.Close();
        }
        object DeSerialize(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
