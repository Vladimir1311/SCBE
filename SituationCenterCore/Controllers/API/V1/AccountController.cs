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
using SituationCenterCore.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SituationCenter.Shared.Requests.Account;
using SituationCenter.Shared.ResponseObjects.General;
using SituationCenter.Shared.ResponseObjects.Account;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace SituationCenterCore.Controllers.API.V1
{
    [Produces("application/json")]
    [Route("api/v1/Account/[action]")]
    public class AccountController : BaseParamsController
    {
        private readonly ILogger<AccountController> logger;
        private readonly IRepository repository;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly AuthOptions authOptions;

        public AccountController(
            ILogger<AccountController> logger,
            IOptions<AuthOptions> option,
            IRepository repository,
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            this.logger = logger;
            this.repository = repository;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.authOptions = option.Value;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ListResponse<RefreshTokenView>> Get()
            => await dbContext.RefreshTokens
                            .Where(t => t.UserId == UserId)
                            .ProjectTo<RefreshTokenView>()
                            .ToListAsync();
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseBase> Disabple(List<Guid> tokensToRemove)
        {
            var targetTokens = await dbContext
                .RefreshTokens
                .Where(t => tokensToRemove.Contains(t.Id))
                .ToListAsync();
            if (targetTokens.Count != tokensToRemove.Count)
                throw new StatusCodeException(SituationCenter.Shared.Exceptions.StatusCode.ArgumentsIncorrect);

            dbContext.RefreshTokens.RemoveRange(targetTokens);
            dbContext.RemovedTokens.AddRange(targetTokens.Select(mapper.Map<RemovedToken>));
            await dbContext.SaveChangesAsync();
            return ResponseBase.OKResponse;
        }

        [HttpPost]
        public async Task<OneObjectResponse<AccessAndRefreshTokenPair>> Authorize([FromBody]LoginRequest model)
        {
            if (!ModelState.IsValid)
                throw new StatusCodeException(SituationCenter.Shared.Exceptions.StatusCode.ArgumentsIncorrect);
            var user = await repository.FindUserByEmailAsync(model.Email);

            if (user == null || !await repository.CheckUserPasswordAsync(user, model.Password))
                throw new StatusCodeException(SituationCenter.Shared.Exceptions.StatusCode.AuthorizeError);
            return await GenerateAll(user);
        }

        [HttpPost]
        public async Task<OneObjectResponse<AccessAndRefreshTokenPair>> RefreshToken([FromBody]string refreshToken)
        {
            var nowRefreshToken = await dbContext
                .RefreshTokens
                .Include(rt => rt.User)
                .Where(t => t.TokenContent == refreshToken)
                .Where(t => t.UserAgent == UserAgent)
                .SingleOrDefaultAsync() ?? throw new StatusCodeException(SituationCenter.Shared.Exceptions.StatusCode.IncorrectRefreshToken);

            dbContext.RefreshTokens.Remove(nowRefreshToken);
            return await GenerateAll(nowRefreshToken.User, nowRefreshToken.CreateTime);
        }

        [HttpPost]
        public async Task<ResponseBase> Registration([FromBody]RegisterRequest model)
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
                    Sex = model.Sex == Sex.Male,
                    Birthday = model.Birthday
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
                    return ResponseBase.OKResponse;
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
            throw new ApiArgumentException();
        }

        private async Task<OneObjectResponse<AccessAndRefreshTokenPair>> GenerateAll(ApplicationUser user,
                                                                                     DateTime? lastRefreshTokenCreateTime = null)
        {
            var refreshToken = await CreateRefreshToken(user.Id, UserAgent, lastRefreshTokenCreateTime);
            var claims = new[]
            {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("RefreshTokenId", refreshToken.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var pair = new AccessAndRefreshTokenPair
            {
                AccessToken = CreateAccessToken(identity),
                RefreshToken = refreshToken.TokenContent
            };
            return pair;
        }

        private async Task<RefreshToken> CreateRefreshToken(Guid userId, string userAgent, DateTime? lastRefreshTokenTime)
        {
            var now = DateTime.Now;
            var refreshToken = new RefreshToken
            {
                TokenContent = "".Random(600),
                UserId = userId,
                UserAgent = userAgent,
                CreateTime = lastRefreshTokenTime ?? now,
                LastUpdateTime = now
            };
            dbContext.Add(refreshToken);
            await dbContext.SaveChangesAsync();
            return refreshToken;
        }


        private static string CreateAccessToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                        issuer: MockAuthOptions.ISSUER,
                        audience: MockAuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(MockAuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(MockAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task CheckRegistrationsArgs(RegisterRequest model)
        {
            var codes = new List<StatusCode>();

            if (await repository.AnyUser(u => u.PhoneNumber == model.PhoneNumber))
                codes.Add(SituationCenter.Shared.Exceptions.StatusCode.PhoneBusy);
            if (await repository.AnyUser(u => u.Email == model.Email))
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