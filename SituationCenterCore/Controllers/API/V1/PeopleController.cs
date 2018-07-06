using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenter.Shared.Requests.People;
using SituationCenter.Shared.ResponseObjects;
using SituationCenter.Shared.ResponseObjects.People;
using SituationCenterCore.Controllers;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Extensions;
using SituationCenterCore.Filters;
using SituationCenterCore.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SSituationCenterCore.Controllers.API.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    [TypeFilter(typeof(JsonExceptionsFilterAttribute))]
    public class PeopleController : BaseParamsController
    {
        private readonly IRepository repository;
        private readonly IFileServerNotifier fileServerNotifier;

        public PeopleController(IRepository repository, IFileServerNotifier fileServerNotifier)
        {
            this.repository = repository;
            this.fileServerNotifier = fileServerNotifier;
        }

        public async Task<MeResponse> Me()
        {
            var user = await repository.FindUser(User) ?? throw new ArgumentException();
            return MeResponse.Create(user.RoomId, user.ToPresent());
        }

        public async Task<SCFSTokenResponse> GetSCFSToken()
        {
            var token = "".Random(40);
            await fileServerNotifier.AddToken(UserId, token);
            return new SCFSTokenResponse { TempToken = token };
        }

        [HttpPost]
        public async Task<UsersSelectionResponse> SelectContacts([FromBody]SelectContactsInfo selectInfo)
        {
            var currentUser = await repository.FindUser(User);
            var usersPresents = repository
                .Users
                .Where(U => U.PhoneNumber != currentUser.PhoneNumber)
                .Where(U => selectInfo.PhoneNumbers.Contains(U.PhoneNumber))
                .Select(U => U.ToPresent())
                .ToArray();
            return UsersSelectionResponse.Create(usersPresents);
        }
    }
}