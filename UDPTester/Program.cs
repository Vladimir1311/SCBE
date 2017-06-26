

using Castle.DynamicProxy;
using CCF;
using Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPTester
{
    public class Program
    {
        public interface ILOL
        {
            void Val();
            string WithReturn();
            int Prop { get; set; }
        }
        private static object locker = new object();
        static void Main(string[] args)
        {

            ILOL loler = RemoteWorker<ILOL>.Create("http://localhost");
            loler.Val();
            loler.Prop += 1;

            return;
            //Console.WriteLine("Start");
            //   if (args.Length != 0)
            //   {
            //       Console.WriteLine("Get port " + args[0]);
            //       TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(args[0]));
            //       listener.Start();
            //       Console.WriteLine("Started listening");
            //       while (true)
            //       {
            //           var client = listener.AcceptTcpClientAsync().Result;
            //           Console.WriteLine($"Client {client.Client.RemoteEndPoint.ToString()}. Waiting for data.");
            //           byte[] buffer = new byte[1024];
            //           client.ReceiveTimeout = 50;
            //           try
            //           {
            //               var readed = client.GetStream().Read(buffer, 0, buffer.Length);
            //               Console.WriteLine($"Readed {readed} bytes");
            //               client.GetStream().Write(buffer, 0, readed);
            //               Console.WriteLine($"Sended test {readed} bytes");
            //           }
            //           catch (Exception ex)
            //           {
            //               Console.WriteLine(ex.Message);
            //           } 
            //           Console.WriteLine("Closing connection.");
            //           client.Dispose();
            //       }
            //   }
            //   return;
            Class1.Main(new string[0]);
            return;
            if (args.Length != 0)
            {
                UdpClient receivingUdpClient = new UdpClient(int.Parse(args[0]));
                Console.WriteLine($"Start listenung on {args[0]} port");
                while (true)
                {
                    try
                    {
                        var recieve = receivingUdpClient.ReceiveAsync().Result;
                        Console.WriteLine($"Receive {recieve.Buffer.Length} bytes from {recieve.RemoteEndPoint.Address}:{recieve.RemoteEndPoint.Port}");
                        byte[] receiveBytes = recieve.Buffer;
                        receivingUdpClient.SendAsync(receiveBytes, receiveBytes.Length, recieve.RemoteEndPoint).Wait();
                        Console.WriteLine($"Sended bytes back");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
            else
            {
                IPEndPoint point = new IPEndPoint(new IPAddress(new byte[] { 52, 187, 24, 208 }), 11000);
                UdpClient sender = new UdpClient();
                int i = 40000;
                int totalbytes = 0;
                Stopwatch timer = new Stopwatch();
                timer.Start();
                while (true)
                {
                    Console.WriteLine($"Sending {i} ");
                    sender.SendAsync(new byte[i], i, point).Wait();
                    var receive = sender.ReceiveAsync().Result;
                    if (receive.Buffer.Length == i)
                        Console.WriteLine("S");
                    else
                        Console.WriteLine("F");
                    totalbytes += i + receive.Buffer.Length;
                    Console.WriteLine($"In/Out : {totalbytes / (timer.Elapsed.Seconds == 0 ? 1 : timer.Elapsed.Seconds)} bytes per second");
                    i += 10;
                }
            }

        }
    }
}
