using Newtonsoft.Json;
using SituationCenter.Shared.People;
using System;

namespace SituationCenter.Shared.ResponseObjects.People
{
    public class MeResponse : ResponseBase
    {
        [JsonProperty("roomId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? RoomId { get; set; }
        [JsonProperty("me")]
        public PersonPresent Me { get; set; }

        protected MeResponse(Guid? roomId, PersonPresent person)
        {
            RoomId = roomId;
            Me = person;
        }

        public static MeResponse Create(Guid? roomId, PersonPresent present) 
            => new MeResponse(roomId, present);
    }
}