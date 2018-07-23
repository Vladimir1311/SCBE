using Common.Requests.Room.CreateRoom;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenter.Shared.Exceptions;
using SituationCenter.Shared.ResponseObjects;
using SituationCenter.Shared.ResponseObjects.People;
using SituationCenter.Shared.ResponseObjects.Rooms;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Extensions;
using SituationCenterCore.Models.Rooms;
using SituationCenterCore.Models.Rooms.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using Exceptions = SituationCenter.Shared.Exceptions;
using System.Collections.Generic;
using AutoMapper;
using SituationCenter.Shared.ResponseObjects.General;

namespace SituationCenterCore.Controllers.API.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    public class RoomsController : Controller
    {
        private readonly IRoomManager roomsManager;
        private readonly IRepository repository;
        private readonly IRoomSecurityManager roomSecyrityManager;
        private readonly IMapper mapper;

        public RoomsController(IRoomManager roomsManager,
            IRepository userManager,
            IRoomSecurityManager roomSecyrityManager,
            IMapper mapper)
        {
            this.roomsManager = roomsManager;
            this.repository = userManager;
            this.roomSecyrityManager = roomSecyrityManager;
            this.mapper = mapper;
        }

        public ListResponse<RoomView> List()
        {
            var userId = repository.GetUserId(User);
            var roomsPresent = roomsManager
                .Rooms(userId)
                .Select(mapper.Map<RoomView>)
                .ToList();
            return roomsPresent;
        }

        [HttpPost]
        public OneObjectResponse<Guid> Create([FromBody] CreateRoomRequest info)
        {
            var room = roomsManager.CreateNewRoom(repository.GetUserId(User), info);
            return room.Id;
        }

        public ResponseBase Join(Guid roomId, string data = null)
        {
            roomsManager.JoinToRoom(repository.GetUserId(User), roomId, data);
            return ResponseBase.OKResponse;
        }

        public ResponseBase Leave()
        {
            var userId = repository.GetUserId(User);
            roomsManager.LeaveFromRoom(userId);
            return ResponseBase.OKResponse;
        }

        public ResponseBase Delete(Guid roomId)
        {
            var userId = repository.GetUserId(User);
            roomsManager.DeleteRoom(userId, roomId);
            return ResponseBase.OKResponse;
        }

        public OneObjectResponse<RoomView> Info(Guid roomId)
        {
            var room = roomsManager.FindRoom(roomId) ??
                       throw new StatusCodeException(Exceptions.StatusCode.DontExistRoom);
            return mapper.Map<RoomView>(room);
        }

        public async Task<OneObjectResponse<RoomView>> Current()
        {
            var user = await repository.FindUser(User);
            var room = roomsManager.FindRoom(user.RoomId ?? Guid.Empty) ??
                       throw new StatusCodeException(Exceptions.StatusCode.YouAreNotInRoom);
            return mapper.Map<RoomView>(room);
        }

        [HttpPost]
        public async Task<ResponseBase> InvitePerson([FromBody]List<string> phones)
        {
            var user = await repository.FindUser(User);
            var currentRoomId = user.RoomId ?? 
                        throw new StatusCodeException(Exceptions.StatusCode.YouAreNotInRoom);
            await roomsManager.InviteUsersByPhoneToRoom(currentRoomId, phones);
            return ResponseBase.OKResponse;
        }
    }
}