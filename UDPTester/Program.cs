

using Castle.DynamicProxy;
using CCF;
using Common;
using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {
            HttpClient cl = new HttpClient();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            cl.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWFrc2FsbWFrQGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6InVzZXIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjhiYmYwMDU5LTcyZDEtNGRkYy1hNzQ4LTk3OTk1ZDE4M2Q1MiIsIm5iZiI6MTQ5OTExNjk4MCwiZXhwIjoxNDk5MTE5OTgwLCJpc3MiOiJEZW1vSXNzdWVyIiwiYXVkIjoiRGVtb0F1ZGllbmNlIn0.eTRCevKOdHOsr719Ae_mfbFXMnS3lHMqETbM2uXzOvY");
            Task[] tasks = new Task[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = cl.GetAsync("http://localhost/api/v1/rooms/roomslist");
                //cl.GetAsync("http://localhost/api/v1/rooms/roomslist").Wait();
                //tasks[i] = Task.CompletedTask;
            }
            Task.WaitAll(tasks);
            Console.WriteLine(watch.ElapsedMilliseconds);
            Console.ReadKey();
            
        }
    }
}
