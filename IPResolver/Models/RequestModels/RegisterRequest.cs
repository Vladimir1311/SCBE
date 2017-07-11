using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Models.RequestModels
{
    public class RegisterRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }
    }
}
