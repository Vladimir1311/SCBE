using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.DocsModels
{
    public class StatusCodeViewModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string CSharpName { get; set; }
    }
}
