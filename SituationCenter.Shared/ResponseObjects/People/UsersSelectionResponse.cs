using System.Collections.Generic;
using SituationCenter.Shared.Models.People;
using System.Linq;
using Newtonsoft.Json;


namespace SituationCenter.Shared.ResponseObjects.People
{
    public class UsersSelectionResponse : ResponseBase
    {
        [JsonProperty("people")]
        public List<PersonPresent> Users { get; }
        private UsersSelectionResponse(IEnumerable<PersonPresent> users) =>
            Users = users.ToList();

        public static UsersSelectionResponse Create(IEnumerable<PersonPresent> users) =>
            new UsersSelectionResponse(users);
    }
}
