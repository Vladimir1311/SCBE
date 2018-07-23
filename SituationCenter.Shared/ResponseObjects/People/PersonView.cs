using Newtonsoft.Json;
using System;

namespace SituationCenter.Shared.ResponseObjects.People
{
    public class PersonView
    {
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastName")]
        public string Surname { get; set; }

        [JsonProperty("phonenumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

    }
}