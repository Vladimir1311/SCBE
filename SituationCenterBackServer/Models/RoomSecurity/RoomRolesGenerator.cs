using SituationCenterBackServer.Models.VoiceChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
