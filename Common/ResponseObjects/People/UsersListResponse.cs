using Common.People;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Common.ResponseObjects.People
{
    public class UsersListResponse : ResponseBase
    {
        [JsonProperty("people")]
        public PersonPresent[] Users { get; set; }

        protected UsersListResponse(IEnumerable<PersonPresent> users) : base() =>
            Users = users.ToArray();

        public static UsersListResponse Create(IEnumerable<PersonPresent> users) =>
            new UsersListResponse(users);
    }
}