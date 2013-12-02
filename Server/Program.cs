using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class Server
    {
        private const int port = 9595;
        public const int bufferSize = 1024;
        public static List<Connection> threads;

        static void Main(string[] args)
        {
            new Server();
        }

        public Server()
        {
            threads = Factory.getThreads(2);
            startListen();
        }

        private void startListen()
        {
            TcpListener Listener = new TcpListener(IPAddress.Any, port);
            Listener.Start();

            while (true)
            {
                while (!Listener.Pending() || threads.Count == 0)
                {
                    Thread.Sleep(1000);
                }
                Console.WriteLine("Thread is working now");
                getFreeThread().setClientAndStart(Listener);
            }
        }

        private Connection getFreeThread()
        {
            return threads[0];
        }

        public class Connection : IDisposable
        {
            TcpClient client;
            NetworkStream stream;
            static int numConnections = 0;
            Thread t;
            volatile Boolean isWork = false;

            public Connection()
            {
                t = new Thread(new ThreadStart(StartRead));
                t.IsBackground = true;
                t.Start();
            }

            public void setClientAndStart(TcpListener L)
            {
                try
                {
                    threads.Remove(this);
                    numConnections++;
                    Console.WriteLine("{0} active connections", numConnections.ToString());
                    client = L.AcceptTcpClient();
                    stream = client.GetStream();
                    isWork = true;
                }
                catch
                {
                    Dispose();
                }
            }

            public void StartRead()
            {
                while (true)
                {
                    while (!isWork)
                    {
                        Thread.Sleep(5000);
                    }

                    try
                    {
                        while (true)
                        {
                            byte[] byteData = new byte[Server.bufferSize];
                            int bytesRead = stream.Read(byteData, 0, byteData.Length);
                            string InputData = Encoding.Default.GetString(byteData, 0, bytesRead);
                            Console.WriteLine(InputData);
                        }
                    }
                    catch
                    {
                        Dispose();
                    }
                }
            }

            public void Dispose()
            {
                stream.Close();
                client.Close();
                numConnections--;
                Console.WriteLine("{0} active connections", numConnections.ToString());
                isWork = false;
                threads.Add(this);
            }
        }
    }

    public class Factory
    {
        public static List<Server.Connection> getThreads(int count)
        {
            List<Server.Connection> connections = new List<Server.Connection>();
            for (int i = 0; i < count; i++)
            {
                Server.Connection newConnection = new Server.Connection();
                connections.Add(newConnection);
            }

            return connections;
        }
    }
}