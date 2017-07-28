

using Castle.DynamicProxy;
using CCF;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var o = new object[] { "Aha", 123, 123.5 };
            var serialized = o.Select(O => JToken.FromObject(O)).ToArray();
            var arrTok = JToken.FromObject(o);
            var arrReader = new JTokenReader(arrTok);
            while (arrReader.Read())
            {
                Console.WriteLine("---");
                Console.WriteLine(arrReader.ValueType);
            }
            foreach (var item in serialized)
            {
                JsonReader reader = new JTokenReader(item);
                Console.WriteLine(reader.Read());
                Console.WriteLine(reader.ValueType.Name);
            }


            IMyService service = RemoteWorker<IMyService>.Create("http://127.0.0.1");
            Console.WriteLine("created");
            var l = service.GetStringLength("ahahahaha", 54, 12.54);
            Console.WriteLine("Executed "+ l);
            Console.ReadKey();
            
        }



    }

    public interface IMyService
    {
        int GetStringLength(string str, int value, double lol);
    }
}
