using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SituationCenterBackServer.Models;
using SituationCenterBackServer.Models.AccountViewModels;
using SituationCenterBackServer.Models.TokenAuthModels;
using SituationCenterBackServer.Models.VoiceChatModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    public class UnrealAPIController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private UnrealAPIConfiguration _config;
        private IRoomManager _roomManager;

        public UnrealAPIController(UserManager<ApplicationUser> userManager,
            IOptions<UnrealAPIConfiguration> config,
            IRoomManager roomManager)
        {
            _userManager = userManager;
            _config = config.Value;
            _roomManager = roomManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetToken(LoginViewModel model)
        {
          //  if (!ModelState.IsValid)
            //    return Json(new { message = "not correct email or password"});

            var identity = await GetIdentity(model.Email, model.Password);
            if (identity == null)
                return Json(new { message = "not correct email or password" });
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Json(new
            {
                access_token = encodedJwt,
                username = identity.Name
            });
        }
        
        public IEnumerable<string> GetRoomNames()
        {
            return _roomManager.RoomNames;
        }

        public async Task<ResponseData> CreateRoom(string name)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var (room, clientId) = _roomManager.CreateNewRoom(currentUser, name);
                return new SignInRoomInfo()
                {
                    ClientId = clientId,
                    RoomId = room.Id,
                    Port = _config.Port
                };
            }
            catch(Exception ex)
            {
                return ResponseData.ErrorRequest(ex.Message);
            }
        }

        public async Task<ResponseData> JoinToRoom (byte? roomId, string roomName)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                (Room room, byte ClientId) returned;
                if (roomId != null)
                    returned = _roomManager.JoinToRoom(currentUser, roomId.Value);
                else if (roomName != null)
                    returned = _roomManager.JoinToRoom(currentUser, roomName);
                else throw new Exception("Передайте параметр для входа в комнату");

                return new SignInRoomInfo
                {
                    ClientId = returned.ClientId,
                    RoomId = returned.room.Id,
                    Port = _config.Port
                };
            }
            catch (Exception ex)
            {
                return ResponseData.ErrorRequest(ex.Message);
                throw;
            }
        }




        private async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };
            return new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

        }
    }
}
