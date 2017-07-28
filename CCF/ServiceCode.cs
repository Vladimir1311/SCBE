
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace CCF
{
    public class ServiceCode<T>
    
    {
        private T worker;
        private Type workerType;
        public ServiceCode(T targetObject)
        {
            worker = targetObject;
            workerType = targetObject.GetType();
        }

        public string Handle(IFormCollection request)
        {
            var message = JsonConvert.DeserializeObject<InvokeMessage>(request);
            var targetMethod = workerType.GetMethod(message.MethodName, ArgTypes(message.Args));
            var result = targetMethod.Invoke(worker, message.Args.ToObject<object[]>());
            return result.ToString();
        }


        private Type[] ArgTypes(JToken args)
        {
            return handle(args).ToArray();
        }

        private IEnumerable<Type> handle(JToken arg)
        {
            using (var reader = new JTokenReader(arg))
            {
                while (reader.Read())
                    if (reader.ValueType != null)
                        yield return reader.ValueType;
            }
        }

    }
}
