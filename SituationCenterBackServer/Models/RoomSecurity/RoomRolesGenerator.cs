using SituationCenterBackServer.Models.VoiceChatModels;

namespace SituationCenterBackServer.Models.RoomSecurity
{
    public class RoomRolesGenerator
    {
        public string GetAdministratorRole(Room room)
        {
            return $"{room.Id}__ADMIN";
        }
    }
}