using Castle.DynamicProxy;
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
using CCF;
namespace UDPTester
{
    public class Program
    {
        static void Main(string[] args)
        {
            CCFServicesManager.RegisterService(new lol() as ILOL);

            var service = CCFServicesManager.GetService<ILOL>();
            Console.WriteLine(service.StrLength("Hello world!"));
        }
    }

    class lol : ILOL
    {
        public int StrLength(string str) => str.Length;
    }
    public interface ILOL
    {
        int StrLength(string str);
    }
}
