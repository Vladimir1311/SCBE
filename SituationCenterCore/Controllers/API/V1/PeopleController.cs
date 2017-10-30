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
using URSA.Respose;

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

        public async Task<URespose<Guid>> Me()
        {
            var user = await repository.FindUser(User) ?? throw new ArgumentException();
            return user.RoomId;
        }

        [HttpPost]
        public async Task<URespose<UsersList>> SelectContacts([FromBody]SelectContactsInfo selectInfo)
        {
            var currentUser = await repository.FindUser(User);
            var usersPresents = repository
                .Users
                .Where(U => U.PhoneNumber != currentUser.PhoneNumber)
                .Where(U => selectInfo.PhoneNumbers.Contains(U.PhoneNumber))
                .Select(U => U.ToPresent())
                .ToArray();
            return UsersList.Create(usersPresents);
        }
    }
}