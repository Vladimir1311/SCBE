using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CCF.Messages
{
    internal class InvokeResult
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public ResultType ResultType { get; set; }
        public JToken PrimitiveValue { get; set; }
        public long HardObjectId { get; set; }
        public string ExceptionMessage { get; set; }
        [JsonIgnore]
        public Stream StreamValue { get; set; }

        public InvokeResult()
        {

        }
        public InvokeResult(Guid id, ResultType resultType)
        {
            Id = id;
            ResultType = resultType;
        }

        public static InvokeResult Exception(Guid id, Exception exception)
            => new InvokeResult(id, ResultType.Exception)
            {
                ExceptionMessage = exception.Message
            };

        internal static InvokeResult Null(Guid id)
            => new InvokeResult(id, ResultType.Null);

        internal static InvokeResult Stream(Guid id, Stream stream)
            => new InvokeResult(id, ResultType.Stream)
            {
                StreamValue = stream
            };

        internal static InvokeResult Primitive(Guid id, JToken jToken)
            => new InvokeResult(id, ResultType.Primitive)
            {
                PrimitiveValue = jToken
            };

        internal static InvokeResult HardObject(Guid id, long newId)
            => new InvokeResult(id, ResultType.HardObject)
            {
                HardObjectId = newId
            };
    }
}