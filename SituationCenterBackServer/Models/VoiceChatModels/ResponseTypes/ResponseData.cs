using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels.ResponseTypes
{
    public class ResponseData
    {
        public bool Success { get; protected set; }
        public string Message { get; set; }

        public static ResponseData ErrorRequest(string message)
            => new ResponseData { Success = false, Message = message };

        public static ResponseData GoodResponse(string message)
            => new ResponseData { Success = true, Message = message };
    }
}
