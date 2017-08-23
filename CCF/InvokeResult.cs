using Newtonsoft.Json.Linq;

namespace CCF
{
    internal class InvokeResult
    {
        public bool IsPrimitive { get; set; }
        public JToken Value { get; set; }
        public int SubObjectId { get; set; }
    }
}