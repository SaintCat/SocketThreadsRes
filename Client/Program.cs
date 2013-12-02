using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
namespace Client
{
    class Client
    {
        const int bufferSize = 512;
        static int id;
        static byte[] byteData = new byte[bufferSize];
        static NetworkStream Stream;
        static System.Net.IPAddress IP = System.Net.IPAddress.Parse("127.0.0.1");
        const int port = 9595;

        static void Main(string[] args)
        {
            id = new Random().Next(100);

        tryConnect:
            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(IP, port);

                Stream = tcpClient.GetStream();
                while (true)
                {
                    Console.WriteLine(id + " " + DateTime.Now.ToString());
                    Write(id + " " + DateTime.Now.ToString());
                    Thread.Sleep(5000);
                }

            }
            catch
            {
                Console.WriteLine("[X] Server not available!");
                Thread.Sleep(3000);
                goto tryConnect;
            }
        }
        static void Write(string s)
        {
            try
            {
                byteData = new byte[bufferSize];
                byteData = Encoding.Default.GetBytes(s);
                Stream.Write(byteData, 0, byteData.Length);
                Thread.Sleep(100);
            }
            catch
            {
                Console.WriteLine("[X] Server not available!");
            }
        }
    }
}