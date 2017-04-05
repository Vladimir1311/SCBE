using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels.ResponseTypes
{
    public class GetTokenInfo : ResponseData
    {
        public string AccessToken { get; set; }
        public int Port { get; set; }
        public string ForConnection { get; set; }

        public GetTokenInfo()
        {
            Success = true;
        }
    }
}
