using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using CCF.Transport;
using System.Threading;
using System.Collections.Concurrent;
using CCF.Shared.Exceptions;
using System.Threading.Tasks;

namespace CCF
{
    public class RemoteWorker : IInterceptor
    {
        private static IProxyGenerator proxyGenerator = new ProxyGenerator();

        private class WaitPair
        {
            public ManualResetEvent ResetEvent { get; set; }
            public InvokeResult Result { get; set; }
        }

        private static Type[] PrimitiveTypes =
            typeof(JToken)
            .GetMethods()
            .Where(M => M.Name == "op_Implicit")
            .Select(M => M.GetParameters()[0].ParameterType)
            .ToArray();


        private readonly ITransporter transporter;
        private int objectId;
        private ConcurrentDictionary<Guid, WaitPair> waiters
            = new ConcurrentDictionary<Guid, WaitPair>();
        private bool aborted = false;


        private RemoteWorker(ITransporter transporter, int objectId)
        {
            this.transporter = transporter;
            this.objectId = objectId;
            transporter.OnReceiveResult += Transporter_OnReceiveResult;
            transporter.OnConnectionLost += () =>
                 {
                     Console.WriteLine("client connection aborted");
                     aborted = true;
                 };
        }

        private Task Transporter_OnReceiveResult(InvokeResult obj)
        {
            if (waiters.TryGetValue(obj.Id, out var value))
            {
                value.Result = obj;
                value.ResetEvent.Set();
            }
            return Task.CompletedTask;
        }

        internal static T Create<T>(ITransporter transporter, int serviceId)
        {
            CheckType(typeof(T));
            return (T)proxyGenerator.CreateInterfaceProxyWithoutTarget(
                typeof(T),
                new RemoteWorker(transporter, serviceId));
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
            if (aborted) throw new ServiceUnavailableException();
            InvokeMessage message = new InvokeMessage
            {
                Id = Guid.NewGuid(),
                MethodName = invocation.Method.Name,
                SubObjectId = objectId
            };

            int i = 0;
            foreach (var param in invocation.Method.GetParameters())
            {
                var argument = invocation.Arguments[i++];
                if (argument is Stream stream)
                {
                    message.Streams[param.Name] = stream;
                }
                else
                {
                    message.Args[param.Name] = JToken.FromObject(argument);
                }
            }
            var data = JsonConvert.SerializeObject(message, Formatting.Indented);
            Console.WriteLine(data);

            ManualResetEvent resetEvent = new ManualResetEvent(false);

            waiters.TryAdd(message.Id, new WaitPair { ResetEvent = resetEvent });

            transporter.SendMessage(message).Wait();

            while(!resetEvent.WaitOne(TimeSpan.FromSeconds(0.5)))
            {
                if (aborted) throw new ServiceUnavailableException();
            }

            if (!waiters.TryRemove(message.Id, out var waitPair))
                throw new Exception("Blya blya blya nety waitera!!!!");
            var result = waitPair.Result;

            if (result.SubObjectId != -1)
            {
                Console.WriteLine($"Hard object, id: {result.SubObjectId} for request {message.MethodName}");
                var worker = new RemoteWorker(transporter, result.SubObjectId);
                invocation.ReturnValue = Create(invocation.Method.ReturnType, worker);
                return;
            }

            if (result.IsPrimitive)
            {
                Console.WriteLine($"Primitive, value: {result.Value} for resuest {message.MethodName}");
                invocation.ReturnValue = result.Value?.ToObject(invocation.Method.ReturnType);
                return;
            }


            if (invocation.Method.ReturnType == typeof(Stream))
            {
                Console.WriteLine($"Stream, length: {result.StreamValue.Length} for request {message.MethodName}");
                invocation.ReturnValue = result.StreamValue;
                return;
            }

            if (invocation.Method.ReturnType == typeof(void))
            {
                Console.WriteLine($"void method {message.MethodName}");
                return;
            }

            if (result.Value.Type == JTokenType.Null)
            {
                Console.WriteLine($"null value for request {message.MethodName}");
                return;
            }

        }
    }
}