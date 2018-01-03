using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.Messages
{
    class Value
    {
        private static Type[] PrimitiveTypes =
            typeof(JToken)
            .GetMethods()
            .Where(M => M.Name == "op_Implicit")
            .Select(M => M.GetParameters()[0].ParameterType)
            .ToArray();

        public ValueType Type { get; set; }
        public object Data { get; set; }

        internal static Value FromObject(object argument)
        {
            Value result = new Value();
            if (argument == null)
            {
                result.Type = ValueType.Null;
            }
            else
            if (PrimitiveTypes.Contains(argument.GetType()))
            {
                result.Type = ValueType.Primitive;
                result.Data = JToken.FromObject(argument);
            }
            else
            {
                result.Type = ValueType.HardObject;
                
            }
        }
    }
}
