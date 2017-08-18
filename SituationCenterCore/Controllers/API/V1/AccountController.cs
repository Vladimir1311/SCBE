using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using SituationCenterCore.Data;
using Newtonsoft.Json;
using SituationCenter.Shared.Exceptions;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Models.TokenAuthModels;
using Microsoft.Extensions.Options;
using SituationCenter.Shared.ResponseObjects;
using SituationCenterCore.Pages.Account;
using SituationCenter.Shared.ResponseObjects.Account;

namespace SituationCenterCore.Controllers.API.V1
{
    [Produces("application/json")]
    [Route("api/v1/Account/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly IRepository repository;
        private readonly AuthOptions authOptions;

        public AccountController(ILogger<AccountController> logger,
            IOptions<AuthOptions> option,
            IRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
            this.authOptions = option.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> Authorize([FromBody]LoginModel.InputModel model)
        {
            if (!ModelState.IsValid)
                return ResponseBase.BadResponse("not correct email or password");
            var user = await repository.FindUserByEmailAsync(model.Email);

            if (user == null)
                return ResponseBase.BadResponse("not correct email");

            if (!await repository.CheckUserPasswordAsync(user, model.Password))
                return ResponseBase.BadResponse("not correct password");

            var claims = new Claim[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var jwt = new JwtSecurityToken(
                issuer: authOptions.Issuer,
                audience: authOptions.Audience,
                notBefore: DateTime.Now,
                claims: claims,
                expires: DateTime.Now.Add(authOptions.Expiration),
                signingCredentials: new SigningCredentials(
                    authOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodetJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            logger.LogDebug($"Send token for {user.Email}");
            return AuthorizeResponse.Create(encodetJwt);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> Registration([FromBody]RegisterModel.InputModel model)
        {
            if (ModelState.IsValid)
            {
                await CheckRegistrationsArgs(model);

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    PhoneNumber = model.PhoneNumber,
                    Sex = model.Sex,
                    Birthday = model.ParsedBirthday()
                };

                var result = await repository.CreateUserAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    logger.LogInformation(3, "User created a new account with password.");
                    return ResponseBase.GoodResponse();
                }
                logger.LogWarning(string.Join(" ", result.Errors.Select(E => $"{E.Code}---{E.Description}")));
                logger.LogWarning("Model valid but error");
                logger.LogWarning(JsonConvert.SerializeObject(ModelState, Formatting.Indented));
                logger.LogWarning(JsonConvert.SerializeObject(model, Formatting.Indented));
            }
            //ModelState.
            logger.LogWarning("Model not valid");
            logger.LogWarning(JsonConvert.SerializeObject(ModelState, Formatting.Indented));
            logger.LogWarning(JsonConvert.SerializeObject(model, Formatting.Indented));
            return ResponseBase.BadResponse("What?!");
        }

        [HttpGet]
        public async Task<ResponseBase> Search(string firstName, string lastName, string phone)
        {
            var users = await repository.FindUsers(U => U.PhoneNumber.Contains(phone));
            return SituationCenter.Shared.ResponseObjects.Account.Search.Create(users.Select(U => new UserPresent { Phone = U.PhoneNumber }));
        }

        private async Task CheckRegistrationsArgs(RegisterModel.InputModel model)
        {
            var codes = new List<StatusCode>();

            if (await repository.AnyUser(U => U.PhoneNumber == model.PhoneNumber))
                codes.Add(SituationCenter.Shared.Exceptions.StatusCode.PhoneBusy);
            if (await repository.AnyUser(U => U.Email == model.Email))
                codes.Add(SituationCenter.Shared.Exceptions.StatusCode.EmailBusy);

            switch (codes.Count)
            {
                case 0: return;
                case 1: throw new StatusCodeException(codes[0]);
                default: throw new MultiStatusCodeException(codes);
            }
        }
    }
}