using CCF.Transport;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using CCF.Shared.Exceptions;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using CCF.Shared.Http;

namespace CCF
{
    public class CCFServicesManager
    {

        private const string SITE_IP = "52.163.114.252";
        private const string SITE_PORT = "80";

        //private const string SITE_IP = "127.0.0.1";
        //private const string SITE_PORT = "5100";

        public static void RegisterService<T>(Func<T> serviceInvoker)
        {
            var result = new HttpClient().GetStringAsync($"http://{SITE_IP}:{SITE_PORT}/ip/TCPRegister/RegisterServiceProvider/{typeof(T).Name}").Result;
            var data = JObject.Parse(result).ToObject<Response>();
            ITransporter transporterCreating(string S) =>
                new TCPTransporter(SITE_IP, data.Port, S, new ConsoleLogger());
            var serviceProvider = new ServiceProvider<T>(serviceInvoker, data.Password, transporterCreating);
        }


        public static T GetService<T>() 
            where T : class
        {
            if (!typeof(T).IsInterface) throw new Exception($"type {typeof(T)} must be interface");
            var result = new HttpClient().GetStringAsync($"http://{SITE_IP}:{SITE_PORT}/ip/TCPRegister/useService?interfaceName={typeof(T).Name}").Result;
            var obj = JObject.Parse(result);
            var success = obj["success"].ToObject<bool>();
            if (!success)
                throw new ServiceUnavailableException();
            var password = obj["password"].ToObject<string>();
            var port = obj["port"].ToObject<int>();
            var serviceId = obj["id"].ToObject<int>();
            var transporter = new TCPTransporter(SITE_IP, port, password, new ConsoleLogger());
            var disposableInterface = DisposableWrapper(typeof(T));
            T worker;
            if(disposableInterface == typeof(T))
                worker = RemoteWorker.Create(disposableInterface, transporter, serviceId) as T;
            else
                worker = RemoteWorker.Create(disposableInterface, transporter, serviceId, true) as T;
            return worker;
        }
        private static Type DisposableWrapper(Type type)
        {
            if (type.GetTypeInfo().ImplementedInterfaces.Any(I => I.Equals(typeof(IDisposable)))) return type;
            var newType = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName("Some"), AssemblyBuilderAccess.Run)
                .DefineDynamicModule("Some.dll")
                .DefineType("MYBeutyInterface", TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);
            newType.AddInterfaceImplementation(type);
            newType.AddInterfaceImplementation(typeof(IDisposable));
            return newType.CreateTypeInfo().AsType();
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
