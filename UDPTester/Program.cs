

using Castle.DynamicProxy;
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
                IPEndPoint point = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1}), 11000);
                UdpClient sender = new UdpClient();
                while (true)
                {
                    Console.WriteLine("Write line for send");
                    var toSend = Encoding.UTF8.GetBytes(Console.ReadLine());
                    sender.SendAsync(toSend, toSend.Length, point).Wait();
                    var receive = sender.ReceiveAsync().Result;
                    Console.WriteLine($"Receive {receive.Buffer.Length} bytes from {receive.RemoteEndPoint.Address}:{receive.RemoteEndPoint.Port}");
                }
            }

        }
    }
}
            //var proxy = new ProxyGenerator()
            //    .CreateInterfaceProxyWithoutTarget<IRemoteWorker>(new ProxyClass());
            //Console.WriteLine($"Result is : {proxy.Value(4.5)}");

            //var alalala = Activator.CreateInstance<IRemoteWorker>();



            //return;
