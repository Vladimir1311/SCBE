using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenter.Shared.Requests.People;
using SituationCenter.Shared.ResponseObjects;
using SituationCenter.Shared.ResponseObjects.People;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Extensions;
using SituationCenterCore.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSituationCenterCore.Controllers.API.V1
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    [TypeFilter(typeof(JsonExceptionsFilterAttribute))]
    public class PeopleController : Controller
    {
        private readonly IRepository repository;

        public PeopleController(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<ResponseBase> Me()
        {
            var user = await repository.FindUser(User) ?? throw new ArgumentException();
            return MeResponse.Create(user.RoomId);
        }

        [HttpPost]
        public ResponseBase SelectContacts([FromBody]SelectContactsInfo selectInfo)
        {
            var usersPresents = repository
                .Users
                .Where(U => selectInfo.PhoneNumbers.Contains(U.PhoneNumber))
                .Select(U => U.ToPresent())
                .ToArray();
            return UsersListResponse.Create(usersPresents);
        }
    }
}