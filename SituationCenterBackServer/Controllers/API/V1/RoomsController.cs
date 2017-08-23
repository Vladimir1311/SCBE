using Common.Exceptions;
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
using System.Linq;

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
        }

        public ResponseBase List()
        {
            var userId = userManager.GetUserGuid(User);
            var roomsPresent = roomsManager
                .Rooms(userId)
                .Select(R => R.ToRoomPresent());
            return RoomsListResponse.Create(roomsPresent);
        }

        [HttpPost]
        public ResponseBase Create([FromBody]CreateRoomRequest info)
        {
            var room = roomsManager.CreateNewRoom(Guid.Parse(userManager.GetUserId(User)), info);
            return RoomCreate.Create(room.Id);
        }

        public ResponseBase Join(Guid roomId, string data = null)
        {
            roomsManager.JoinToRoom(userManager.GetUserGuid(User), roomId, data);
            return ResponseBase.GoodResponse();
        }

        public ResponseBase Leave()
        {
            var userId = userManager.GetUserGuid(User);
            roomsManager.LeaveFromRoom(userId);
            return ResponseBase.GoodResponse();
        }

        public ResponseBase Delete(Guid roomId)
        {
            var userId = userManager.GetUserGuid(User);
            roomsManager.DeleteRoom(userId, roomId);
            return ResponseBase.GoodResponse();
        }

        public ResponseBase Info(Guid roomId)
        {
            var room = roomsManager.FindRoom(roomId) ?? throw new StatusCodeException(Common.ResponseObjects.StatusCode.DontExistRoom);
            return RoomInfoResponse.Create(room.ToRoomPresent());
        }
    }
}