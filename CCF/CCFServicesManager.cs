using CCF.Transport;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CCF
{
    public class CCFServicesManager
    {
        private const string SITE_IP = "52.163.250.253";
        private const string SITE_PORT = "80";

        //private const string SITE_IP = "127.0.0.1";
        //private const string SITE_PORT = "5100";

        public static void RegisterService<T>(T serviceInvoker)
        {
            var result = new HttpClient().GetStringAsync($"http://{SITE_IP}:{SITE_PORT}/ip/TCPRegister/registerService?interfaceName={typeof(T).Name}").Result;
            var obj = JObject.Parse(result);
            var password = obj["password"].ToObject<string>();
            var port = obj["port"].ToObject<int>();
            var transporter = new TCPTransporter(SITE_IP, port, password, new ConsoleLogger());
            ServiceCode code = ServiceCode.Create(transporter ,serviceInvoker);
        }


        public static T GetService<T>()
        {
            var result = new HttpClient().GetStringAsync($"http://{SITE_IP}:{SITE_PORT}/ip/TCPRegister/useService?interfaceName={typeof(T).Name}").Result;
            var obj = JObject.Parse(result);
            var password = obj["password"].ToObject<string>();
            var port = obj["port"].ToObject<int>();
            var transporter = new TCPTransporter(SITE_IP, port, password, new ConsoleLogger());
            T worker = RemoteWorker.Create<T>(transporter);
            return worker;
        }

        private class ConsoleLogger : ILogger<TCPTransporter>
        {
            public IDisposable BeginScope<TState>(TState state) => new Disposable();

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Console.WriteLine($"{logLevel} : {formatter(state, exception)}");
            }
            private class Disposable : IDisposable
            {
                public void Dispose()
                {}
            }
        }
    }
}
