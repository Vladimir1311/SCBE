using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class ResponseData
    {
        public bool Succsess { get; protected set; }
        public string Message { get; set; }

        public static ResponseData ErrorRequest(string message)
            => new ResponseData { Succsess = false, Message = message };
    }
}
