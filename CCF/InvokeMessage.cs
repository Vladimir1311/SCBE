using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CCF
{
    class InvokeMessage
    {
        public string MethodName { get; set; }
        public Dictionary<string, JToken> Args { get; set; }
        public int SubObjectId { get; set; } = -1;
    }
}