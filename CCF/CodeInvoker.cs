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



        public object Invoke(InvokeMessage message, Func<long, Type, object> hardbjectFor)
        {
            var targetMethod = GetTargetMethod(message.MethodName)
                ?? throw new Exception($"Method {message.MethodName} was not found");
            var parameters = GetArguments(message, hardbjectFor, targetMethod);

            return targetMethod.Invoke(instance, parameters);
        }

        private object[] GetArguments(InvokeMessage message, Func<long, Type, object> hardbjectFor, MethodInfo methodInfo)
        {
            var args = new List<object>();
            foreach (var param in methodInfo.GetParameters())
            {
                if (message.Args.TryGetValue(param.Name, out var token))
                {
                    switch (token.Type)
                    {
                        case Messages.ValueType.Null:
                            args.Add(null);
                            break;
                        case Messages.ValueType.Primitive:
                            args.Add(token.Data.ToObject(param.ParameterType));
                            break;
                        case Messages.ValueType.HardObject:
                            args.Add(hardbjectFor(token.Data.ToObject<long>(),
                                param.ParameterType));
                            break;
                    }
                }
                else
                if (message.Streams.TryGetValue(param.Name, out var stream))
                    args.Add(stream);
                else
                    throw new Exception($"Parameter {param.Name} was not found in InvokeMessage");
            }
            return args.ToArray();
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
