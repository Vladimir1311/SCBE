using Common.ResponseObjects;
using Common.ResponseObjects.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Filters;
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
    public class RoomsController
    {
        private readonly IRoomManager roomsManager;

        public RoomsController(IRoomManager roomsManager)
        {
            this.roomsManager = roomsManager;
        }
        public ResponseBase RoomsList()
        {
            var roomsTuples = roomsManager.Rooms
                .Select(R => new RoomPresent(R.Id, R.Name));
            return RoomsListResponse.Create(roomsTuples);
        }
    }
}
