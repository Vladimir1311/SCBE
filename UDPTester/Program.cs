
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
        private static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/scfsnotify")
                .Build();
            connection.StartAsync().Wait();
            connection.On<Guid, string>("token", (uId, token) =>
            {
                Console.WriteLine(uId);
                Console.WriteLine(token);
            });
            connection.SendAsync("Test", "lol");
            Console.WriteLine($"started");
            Console.ReadLine();
        }
        

    }
}