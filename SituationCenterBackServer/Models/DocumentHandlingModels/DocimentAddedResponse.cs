using Common.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.DocumentHandlingModels
{
    public class DocimentAddedResponse : ResponseBase
    {
        public new DocumentAddedObject Object { get; set; }
    }
}
