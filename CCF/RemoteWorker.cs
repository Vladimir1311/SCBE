using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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

        private static void CheckType(Type type)
        {
            ;
        }

        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name);
            invocation.ReturnValue = 1;
        }
    }
}
