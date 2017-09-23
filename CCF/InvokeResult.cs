using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CCF
{
    internal class InvokeResult
    {
        public Guid Id { get; set; }
        public bool IsPrimitive { get; set; }
        public JToken Value { get; set; }
        public Stream StreamValue { get; set; }
        public int SubObjectId { get; set; } = -1;
    }
}