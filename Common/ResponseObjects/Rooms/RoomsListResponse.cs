using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects.Rooms
{
    public class RoomsListResponse : ResponseBase
    {
        [JsonProperty("rooms")]
        public IEnumerable<RoomPresent> Rooms { get; set; }

        protected RoomsListResponse(IEnumerable<RoomPresent> rooms): base() =>
            Rooms = rooms;

        public static RoomsListResponse Create(IEnumerable<RoomPresent> rooms) =>
            new RoomsListResponse(rooms);

    }
}
