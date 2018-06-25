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
using CCF.Messages;

namespace CCF
{
    public class RemoteWorker : IInterceptor, IDisposable
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
        private long objectId;
        private ConcurrentDictionary<Guid, WaitPair> waiters
            = new ConcurrentDictionary<Guid, WaitPair>();
        private bool aborted = false;
        private bool wrapped;


        private RemoteWorker(ITransporter transporter, long objectId, bool wrapped)
        {
            this.transporter = transporter;
            this.objectId = objectId;
            this.wrapped = wrapped;
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

        internal static object Create(Type workerType, ITransporter transporter, long serviceId, bool disposeWrapped = false)
        {
            CheckType(workerType);
            return proxyGenerator.CreateInterfaceProxyWithoutTarget(
                workerType,
                new RemoteWorker(transporter, serviceId, disposeWrapped));
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
                SubObjectId = objectId,
                Args = new Dictionary<string, Value>(),
                Streams = new Dictionary<string, Stream>()
            };

            if (invocation.Method.Name == "Dispose" && wrapped)
            {
                Dispose();
                return;
            }

            int i = 0;
            foreach (var param in invocation.Method.GetParameters())
            {
                var argument = invocation.Arguments[i++];
                if(argument == null)
                {
                    message.Args[param.Name] = new Value { Type = Messages.ValueType.Null };
                }
                else
                if (argument is Stream stream)
                {
                    message.Streams[param.Name] = stream;
                    continue;
                }
                Value value = new Value();
                if (argument == null)
                {
                    value.Type = Messages.ValueType.Null;
                }
                else
                if (PrimitiveTypes.Contains(argument.GetType()))
                {
                    value.Type = Messages.ValueType.Primitive;
                    value.Data = JToken.FromObject(argument);
                }                
                else
                {
                    value.Type = Messages.ValueType.HardObject;
                    CodeInvoker newInvoker = new CodeInvoker(argument, param.ParameterType);
                    InstanceWrapper instanceWrapper = new InstanceWrapper(newInvoker, transporter);
                    value.Data = instanceWrapper.Id;
                }
                message.Args[param.Name] = value;
            }
            var data = JsonConvert.SerializeObject(message, Formatting.Indented);
            Console.WriteLine(data);

            ManualResetEvent resetEvent = new ManualResetEvent(false);

            waiters.TryAdd(message.Id, new WaitPair { ResetEvent = resetEvent });

            transporter.SendMessage(message).Wait();

            while (!resetEvent.WaitOne(TimeSpan.FromSeconds(0.5)))
            {
                if (aborted) throw new ServiceUnavailableException();
            }

            if (!waiters.TryRemove(message.Id, out var waitPair))
                throw new Exception("Blya blya blya nety waitera!!!!");
            var result = waitPair.Result;
            switch (result.ResultType)
            {
                case ResultType.Primitive:
                    Console.WriteLine($"Primitive, value: {result.PrimitiveValue} for resuest {message.MethodName}");
                    invocation.ReturnValue = result.PrimitiveValue.ToObject(invocation.Method.ReturnType);
                    return;
                case ResultType.Null:
                    Console.WriteLine($"null value for request {message.MethodName} or void method");
                    return;
                case ResultType.Stream:
                    Console.WriteLine($"Stream, length: {result.StreamValue.Length} for request {message.MethodName}");
                    invocation.ReturnValue = result.StreamValue;
                    return;
                case ResultType.Exception:
                    throw new Exception(result.ExceptionMessage);
                case ResultType.HardObject:
                    Console.WriteLine($"Hard object, id: {result.HardObjectId} for request {message.MethodName}");
                    var worker = new RemoteWorker(transporter, result.HardObjectId, false);
                    invocation.ReturnValue = Create(invocation.Method.ReturnType, worker);
                    return;
            }
        }

        public void Dispose()
        {
            transporter.Dispose();
            aborted = true;
        }
    }
}