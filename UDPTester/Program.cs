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
using Microsoft.AspNetCore.SignalR.Client;

namespace UDPTester
{
    public class Program
    {
        static IServiceProvider provider;
        
        private static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5000/scfs")
                .Build();
            connection.StartAsync().Wait();
            connection.On<Guid, string>("token", (uId, token) =>
            {
                Console.WriteLine(uId);
                Console.WriteLine(token);
            });
        }
        

    }
}