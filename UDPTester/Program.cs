using Castle.DynamicProxy;
using CCF;
using System;
using System.Dynamic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;

namespace UDPTester
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var inp = Console.ReadLine();
            if (inp == "SP")
            {

                CCFServicesManager.RegisterService<IRemoteWorker>(() => new RemoteWorker());
                Console.WriteLine("Registered...");
                Console.ReadLine();
            }
            else
            if (inp == "C")
            {

                var client = CCFServicesManager.GetService<IRemoteWorker>();
                Console.WriteLine("Getted invoker");
                Console.ReadLine();

                var value = 5.4d;
                Console.WriteLine($"method Value with {value}");
                Console.WriteLine(client.Value(value));
                Console.ReadLine();

                var codeToJson = "Hi! I есть гр4т";
                var str = client.CodeToJson(codeToJson);
                Console.WriteLine($"receive stream length {str.Length}");
                using (var reader = new StreamReader(str))
                    Console.WriteLine(reader.ReadToEnd());

                var streamInfo = new MemoryStream(new byte[] { 1, 2, 3, 4, 5});
                Console.WriteLine(client.StreamInfo(streamInfo));
                Console.ReadLine();

                var not = new N();
                client.HardWork(not);
                Console.WriteLine("Invoked magic");

            }
            Console.WriteLine("General end");


            Console.ReadLine();

        }

        class N : INotifyer
        {
            public void Notify(string message)
            {
                Console.WriteLine("THIS IS MESSAGE FROM SERVICE!");
                Console.WriteLine(message);
                Console.WriteLine("THIS IS MESSAGE FROM SERVICE!");
                Console.WriteLine(message);
                Console.WriteLine("THIS IS MESSAGE FROM SERVICE!");
                Console.WriteLine(message);
                Console.WriteLine("THIS IS MESSAGE FROM SERVICE!");
                Console.WriteLine(message);
            }
        }

    }
}