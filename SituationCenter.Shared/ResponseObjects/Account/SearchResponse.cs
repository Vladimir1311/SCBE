using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using SituationCenter.Shared.Models.People;

namespace SituationCenter.Shared.ResponseObjects.Account
{
    public class SearchResponse : ResponseBase
    {
        public IEnumerable<PersonPresent> Users { get; }
        private SearchResponse(IEnumerable<PersonPresent> list) =>
            Users = list;

        public static SearchResponse Create(IEnumerable<PersonPresent> list)
            => new SearchResponse(list);
    }
}
