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
using Newtonsoft.Json;

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
        private readonly AuthOptions authOptions;

        public UnrealApiController(UserManager<ApplicationUser> userManager,
            IOptions<UnrealAPIConfiguration> unrealApiOptions,
            IRoomManager roomManager,
            IOptions<AuthOptions> authOptions,
            ILogger<UnrealApiController> logger)
        {
            _userManager = userManager;
            _config = unrealApiOptions.Value;
            _roomManager = roomManager;
            _logger = logger;
            this.authOptions = authOptions.Value;
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
                issuer: authOptions.Issuer,
                audience: authOptions.Audience,
                notBefore: DateTime.Now,
                claims: identity.Claims,
                expires: DateTime.Now.Add(authOptions.Expiration),
                signingCredentials: new SigningCredentials(
                    authOptions.GetSymmetricSecurityKey(),
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
            return new GetRoomsInfo() { Rooms = _roomManager.Rooms };
        }

        [HttpPost]
        public async Task<ResponseData> CreateRoom([FromBody]CreateRoomInfo info)
        {
            var name = info.Name;
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                return ResponseData.ErrorRequest("Неправильное имя комнаты");
            try
            {
                _logger.LogInformation("Request for creating room with name " + name);
                var currentUser = await _userManager.GetUserAsync(User);
                var (room, clientId) = _roomManager.CreateNewRoom(Guid.Empty, new Common.Requests.Room.CreateRoom.CreateRoomRequest { Name = name});
                return new SignInRoomInfo()
                {
                    ClientId = clientId,
                    RoomId = room.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error while creating room {ex.Message}");
                return ResponseData.ErrorRequest(ex.Message);
            }
        }

        public async Task<ResponseData> JoinToRoom(Guid roomId)
        {
            try
            {
                //Выбрать один тдентификатор комнаты
                var currentUser = await _userManager.GetUserAsync(User);
                _logger.LogDebug($"client {currentUser.Id} try join in room {roomId}");
                (Room room, byte ClientId) returned;
                if (roomId != null)
                    returned = _roomManager.JoinToRoom(currentUser, roomId, "");
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
            _logger.LogInformation($"try leave from room user id: {userId}");
            return ResponseData.GoodResponse("");
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
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            return (new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType),
                user.Id);

        }
    }

    public class CreateRoomInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
