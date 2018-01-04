using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.Messages
{
    class Value
    {
        public ValueType Type { get; set; }
        public JToken Data { get; set; }
    }
}
