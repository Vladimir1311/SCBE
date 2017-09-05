using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.IO;
using CCF.Transport;

namespace CCF
{
    public class ServiceCode
    {
        private static Type[] PrimitiveTypes =
            typeof(JToken)
            .GetMethods()
            .Where(M => M.Name == "op_Implicit")
            .Select(M => M.GetParameters()[0].ParameterType)
            .ToArray();

        private object worker;
        private Type workerType;
        private readonly ITransporter transporter;
        private static int lastSubWorkerId;

        private static ConcurrentDictionary<int, ServiceCode> subWorkers
            = new ConcurrentDictionary<int, ServiceCode>();

        internal static ServiceCode Create<T>(ITransporter transporter, T serviceInvoker)
        {
            var code = new ServiceCode(serviceInvoker, typeof(T));
            transporter.OnReceiveMessge += M => transporter.SendResult(code.Handle(M));
            return code;
        }

        private ServiceCode(object targetObject, Type objectType)
        {
            worker = targetObject;
            workerType = objectType;
        }


        private InvokeResult Handle(InvokeMessage invokeMessage)
        {

            if (subWorkers.TryGetValue(invokeMessage.SubObjectId, out var handler))
            {
                return handler.HardWork(invokeMessage);
            }
            return HardWork(invokeMessage);
        }

        private InvokeResult HardWork(InvokeMessage message)
        {
            var targetMethod = GetTargetMethod(message.MethodName);
            var parameters = new List<object>();
            foreach (var param in targetMethod.GetParameters())
            {
                if (message.Args.TryGetValue(param.Name, out var token))
                {
                    parameters.Add(token.ToObject(param.ParameterType));
                }
                else
                {
                    var stream = message.Streams.FirstOrDefault(P => P.Key == param.Name).Value;
                    parameters.Add(stream);
                }
            }
            var result = targetMethod.Invoke(worker, parameters.ToArray());
            if (result == null)
            {
                return new InvokeResult
                {
                    Id = message.Id,
                    IsPrimitive = false,
                    Value = JValue.CreateNull()
                };
            }
            if (PrimitiveTypes.Contains(targetMethod.ReturnType))
                return new InvokeResult
                {
                    Id = message.Id,
                    IsPrimitive = true,
                    Value = JToken.FromObject(result ?? "Void")
                };
            if (result is Stream streamResult)
                return new InvokeResult
                {
                    Id = message.Id,
                    IsPrimitive = false,
                    StreamValue = streamResult
                };
            var subWorkerKey = lastSubWorkerId++;
            subWorkers[subWorkerKey] = new ServiceCode(result, targetMethod.ReturnType);
            return new InvokeResult
            {
                Id = message.Id,
                IsPrimitive = false,
                SubObjectId = subWorkerKey
            };
        }

        private MethodInfo GetTargetMethod(string methodName)
        {
            Queue<Type> allTypes = new Queue<Type>();
            allTypes.Enqueue(workerType);
            while (allTypes.Count != 0)
            {
                var type = allTypes.Dequeue();
                var method = type.GetMethod(methodName);
                if (method != null)
                    return method;
                foreach (var baseType in type.GetInterfaces())
                {
                    allTypes.Enqueue(baseType);
                }
                allTypes.Enqueue(type.GetTypeInfo().BaseType);
            }
            return null;
        }
    }
}