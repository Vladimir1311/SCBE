using Castle.DynamicProxy;
using CCF;
using DocsToPictures.Models;
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
        
        private static void Main(string[] args)
        {
            var proccessor = new DocumentProcessor();
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