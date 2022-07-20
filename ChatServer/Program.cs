using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static readonly Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
        static readonly object _lock = new object();
        public static void Main(string[] args)
        {
            int IdCounter = 1;
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5003);
            ServerSocket.Start();

            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) clients.Add(IdCounter, client);
                Console.WriteLine("Client " + IdCounter + " Joined");

                Thread th = new Thread(HandleClients);
                th.Start(IdCounter);
                IdCounter++;
            }
        }

        public static void HandleClients(object o)
        {
            int ClientId = (int)o;
            TcpClient client;

            lock (_lock) client = clients[ClientId];

            while(true)
            {
                NetworkStream stream = client.GetStream();
                byte[] BufferStream = new byte[2048];
                int BufferStreamLength = stream.Read(BufferStream, 0, BufferStream.Length);

                if(BufferStreamLength == 0)
                {
                    break;
                }
                string data = Encoding.ASCII.GetString(BufferStream, 0, BufferStreamLength);
                HandleBroadcast(data);
                Console.WriteLine(data);
            }


        }

        public static void HandleBroadcast(string data)
        {
            byte[] BufferStream = Encoding.ASCII.GetBytes(data + Environment.NewLine);
            lock (_lock)
            {
                foreach (TcpClient tcpclient in clients.Values)
                {
                    NetworkStream stream = tcpclient.GetStream();
                    stream.Write(BufferStream, 0, BufferStream.Length);
                }
            }
        }
    }
}
