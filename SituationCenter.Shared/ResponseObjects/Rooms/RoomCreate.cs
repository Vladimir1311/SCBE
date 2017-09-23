using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationCenter.Shared.ResponseObjects.Rooms
{
    public class RoomCreate : ResponseBase
    {
        [JsonProperty("id")]
        public Guid Id {get; set;}

        public RoomCreate(Guid id) : base() => 
            Id = id;

        public static RoomCreate Create(Guid id) =>
            new RoomCreate(id);
    }
}
