using Newtonsoft.Json;

namespace SituationCenter.Shared.ResponseObjects.Rooms
{
    public class RoomInfoResponse : ResponseBase
    {
        [JsonProperty("room")]
        public RoomPresent RoomPresent { get; set; }

        protected RoomInfoResponse(RoomPresent present) =>
            RoomPresent = present;

        public static RoomInfoResponse Create(RoomPresent present) =>
            new RoomInfoResponse(present);
    }
}