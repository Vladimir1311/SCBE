using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UDPTester
{
    public class RemoteWorker : IRemoteWorker
    {
        public Stream CodeToJson(string value)
            => new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { val = value })));

        public int HardWork(INotifyer notifyer)
        {
            notifyer.Notify("AZAZAZAZAZAZAZAZAZAZ Other machine :)");
            return 5;
        }

        public string StreamInfo(Stream param)
            => $"Length - {param.Length}";

        public int Value(double arg)
        {
            Console.WriteLine($"i write {arg}");
            return (int)arg;
        }
    }
}
