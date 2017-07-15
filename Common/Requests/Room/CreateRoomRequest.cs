using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Requests.Room
{
    public class CreateRoomRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
