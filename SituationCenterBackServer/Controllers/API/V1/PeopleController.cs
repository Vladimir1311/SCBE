using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common.ResponseObjects;
using Microsoft.AspNetCore.Authorization;
using SituationCenterBackServer.Filters;
using SituationCenterBackServer.Models;
using Microsoft.AspNetCore.Identity;
using SituationCenterBackServer.Extensions;
using Common.ResponseObjects.People;
using Common.Requests.People;

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
            var roomId = (await userManager.FindUser(User))?.RoomId ?? throw new ArgumentException();
            return MeResponse.Create(roomId);
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