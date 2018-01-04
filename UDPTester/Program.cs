using Castle.DynamicProxy;
using CCF;
using System;
using System.Dynamic;
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

                Console.WriteLine(client.Value(5.4d));
                Console.ReadLine();
            }
            Console.WriteLine("General end");
            Console.ReadLine();

        }

    }
}