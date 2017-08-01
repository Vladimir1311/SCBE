using Common.ResponseObjects.Rooms;
using SituationCenterBackServer.Models.VoiceChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Extensions
{
    public static class Room
    {
        public static RoomPresent ToRoomPresent(this SituationCenterBackServer.Models.VoiceChatModels.Room room) =>
            new RoomPresent(
                id: room.Id,
                name: room.Name,
                userCount: room.Users.Count,
                privacy: room.SecurityRule.PrivacyRule,
                peopleCountLimit: room.PeopleCountLimit
                );
    

    }
}
