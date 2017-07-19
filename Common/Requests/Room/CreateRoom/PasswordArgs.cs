using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Requests.Room.CreateRoom
{
    class PasswordArgs
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
