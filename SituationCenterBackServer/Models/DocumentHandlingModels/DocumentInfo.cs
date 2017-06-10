using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.DocumentHandlingModels
{
    public class DocumentInfo
    {
        public int Progress { get; set; }
        public int[] AvailablePages { get; set; }

    }
}
