using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SituationCenterBackServer.Models;
using SituationCenterBackServer.Models.AccountViewModels;
using SituationCenterBackServer.Models.TokenAuthModels;
using SituationCenterBackServer.Models.VoiceChatModels;
using SituationCenterBackServer.Models.VoiceChatModels.ResponseTypes;
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
        private readonly ILogger<UnrealAPIController> _logger;

        private UnrealAPIConfiguration _config;
        private IRoomManager _roomManager;

        public UnrealAPIController(UserManager<ApplicationUser> userManager,
            IOptions<UnrealAPIConfiguration> config,
            IRoomManager roomManager,
            ILogger<UnrealAPIController> logger)
        {
            _userManager = userManager;
            _config = config.Value;
            _roomManager = roomManager;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<ResponseData> GetToken(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Не переданы email и/или пароль");
                return ResponseData.ErrorRequest("not correct email or password");
            }
            var identity = await GetIdentity(model.Email, model.Password);
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
            _logger.LogInformation($"Выдан токен {encodedJwt} на логин {model.Email}");
            return new GetTokenInfo()
            {
                AccessToken = encodedJwt
            };
        }
        
        public ResponseData GetRoomsData()
        {
            _logger.LogInformation("Запрошен вывод списка комнат");
            return new GetRoomsInfo() { Rooms = _roomManager.Rooms};
        }

        public async Task<ResponseData> CreateRoom(string name)
        {
            try
            {
                _logger.LogInformation("Запрошено создание комнаты с именем " + name);
                var userId= _userManager.GetUserName(User);
                var currentUser = await _userManager.FindByNameAsync(userId);
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
                _logger.LogWarning($"Ошибка при создании комнаты {ex.Message}");
                return ResponseData.ErrorRequest(ex.Message);
            }
        }

        public async Task<ResponseData> JoinToRoom (byte? roomId, string roomName, float? floatId)
        {
            try
            {
                //TODO КОСТЫЛЬ!!!
                if (floatId != null)
                    roomId = (byte)(floatId.Value);

                var userId = _userManager.GetUserName(User);
                var currentUser = await _userManager.FindByNameAsync(userId);
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
        public async Task<ResponseData> LeaveTheRoom()
        {
            var userId = _userManager.GetUserName(User);
            var currentUser = await _userManager.FindByNameAsync(userId);
            if (_roomManager.RemoveFromRoom(currentUser))
                return ResponseData.GoodResponse("Вы успещно вышли из комнаты");
            else
                return ResponseData.GoodResponse("Вы не состояли ни в какой комнате");
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
