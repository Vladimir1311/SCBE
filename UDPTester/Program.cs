
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Dynamic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using SituationCenter.NotifyProtocol.Messages;

namespace UDPTester
{
    public class Program
    {
        private static readonly string RemoteUrl = File.ReadAllText("Secret.txt");
        private const string LocalUrl = "ws://localhost:60955";
        private static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    await WorkCycle();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"exception {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("Wait 4 seconds for reconnect");
                    await Task.Delay(TimeSpan.FromSeconds(4));
                }
            }
        }

        private static async Task WorkCycle()
        {
            var tokSource = new CancellationTokenSource();
            var client = new ClientWebSocket();
            Console.WriteLine($"0 for {RemoteUrl} 1 for {LocalUrl}");
            var urlNum = int.Parse(Console.ReadLine());
            var url = "";
            switch (urlNum)
            {
                case 0: url = LocalUrl;
                    break;
                case 1: url = RemoteUrl;
                    break;
                default: throw new Exception("Incorret url type");
            }
            await client.ConnectAsync(new Uri($"{url}/ws?userId=5b31fe42-93a5-40bf-bab9-e20706b98991"), tokSource.Token);
            var receiveTask = Task.Factory.StartNew(() => Receive(client, tokSource.Token), tokSource.Token);
            while (true)
            {
                var m = new Message();
                var command = Console.ReadLine();
                var splitted = command.Split(' ');
                if (splitted.Length != 2)
                    continue;
                if (!int.TryParse(splitted[0], out var type))
                    continue;
                switch (type)
                {
                    case 0:
                        await Send(client, new GenericMessage<string> { MessageType = MessageType.AddTopic, Data = splitted[1] });
                        break;
                    case 1:
                        await Send(client, new GenericMessage<string> { MessageType = MessageType.RemoveTopic, Data = splitted[1] });
                        break;
                }
            }
        }

        private static async Task Send(ClientWebSocket client, Message data)
        {
            var str = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(str);
            await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static async Task Receive(ClientWebSocket client, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var bytes = new byte[4096 * 4];
                var arraySegment = new ArraySegment<byte>(bytes);
                var received = await client.ReceiveAsync(arraySegment, token);
                if (received.MessageType == WebSocketMessageType.Close)
                    return;
                var str = Encoding.UTF8.GetString(bytes, 0, received.Count);
                Console.WriteLine($"received >>{str}<<");
            }
        }
    }
}
