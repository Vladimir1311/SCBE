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
using SituationCenterBackServer.Filters;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    [TypeFilter(typeof(JsonExceptionsFilterAttribute))]
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseData> GetToken([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Не переданы email и/или пароль");
                return ResponseData.ErrorRequest("not correct email or password");
            }
            var (identity, userid) = await GetIdentity(model.Email, model.Password);
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
                TcpPort = _config.TcpPort,
                UdpPort = _config.UdpPort,
                ForConnection = userid
            };
        }
        
        public ResponseData GetRoomsData()
        {
            _logger.LogInformation("Request for Room list");
            return new GetRoomsInfo() { Rooms = _roomManager.Rooms};
        }
        public async Task<ResponseData> CreateRoom(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                return ResponseData.ErrorRequest("Неправильное имя комнаты");
            try
            {
                _logger.LogInformation("Request for creating room with name " + name);
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
                _logger.LogWarning($"Error while creating room {ex.Message}");
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

                LeaveTheRoom();         // Кривошея

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
        public ResponseData LeaveTheRoom()
        {
            var userId = _userManager.GetUserId(User);
            return ResponseData.GoodResponse(_roomManager.RemoveFromRoom(userId) ? "Вы успешно вышли из комнаты" : "Вы не состояли ни в какой комнате");
        }


        private async Task<(ClaimsIdentity, string)> GetIdentity(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (null, null);
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogWarning("Некоррекнтый пароль");
                return (null, null);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };
            return (new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType),
                user.Id);

        }
    }
}
