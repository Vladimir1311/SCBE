using CCF.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CCF
{
    internal class CodeInvoker
    {
        private static long count;
        private readonly object instance;
        private readonly Type instanceType;
        private long id = count++;


        public CodeInvoker(object instance, Type instanceType)
        {
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.instanceType = instanceType ?? throw new ArgumentNullException(nameof(instanceType));
        }

        public object Invoke(InvokeMessage message)
        {
            var targetMethod = GetTargetMethod(message.MethodName) 
                ?? throw new Exception($"Method {message.MethodName} was not found");
            var parameters = new List<object>();
            foreach (var param in targetMethod.GetParameters())
            {
                if (message.Args.TryGetValue(param.Name, out var token))
                    parameters.Add(token.Data);
                else
                    throw new Exception($"Parameter {param.Name} was nott found in InvokeMessage");
            }
            return targetMethod.Invoke(instance, parameters.ToArray());
        }

        private MethodInfo GetTargetMethod(string methodName)
        {
            Queue<Type> allTypes = new Queue<Type>();
            allTypes.Enqueue(instanceType);
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
