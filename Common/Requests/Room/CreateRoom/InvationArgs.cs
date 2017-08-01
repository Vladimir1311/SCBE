using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Requests.Room.CreateRoom
{
    public class InvationArgs
    {
        [JsonProperty("users")]
        public string[] Phones { get; set; }
    }
}
