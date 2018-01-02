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
        public Dictionary<string, JToken> Args { get; set; } = new Dictionary<string, JToken>();
        [JsonIgnore]
        public Dictionary<string, Stream> Streams { get; set; } = new Dictionary<string, Stream>();
        public int SubObjectId { get; set; } = -1;
    }
}