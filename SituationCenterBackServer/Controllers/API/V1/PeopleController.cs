using Common.Requests.People;
using Common.ResponseObjects;
using Common.ResponseObjects.People;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Extensions;
using SituationCenterBackServer.Filters;
using SituationCenterBackServer.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Controllers.API.V1
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    [TypeFilter(typeof(JsonExceptionsFilterAttribute))]
    public class PeopleController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public PeopleController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<ResponseBase> Me()
        {
            var user = await userManager.FindUser(User) ?? throw new ArgumentException();
            return MeResponse.Create(user.RoomId);
        }

        [HttpPost]
        public ResponseBase SelectContacts([FromBody]SelectContactsInfo selectInfo)
        {
            var usersPresents = userManager
                .Users
                .Where(U => selectInfo.PhoneNumbers.Contains(U.PhoneNumber))
                .Select(U => U.ToPresent())
                .ToArray();
            return UsersListResponse.Create(usersPresents);
        }
    }
}