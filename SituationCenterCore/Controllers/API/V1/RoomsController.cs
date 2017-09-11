﻿using Common.Requests.Room.CreateRoom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenter.Shared.Exceptions;
using SituationCenter.Shared.ResponseObjects;
using SituationCenter.Shared.ResponseObjects.Rooms;
using SituationCenterBackServer.Extensions;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Filters;
using SituationCenterCore.Models.Rooms;
using SituationCenterCore.Models.Rooms.Security;
using System;
using System.Linq;

using Exceptions = SituationCenter.Shared.Exceptions;

namespace SituationCenterCore.Controllers.API.V1
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    [TypeFilter(typeof(JsonExceptionsFilterAttribute))]
    public class RoomsController : Controller
    {
        private readonly IRoomManager roomsManager;
        private readonly IRepository repository;
        private readonly IRoomSecurityManager roomSecyrityManager;

        public RoomsController(IRoomManager roomsManager,
            IRepository userManager,
            IRoomSecurityManager roomSecyrityManager)
        {
            this.roomsManager = roomsManager;
            this.repository = userManager;
            this.roomSecyrityManager = roomSecyrityManager;
        }

        public ResponseBase List()
        {
            var userId = repository.GetUserId(User);
            var roomsPresent = roomsManager
                .Rooms(userId)
                .Select(R => R.ToRoomPresent());
            return RoomsListResponse.Create(roomsPresent);
        }

        [HttpPost]
        public ResponseBase Create([FromBody]CreateRoomRequest info)
        {
            var room = roomsManager.CreateNewRoom(repository.GetUserId(User), info);
            return RoomCreate.Create(room.Id);
        }

        public ResponseBase Join(Guid roomId, string data = null)
        {
            roomsManager.JoinToRoom(repository.GetUserId(User), roomId, data);
            return ResponseBase.GoodResponse();
        }

        public ResponseBase Leave()
        {
            var userId = repository.GetUserId(User);
            roomsManager.LeaveFromRoom(userId);
            return ResponseBase.GoodResponse();
        }

        public ResponseBase Delete(Guid roomId)
        {
            var userId = repository.GetUserId(User);
            roomsManager.DeleteRoom(userId, roomId);
            return ResponseBase.GoodResponse();
        }

        public ResponseBase Info(Guid roomId)
        {
            var room = roomsManager.FindRoom(roomId) ?? throw new StatusCodeException(Exceptions.StatusCode.DontExistRoom);
            return RoomInfoResponse.Create(room.ToRoomPresent());
        }
    }
}