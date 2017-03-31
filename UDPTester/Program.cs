

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDPTester
{
    class Program
    {
        static void Main(string[] args)
        {
	    Console.WriteLine("Start");
            if (args.Length != 0)
            {
                TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(args[0]));
                listener.Start();
                while (true)
                {
                    var client = listener.AcceptTcpClientAsync().Result;
                    Console.WriteLine($"Client {client.Client.RemoteEndPoint.ToString()}. Waiting for data.");
                    byte[] buffer = new byte[1024];
                    var readed = client.GetStream().Read(buffer, 0, buffer.Length);
                    Console.WriteLine($"Readed {readed} bytes");
                    client.GetStream().Write(buffer, 0, readed);
                    Console.WriteLine($"Sended test {readed} bytes");
                    Console.WriteLine("Closing connection.");
                    client.GetStream().Dispose();
                }

            }








            return;
            UdpClient receivingUdpClient = new UdpClient(11000);
            int total = 0;
            receivingUdpClient.SendAsync(new byte[] { 1, 2, 3, 4, 5}, 5, "13.84.55.187", 12000);
            //return;
            DateTime last = DateTime.Now;
            while (true)
            {
                try
                {
                    if ((DateTime.Now - last).Seconds > 1)
                    {
                        Console.WriteLine($"{total} bytes per second");
                        last = DateTime.Now;
                        total = 0;
                    }
                    var recieve = receivingUdpClient.ReceiveAsync().Result;

                    Byte[] receiveBytes = recieve.Buffer;
                    total += receiveBytes.Length;
                    //for (int i = 0; i < receiveBytes.Length - 1; i +=  2)
                    //{
                    //    var sred = (receiveBytes[i] + receiveBytes[i + 1]) / 2;
                    //    receiveBytes[i] = (byte)sred;
                    //    receiveBytes[i + 1] = (byte)sred;
                    //}
                    var a = receivingUdpClient.SendAsync(receiveBytes, receiveBytes.Length, recieve.RemoteEndPoint.Address.ToString(), 15000).Result;
                    //Console.WriteLine(a);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }
    }
}
