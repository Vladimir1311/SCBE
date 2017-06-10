using Common.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.DocumentHandlingModels
{
    public class GetDocumentInfoResponse : ResponseBase
    {
        public new DocumentInfo Object { get; set; }
    }
}
