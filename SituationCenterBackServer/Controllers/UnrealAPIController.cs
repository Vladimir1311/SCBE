using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    public class UnrealApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UnrealApiController> _logger;

        private readonly UnrealAPIConfiguration _config;
        private readonly IRoomManager _roomManager;

        public UnrealApiController(UserManager<ApplicationUser> userManager,
            IOptions<UnrealAPIConfiguration> config,
            IRoomManager roomManager,
            ILogger<UnrealApiController> logger)
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
            if (identity == null)
            {
                _logger.LogWarning("Не верные логин и/или пароль");
                return ResponseData.ErrorRequest("not correct email or password");
            }
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
                AccessToken = encodedJwt,
                Port = _config.Port
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
                    RoomId = room.Id
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
                    RoomId = returned.room.Id
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
            return ResponseData.GoodResponse(_roomManager.RemoveFromRoom(currentUser) ? "Вы успещно вышли из комнаты" : "Вы не состояли ни в какой комнате");
        }



        private async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogWarning("Некоррекнтый пароль");
                return null;
            }
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
