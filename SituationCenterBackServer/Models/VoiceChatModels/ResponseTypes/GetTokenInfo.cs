﻿namespace SituationCenterBackServer.Models.VoiceChatModels.ResponseTypes
{
    public class GetTokenInfo : ResponseData
    {
        public string AccessToken { get; set; }
        public int TcpPort { get; set; }
        public int UdpPort { get; set; }
        public string ForConnection { get; set; }

        public GetTokenInfo()
        {
            Success = true;
        }
    }
}