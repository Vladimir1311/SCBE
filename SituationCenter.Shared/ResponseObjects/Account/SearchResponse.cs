using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationCenter.Shared.ResponseObjects.Account
{
    public class SearchResponse : ResponseBase
    {
        public IEnumerable<UserPresent> Users { get; }
        private SearchResponse(IEnumerable<UserPresent> list) =>
            Users = list;

        public static SearchResponse Create(IEnumerable<UserPresent> list)
            => new SearchResponse(list);
    }

    public class UserPresent
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "No info";
        [JsonProperty("lastName")]
        public string LastName { get; set; } = "No info";
        [JsonProperty("phone")]
        public string Phone { get; set; } = "No info";
        [JsonProperty("email")]
        public string Email { get; set; } = "No info";
    }
}
