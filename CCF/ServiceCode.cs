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

        private static int lastSubWorkerId;

        private static ConcurrentDictionary<int, ServiceCode> subWorkers
            = new ConcurrentDictionary<int, ServiceCode>();

        public static ServiceCode Create<T>(T targetObject)
        {
            return new ServiceCode(targetObject, typeof(T));
        }

        private ServiceCode(object targetObject, Type objectType)
        {
            worker = targetObject;
            workerType = objectType;
        }


        public object Handle(string request, IEnumerable<StreamValue> streams)
        {
            var message = JsonConvert.DeserializeObject<InvokeMessage>(request);

            if (subWorkers.TryGetValue(message.SubObjectId, out var handler))
            {
                return handler.HardWork(message, streams);
            }
            return HardWork(message, streams);
        }

        private object HardWork(InvokeMessage message, IEnumerable<StreamValue> streams)
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
                    var stream = streams.FirstOrDefault(P => P.Name == param.Name).Value;
                    parameters.Add(stream);
                }
            }
            var result = targetMethod.Invoke(worker, parameters.ToArray());
            if (result == null)
            {
                return null;
            }
            if (PrimitiveTypes.Contains(targetMethod.ReturnType))
                return JsonConvert.SerializeObject(
                    new InvokeResult
                    {
                        IsPrimitive = true,
                        Value = JToken.FromObject(result ?? "Void")
                    });
            if (result is Stream streamResult) return streamResult;
            var subWorkerKey = lastSubWorkerId++;
            subWorkers[subWorkerKey] = new ServiceCode(result, targetMethod.ReturnType);
            return JsonConvert.SerializeObject(
                new InvokeResult
                {
                    IsPrimitive = false,
                    SubObjectId = subWorkerKey
                });
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