using Castle.DynamicProxy;
using CCF;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        static IServiceProvider provider;
        static Program()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new LoggerFactory()
                .AddConsole());
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(CCFServicesManager.Params.LocalHost);
            serviceCollection.AddSingleton<CCFServicesManager>();
            provider = serviceCollection.BuildServiceProvider();
        }
        private static void Main(string[] args)
        {
            var ccfManager = provider.GetService<CCFServicesManager>();
            var inp = Console.ReadLine();
            if (inp == "SP")
            {

                ccfManager.RegisterService<IRemoteWorker>(() => new RemoteWorker());
                Console.WriteLine("Registered...");
                Console.ReadLine();
            }
            else
            if (inp == "C")
            {

                var client = ccfManager.GetService<IRemoteWorker>();
                //Console.WriteLine("Getted invoker");
                //Console.ReadLine();

                //var value = 5.4d;
                //Console.WriteLine($"method Value with {value}");
                //Console.WriteLine(client.Value(value));
                //Console.ReadLine();

                //var codeToJson = "Hi! I есть гр4т";
                //var str = client.CodeToJson(codeToJson);
                //Console.WriteLine($"receive stream length {str.Length}");
                //using (var reader = new StreamReader(str))
                //    Console.WriteLine(reader.ReadToEnd());

                //var streamInfo = new MemoryStream(new byte[] { 1, 2, 3, 4, 5});
                //Console.WriteLine(client.StreamInfo(streamInfo));
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