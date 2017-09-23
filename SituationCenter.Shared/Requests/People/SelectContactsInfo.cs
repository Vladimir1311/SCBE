using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationCenter.Shared.Requests.People
{
    public class SelectContactsInfo
    {
        [JsonProperty("numbers")]
        public string[] PhoneNumbers { get; set; }
    }
}
