using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace CCF
{
    public class RemoteWorker<T> : IInterceptor
    {
        private static IProxyGenerator proxyGenerator = new ProxyGenerator();


        private string endPoint;
        private HttpClient httpClient;


        private RemoteWorker(string endPoint)
        {
            this.endPoint = endPoint;
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(endPoint)
            };
        }

        public static T Create(string endPoint)
        {
            CheckType(typeof(T));
            return (T)proxyGenerator.CreateInterfaceProxyWithoutTarget(
                typeof(T),
                new RemoteWorker<T>(endPoint));
        }
        private static Type[] acceptedTypes = new Type[]
        {
            typeof(int),
            typeof(double),
            typeof(long),
            typeof(string),
            typeof(Guid)
        };
        private static void CheckType(Type type)
        {
            if (type.GetMembers().Count() != type.GetMethods().Count())
                throw new NotSupportedException($"Type {type.FullName} is not supported");

            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (method.ContainsGenericParameters)
                    throw new NotSupportedException($"Type {type.FullName} is not supported");
                if (method.GetParameters().Any(I => !acceptedTypes.Contains(I.ParameterType)))
                    throw new NotSupportedException($"Type {type.FullName} is not supported");
                if (!acceptedTypes.Contains(method.ReturnType))
                    throw new NotSupportedException($"Type {type.FullName} is not supported");
            }
        }

        public void Intercept(IInvocation invocation)
        {
            InvokeMessage message = new InvokeMessage
            {
                MessageId = Guid.NewGuid(),
                Args = JToken.FromObject(
                    invocation.Arguments.ToArray()),
                MethodName = invocation.Method.Name
            };
            var data = JsonConvert.SerializeObject(message);
            Console.WriteLine(data);

            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            StringContent content = new StringContent(data);
            multiContent.Add(content, "simpleargs");
            var res = httpClient.PostAsync("CCF/Recieve", multiContent).Result.Content.ReadAsStringAsync().Result;
            invocation.ReturnValue = int.Parse(res);
        }
    }
}
