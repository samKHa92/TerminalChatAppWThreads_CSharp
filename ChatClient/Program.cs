using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), 5003);
            Console.WriteLine("You have successfully connected to the server!");
            NetworkStream ns = client.GetStream();
            Thread th = new Thread(o => RecieveData((TcpClient)o));
            th.Start(client);

            string inp = string.Empty;
            while (inp != "Exit")
            {
                inp = Console.ReadLine();
                byte[] BufferStream = Encoding.ASCII.GetBytes(inp);
                ns.Write(BufferStream, 0, BufferStream.Length);
            }

            client.Client.Shutdown(SocketShutdown.Send);
            th.Join();
            ns.Close();
            client.Close();
            Console.WriteLine("You have disconnected from the server!");

        }

        public static void RecieveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] ReceivedBytes = new byte[2048];
            int ReceivedBytesLengh;

            while ((ReceivedBytesLengh = ns.Read(ReceivedBytes, 0, ReceivedBytes.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(ReceivedBytes, 0, ReceivedBytesLengh));
            }
        }
    }
}
