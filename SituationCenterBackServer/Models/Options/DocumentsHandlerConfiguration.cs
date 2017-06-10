using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.Options
{
    public class DocumentsHandlerConfiguration
    {
        public string[] SupportedFormats { get; set; }
        public string EndPoint { get; set; }
    }
}
