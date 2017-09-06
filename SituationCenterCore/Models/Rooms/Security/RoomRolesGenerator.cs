namespace SituationCenterCore.Models.Rooms.Security
{
    public class RoomRolesGenerator
    {
        public string GetAdministratorRole(Room room) =>
            $"{room.Id}__ADMIN";

        public string GetMemberRole(Room room) =>
            $"{room.Id}__MEMBER";
    }
}