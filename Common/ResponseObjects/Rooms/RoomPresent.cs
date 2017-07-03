using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects.Rooms
{
    public class RoomPresent
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        public RoomPresent(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
