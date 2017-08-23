using Common.Models.Rooms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Requests.Room.CreateRoom
{
    public class CreateRoomRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("privacy")]
        public PrivacyRoomType PrivacyType { get; set; }
        [JsonProperty("peopleCountLimit")]
        public int UsersCountMax { get; set; } = 6;
        [JsonProperty("args")]
        public JObject Args { get; set; }
    }
}
