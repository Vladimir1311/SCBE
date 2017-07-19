using Common.Models.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models
{
    public class RoomSecurity
    {
        public Guid Id { get; set; }
        public PrivacyRoomType PrivacyRule { get; set; }
        public string Data { get; set; }
    }
}
