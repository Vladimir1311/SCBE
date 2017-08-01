﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects.People
{
    public class MeResponse : ResponseBase
    {
        [JsonProperty("roomId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid RoomId { get; set; }


        protected MeResponse(Guid roomId)
        {
            RoomId = roomId;
        }

        public static MeResponse Create(Guid roomId) => new MeResponse(roomId);
    }
}
