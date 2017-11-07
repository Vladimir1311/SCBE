﻿using Common.Requests.Room.CreateRoom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenter.Shared.Exceptions;
using SituationCenter.Shared.ResponseObjects;
using SituationCenter.Shared.ResponseObjects.People;
using SituationCenter.Shared.ResponseObjects.Rooms;
using SituationCenterBackServer.Extensions;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Extensions;
using SituationCenterCore.Filters;
using SituationCenterCore.Models.Rooms;
using SituationCenterCore.Models.Rooms.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using URSA.Respose;
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

        public URespose<RoomsList> List()
        {
            var userId = repository.GetUserId(User);
            var roomsPresent = roomsManager
                .Rooms(userId)
                .Select(R => R.ToRoomPresent());
            return RoomsList.Create(roomsPresent);
        }

        [HttpPost]
        public URespose<RoomCreate> Create([FromBody] CreateRoom info)
        {
            var room = roomsManager.CreateNewRoom(repository.GetUserId(User), info);
            return RoomCreate.Create(room.Id);
        }

        public URespose Join(Guid roomId, string data = null)
        {
            roomsManager.JoinToRoom(repository.GetUserId(User), roomId, data);
            return URespose.GoodResponse();
        }

        public URespose Leave()
        {
            var userId = repository.GetUserId(User);
            roomsManager.LeaveFromRoom(userId);
            return URespose.GoodResponse();
        }

        public URespose Delete(Guid roomId)
        {
            var userId = repository.GetUserId(User);
            roomsManager.DeleteRoom(userId, roomId);
            return URespose.GoodResponse();
        }

        public URespose<RoomInfo> Info(Guid roomId)
        {
            var room = roomsManager.FindRoom(roomId) ??
                       throw new StatusCodeException(Exceptions.StatusCode.DontExistRoom);
            return RoomInfo.Create(room.ToRoomPresent());
        }

        public async Task<URespose<UsersList>> Current()
        {
            var user = await repository.FindUser(User);
            var room = roomsManager.FindRoom(user.RoomId ?? Guid.Empty) ??
                       throw new StatusCodeException(Exceptions.StatusCode.YouAreNotInRoom);
            return UsersList.Create(
                room
                    .Users
                    .Where(U => U.Id != user.Id)
                    .Select(U => U.ToPresent()));
        }

    }
}