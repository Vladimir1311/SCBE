using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace CCF
{
    public class RemoteWorker : IInterceptor
    {
        private static IProxyGenerator proxyGenerator = new ProxyGenerator();

        private static Type[] PrimitiveTypes =
            typeof(JToken)
            .GetMethods()
            .Where(M => M.Name == "op_Implicit")
            .Select(M => M.GetParameters()[0].ParameterType)
            .ToArray();

        private string endPoint;
        private HttpClient httpClient;
        private int objectId;

        private RemoteWorker(string endPoint, int objectId)
        {
            this.endPoint = endPoint;
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(endPoint),
                Timeout = TimeSpan.FromHours(5)
            };
            this.objectId = objectId;
        }

        public static T Create<T>(string endPoint)
        {
            CheckType(typeof(T));
            return (T)proxyGenerator.CreateInterfaceProxyWithoutTarget(
                typeof(T),
                new RemoteWorker(endPoint, -1));
        }

        private static object Create(Type targetType, RemoteWorker worker)
        {
            try
            {
                return proxyGenerator.CreateInterfaceProxyWithoutTarget(
                    targetType,
                    worker);
            }
            catch
            {
                return proxyGenerator.CreateClassProxy(
                    targetType,
                    worker);
            }
        }

        private static void CheckType(Type type)
        {
            return;
            if (type.GetMembers().Count() != type.GetMethods().Count())
                throw new NotSupportedException($"Type {type.FullName} is not supported");

            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (method.ContainsGenericParameters)
                    throw new NotSupportedException($"Type {type.FullName} is not supported");
            }
        }

        public void Intercept(IInvocation invocation)
        {
            InvokeMessage message = new InvokeMessage
            {
                MethodName = invocation.Method.Name,
                SubObjectId = objectId
            };

            Dictionary<string, JToken> args = new Dictionary<string, JToken>();
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            int i = 0;
            foreach (var param in invocation.Method.GetParameters())
            {
                var argument = invocation.Arguments[i++];
                if (argument is Stream stream)
                {
                    StreamContent argContent = new StreamContent(stream);
                    multiContent.Add(argContent, param.Name, param.Name);
                }
                else
                {
                    args.Add(param.Name, JToken.FromObject(argument));
                }
            }
            message.Args = args;
            var data = JsonConvert.SerializeObject(message, Formatting.Indented);
            Console.WriteLine(data);

            StringContent content = new StringContent(data);
            multiContent.Add(content, "simpleargs");
            var result = httpClient.PostAsync("", multiContent).Result;

            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                return;
            
            if (invocation.Method.ReturnType == typeof(Stream))
            {
                invocation.ReturnValue = result.Content.ReadAsStreamAsync().Result;
                return;
            }
            var invokeResult = JsonConvert.DeserializeObject<InvokeResult>(result.Content.ReadAsStringAsync().Result);

            if (invocation.Method.ReturnType == typeof(void))
                return;
            if (invokeResult.IsPrimitive)
            {
                invocation.ReturnValue = invokeResult.Value?.ToObject(invocation.Method.ReturnType);
                return;
            }
            var worker = new RemoteWorker(endPoint, invokeResult.SubObjectId);
            invocation.ReturnValue = Create(invocation.Method.ReturnType, worker);
        }
    }
}