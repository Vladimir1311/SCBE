using Common.Models.Rooms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects.Rooms
{
    public class RoomPresent
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("usersCount")]
        public int UsersCount { get; set; }
        [JsonProperty("privacyType")]
        public PrivacyRoomType PrivacyType { get; set; }

        public RoomPresent(Guid id, string name, int userCount, PrivacyRoomType privacy)
        {
            Id = id;
            Name = name;
            UsersCount = userCount;
            PrivacyType = privacy;
        }
    }
}
