using Newtonsoft.Json;
using SituationCenter.Shared.Models.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using SituationCenter.Shared.ResponseObjects.People;

namespace SituationCenter.Shared.ResponseObjects.Rooms
{
    public class RoomView
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("usersCount")]
        public int UsersCount { get; set; }
        [JsonProperty("peopleCountLimit")]
        public int MaxPeopleCount { get; set; }
        [JsonProperty("privacyType")]
        public PrivacyRoomType PrivacyType { get; set; }
        [JsonProperty("users")]
        public List<PersonView> Users { get; set; }
    }
}
