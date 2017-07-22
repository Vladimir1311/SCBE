using Common.Requests.Room.CreateRoom;
using Common.ResponseObjects;
using Common.ResponseObjects.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Extensions;
using SituationCenterBackServer.Filters;
using SituationCenterBackServer.Models;
using SituationCenterBackServer.Models.RoomSecurity;
using SituationCenterBackServer.Models.VoiceChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Controllers.API.V1
{
    
    [Authorize]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    [TypeFilter(typeof(JsonExceptionsFilterAttribute))]
    public class RoomsController : Controller
    {
        private readonly IRoomManager roomsManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRoomSecurityManager roomSecyrityManager;

        public RoomsController(IRoomManager roomsManager,
            UserManager<ApplicationUser> userManager,
            IRoomSecurityManager roomSecyrityManager)
        {
            this.roomsManager = roomsManager;
            this.userManager = userManager;
            this.roomSecyrityManager = roomSecyrityManager;
            roomsManager.SaveState += (U) => userManager.UpdateAsync(U);
        }
        public ResponseBase List()
        {
            var roomsTuples = roomsManager.Rooms
                .Select(R => new RoomPresent(R.Id, R.Name));
            return RoomsListResponse.Create(roomsTuples);
        }

        [HttpPost]
        public async Task<ResponseBase> Create([FromBody]CreateRoomRequest info)
        {
            var user = await userManager.FindByIdAsync(userManager.GetUserId(User));
            roomsManager.CreateNewRoom(user, info);
            return ResponseBase.GoodResponse();
        }

        public async Task<ResponseBase> Join(Guid roomId, string data = null)
        {
            var user = await userManager.FindUser(User);
            roomsManager.JoinToRoom(user, roomId, data);
            return ResponseBase.GoodResponse();
        }

        public async Task<ResponseBase> Leave()
        {
            var user = await userManager.FindUser(User);
            roomsManager.LeaveFromRoom(user);
            return ResponseBase.GoodResponse();
        }
    }
}
