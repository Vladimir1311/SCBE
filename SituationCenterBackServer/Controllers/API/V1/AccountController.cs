using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SituationCenterBackServer.Models.AccountViewModels;
using Common.ResponseObjects;
using SituationCenterBackServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using SituationCenterBackServer.Models.TokenAuthModels;
using Microsoft.IdentityModel.Tokens;
using Common.ResponseObjects.Account;
using Microsoft.Extensions.Options;
using SituationCenterBackServer.Data;

namespace SituationCenterBackServer.Controllers.API.V1
{
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuthOptions authOptions;
        private readonly ApplicationDbContext database;

        public AccountController(ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            IOptions<AuthOptions> option,
            ApplicationDbContext database)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.authOptions = option.Value;
            this.database = database;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> Authorize([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return ResponseBase.BadResponse("not correct email or password", logger);
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ResponseBase.BadResponse("not correct email", logger);
            if (!await userManager.CheckPasswordAsync(user, model.Password))
                return ResponseBase.BadResponse("not correct passeord", logger);
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
        public async Task<ResponseBase> Register([FromBody]RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await userManager.CreateAsync(user, model.Password);
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
                else
                {
                    logger.LogWarning(string.Join(" ", result.Errors.Select(E => $"{E.Code}---{E.Description}")));
                }
            }
            return ResponseBase.BadResponse("What?!");
        }

        [HttpGet]
        public ResponseBase Search(string firstName, string lastName, string phone)
        {
            var users = database.Users.Where(U => U.PhoneNumber.Contains(phone));
            return Common.ResponseObjects.Account.Search.Create(users.Select(U => new UserPresent { Phone = U.PhoneNumber }));
        }


    }
}