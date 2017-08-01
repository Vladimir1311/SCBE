using Common.ResponseObjects.Rooms;

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