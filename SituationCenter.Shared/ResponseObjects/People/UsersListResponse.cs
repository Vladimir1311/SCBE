using Newtonsoft.Json;
using SituationCenter.Shared.People;
using System.Collections.Generic;
using System.Linq;

namespace SituationCenter.Shared.ResponseObjects.People
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