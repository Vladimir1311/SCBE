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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SituationCenter.Shared.ResponseObjects.General;
using SituationCenterCore.DataFormatting;

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
        private readonly IMapper mapper;

        public PeopleController(
            IRepository repository,
            IFileServerNotifier fileServerNotifier,
            IMapper mapper)
        {
            this.repository = repository;
            this.fileServerNotifier = fileServerNotifier;
            this.mapper = mapper;
        }

        public async Task<OneObjectResponse<MeAndRoom>> Me()
        {
            var user = await repository.FindUser(User) ?? throw new ArgumentException();
            return mapper.Map<MeAndRoom>(user);
        }

        public async Task<OneObjectResponse<string>> GetSCFSToken()
        {
            var token = "".Random(40);
            var user = await repository.FindUser(User);
            await fileServerNotifier.AddToken(UserId, token);
            await fileServerNotifier.SetRoom(UserId, user.RoomId);
            return token;
        }

        [HttpGet]
        public async Task<ListResponse<PersonView>> Search(
            [UpperCase] string firstName,
            [UpperCase] string lastName,
            [UpperCase] string email,
            string phone,
            int pageSize = 10,
            int pageNum = 0)
         => await repository
             .Users
             .IfNotNullOrEmpry(firstName, users => users.Where(u => u.Name.ToUpper().Contains(firstName)))
             .IfNotNullOrEmpry(lastName, users => users.Where(u => u.Surname.ToUpper().Contains(lastName)))
             .IfNotNullOrEmpry(email, users => users.Where(u => u.Email.ToUpper().Contains(email)))
             .IfNotNullOrEmpry(phone, users => users.Where(u => u.PhoneNumber.ToUpper().Contains(phone)))
             .Skip(pageNum * pageSize)
             .Take(pageSize)
             .ProjectTo<PersonView>()
             .ToListAsync();



        [HttpPost]
        public async Task<ListResponse<PersonView>> SelectContacts([FromBody]SelectContactsInfo selectInfo)
        {
            var currentUserPhone = await repository
                .Users
                .Where(u => u.Id == UserId.ToString())
                .Select(u => u.PhoneNumber)
                .SingleAsync();

            var usersPresents = await repository
                .Users
                .Where(u => u.PhoneNumber != currentUserPhone)
                .Where(u => selectInfo.PhoneNumbers.Contains(u.PhoneNumber))
                .ProjectTo<PersonView>()
                .ToListAsync();
            return usersPresents;
        }
    }
}