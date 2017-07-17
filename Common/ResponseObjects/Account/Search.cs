using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects.Account
{
    public class Search : ResponseBase
    {
        public IEnumerable<UserPresent> Users;
        private Search(IEnumerable<UserPresent> list) =>
            Users = list;

        public static Search Create(IEnumerable<UserPresent> list)
            => new Search(list);
    }

    public class UserPresent
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "No info";
        [JsonProperty("lastName")]
        public string LastName { get; set; } = "No info";
        [JsonProperty("phone")]
        public string Phone { get; set; } = "Np info";
    }
}
