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


        public string Handle(IFormCollection request)
        {
            var jsonValue = request["simpleargs"].ToString();
            var message = JsonConvert.DeserializeObject<InvokeMessage>(jsonValue);

            if (subWorkers.TryGetValue(message.SubObjectId, out var handler))
            {
                return handler.HardWork(request, message);
            }
            return HardWork(request, message);
        }

        private string HardWork(IFormCollection request, InvokeMessage message)
        {
            var targetMethod = GetTargetMethod(message.MethodName);
            var parameters = new List<object>();
            foreach (var param in targetMethod.GetParameters())
            {
                var arg = message.Args[param.Name];
                parameters.Add(arg.ToObject(param.ParameterType));
            }
            var result = targetMethod.Invoke(worker, parameters.ToArray());
            if (PrimitiveTypes.Contains(targetMethod.ReturnType) || result == null)
                return JsonConvert.SerializeObject(
                    new InvokeResult
                    {
                        IsPrimitive = true,
                        Value = JToken.FromObject(result ?? "Void")
                    });
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
            while(allTypes.Count != 0)
            {
                var type = allTypes.Dequeue();
                var method = type.GetMethod(methodName);
                if (method != null)
                    return method;
                foreach (var baseType in type.GetInterfaces())
                {
                    allTypes.Enqueue(baseType);
                }
            }
            return null;

        }
    }
}
