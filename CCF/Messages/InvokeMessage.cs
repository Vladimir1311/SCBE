using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace CCF.Messages
{
    class InvokeMessage
    {
        public Guid Id { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string, Value> Args { get; set; }
        [JsonIgnore]
        public Dictionary<string, Stream> Streams { get; set; }
        public int SubObjectId { get; set; } = -1;
    }
}