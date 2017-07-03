using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Services
{
    public class FileConfigsManager : IConfigsManager
    {

        private static object locker = new object();
        public string CoreIP
        {
            get => ReadCoreIP();
            set => SaveCoreIP(value);
        }


        private string ReadCoreIP()
        {
            lock(locker)
            {
                var obj = JObject.Parse(File.ReadAllText("settings.json"));
                return obj.GetValue("coreIp").ToString();
            }
        }

        private void SaveCoreIP(string newIP)
        {
            lock (locker)
            {
                var newval = JsonConvert.SerializeObject(new { coreIp = newIP});
                File.WriteAllText("settings.json", newval);
            }
        }
    }
}
