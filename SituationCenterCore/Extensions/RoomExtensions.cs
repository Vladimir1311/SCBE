
using System.Linq;
using SituationCenter.Shared.ResponseObjects.Rooms;
using SituationCenterCore.Extensions;
using SituationCenterCore.Models.Rooms;

namespace SituationCenterBackServer.Extensions
{
    public static class RoomExtensions
    {
        public static RoomPresent ToRoomPresent(this Room room) =>
            new RoomPresent(
                id: room.Id,
                name: room.Name,
                userCount: room.Users.Count,
                privacy: room.SecurityRule.PrivacyRule,
                peopleCountLimit: room.PeopleCountLimit,
            users: room.Users.Select(U => U.ToPresent() )
        );
    }
}