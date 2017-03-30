

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
            UdpClient receivingUdpClient = new UdpClient(11000);
            int total = 0;
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