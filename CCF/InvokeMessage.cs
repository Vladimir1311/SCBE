using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCF
{
    class InvokeMessage
    {
        public Guid MessageId { get; set; }
        public string MethodName { get; set; }
        public JToken Args { get; set; }
    }
}
