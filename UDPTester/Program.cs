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

namespace UDPTester
{
    public class Program
    {
        static void Main(string[] args)
        {
            ProxyGenerator generator = new ProxyGenerator();
            ILOL t = (ILOL)generator.CreateInterfaceProxyWithoutTarget(typeof(ILOL), new Worker());
            t.StrLength("sdfsdfsgsrg");
        }
    }

    class Worker : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = "sdfsfe";
        }
    }


    public interface ILOL
    {
        int StrLength(string str);
    }
}
