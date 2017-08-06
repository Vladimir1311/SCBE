using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCF
{
    class InvokeResult
    {
        public bool IsPrimitive { get; set; }
        public JToken Value { get; set; }
        public int SubObjectId { get; set; }
    }
}
