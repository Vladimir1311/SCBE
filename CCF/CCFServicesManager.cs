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
using System.Threading.Tasks;

namespace CCF
{
    public class CCFServicesManager
    {
        private readonly string hostName;
        private readonly string port;
        private readonly ILogger<CCFServicesManager> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly HttpClient httpClient = new HttpClient();


        public CCFServicesManager(ILoggerFactory loggerFactory, Params @params)
        {
            logger = loggerFactory.CreateLogger<CCFServicesManager>();
            this.loggerFactory = loggerFactory;
            (hostName, port) = (@params.HostName, @params.Port);
        }


        public async void RegisterService<T>(Func<T> serviceInvoker)
        {
            try
            {
                ITransporter transporterCreating(string password)
                {
                    var port = GetPort();
                    logger.LogDebug($"TCP Connection to port {port}");
                    return new TCPTransporter(hostName, port, password, loggerFactory.CreateLogger<TCPTransporter>());
                }
                ITransporter providerTransporterCreating()
                {
                    var result = httpClient.GetAsync($"http://{hostName}:{port}/ip/TCPRegister/RegisterServiceProvider/{typeof(T).Name}").Result;
                    logger.LogDebug($"receved result from HTTP request {result.StatusCode}");
                    var data = JObject.Parse(result.Content.ReadAsStringAsync().Result).ToObject<Response>();
                    return new TCPTransporter(hostName, data.Port, data.Password, loggerFactory.CreateLogger<TCPTransporter>());
                }
                var serviceProvider = new ServiceProvider<T>(serviceInvoker,
                    providerTransporterCreating,
                    transporterCreating,
                    loggerFactory.CreateLogger<ServiceProvider<T>>());
            }
            catch (HttpRequestException)
            {
                logger.LogWarning("Error while sending request, try to reconnect");
                var newTry = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    RegisterService(serviceInvoker);
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unexpected error");
                throw;
            }
        }


        public T GetService<T>()
            where T : class
        {
            if (!typeof(T).IsInterface) throw new Exception($"type {typeof(T)} must be interface");
            var result = httpClient.GetStringAsync($"http://{hostName}:{port}/ip/TCPRegister/ConnectoToService/{typeof(T).Name}").Result;
            var data = JObject.Parse(result).ToObject<Response>();
            if (!data.Success)
                throw new ServiceUnavailableException();
            var transporter = new TCPTransporter(hostName, data.Port, data.Password, loggerFactory.CreateLogger<TCPTransporter>());
            var disposableInterface = DisposableWrapper(typeof(T));
            T worker;
            if (disposableInterface == typeof(T))
                worker = RemoteWorker.Create(disposableInterface, transporter, 0) as T;
            else
                worker = RemoteWorker.Create(disposableInterface, transporter, 0, true) as T;
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

        private int GetPort()
        {
            var result = httpClient.GetStringAsync($"http://{hostName}:{port}/ip/TCPRegister/GetPort").Result;
            var data = JObject.Parse(result).ToObject<Response>();
            return data.Port;
        }


        public class Params
        {
            public Params(string hostName, string ip)
            {
                HostName = hostName;
                Port = ip;
            }

            public string HostName { get; }
            public string Port { get; }

            public static Params Default => new Params("13.79.225.57", "80");
            public static Params LocalHost => new Params("127.0.0.1", "5100");
        }
    }
}
