using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Requests.Room.CreateRoom;
using SituationCenter.Shared.ResponseObjects.Rooms;
using SituationCenterCore.Models.Rooms;

namespace SituationCenterCore.DataFormatting.Mappings
{
    public class RoomMappingProfile : Profile
    {
        public RoomMappingProfile()
        {
            CreateMap<Room, RoomView>()
                .ForMember(rv => rv.PrivacyType, map => map.MapFrom(r => r.SecurityRule.PrivacyRule));

            CreateMap<CreateRoomRequest, Room>();
        }
    }
}
