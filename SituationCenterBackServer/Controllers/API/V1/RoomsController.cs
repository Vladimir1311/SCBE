using Common.Requests.Room;
using Common.ResponseObjects;
using Common.ResponseObjects.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Filters;
using SituationCenterBackServer.Models;
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

        public RoomsController(IRoomManager roomsManager,
            UserManager<ApplicationUser> userManager)
        {
            this.roomsManager = roomsManager;
            this.userManager = userManager;
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
            roomsManager.CreateNewRoom(user, info.Name);
            return ResponseBase.GoodResponse();
        }
    }
}
