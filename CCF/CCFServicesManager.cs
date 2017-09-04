using CCF.Transport;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CCF
{
    public class CCFServicesManager
    {
        public static void RegisterService<T>(T serviceInvoker)
        {
            var result = new HttpClient().GetStringAsync($"http://127.0.0.1:53661/api/TCPRegister/registerService?interfaceName={typeof(T).Name}").Result;
            var obj = JObject.Parse(result);
            var password = obj["password"].ToObject<string>();
            var port = obj["port"].ToObject<int>();
            var transporter = new TCPTransporter("127.0.0.1", port, password);
            ServiceCode code = ServiceCode.Create(transporter ,serviceInvoker);
        }


        public static T GetService<T>()
        {
            var result = new HttpClient().GetStringAsync($"http://127.0.0.1:53661/api/TCPRegister/useService?interfaceName={typeof(T).Name}").Result;
            var obj = JObject.Parse(result);
            var password = obj["password"].ToObject<string>();
            var port = obj["port"].ToObject<int>();
            var transporter = new TCPTransporter("127.0.0.1", port, password);
            T worker = RemoteWorker.Create<T>(transporter);
            return worker;
        }
    }
}
