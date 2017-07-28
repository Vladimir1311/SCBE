using SituationCenterBackServer.Models.VoiceChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.RoomSecurity
{
    public interface IRoomSecurityManager
    {
        void Validate(ApplicationUser user, Room room, string data);
        void CreatePublicRule(Room room);
        void CreatePasswordRule(Room room, string pasword);
        void CreateInvationRule(Room room, IEnumerable<ApplicationUser> users);
        
    }
}
