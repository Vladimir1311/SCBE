﻿using System;

namespace SituationCenterBackServer.Models.VoiceChatModels.ResponseTypes
{
    public class SignInRoomInfo : ResponseData
    {
        public Guid RoomId { get; set; }
        public byte ClientId { get; set; }

        public SignInRoomInfo()
        {
            Success = true;
        }
    }
}