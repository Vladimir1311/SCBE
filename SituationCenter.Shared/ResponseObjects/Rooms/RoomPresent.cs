using Newtonsoft.Json;
using SituationCenter.Shared.Models.Rooms;
using System;
using SituationCenter.Shared.ResponseObjects.Account;
using System.Collections.Generic;
using System.Linq;
using SituationCenter.Shared.Models.People;

namespace SituationCenter.Shared.ResponseObjects.Rooms
{
    public class RoomPresent
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
        public List<PersonPresent> Users { get; set; }

        public RoomPresent(Guid id, string name, int userCount, PrivacyRoomType privacy, int peopleCountLimit, IEnumerable<PersonPresent> users)
        {
            Id = id;
            Name = name;
            UsersCount = userCount;
            PrivacyType = privacy;
            MaxPeopleCount = peopleCountLimit;
            Users = users.ToList();
        }
    }
}
