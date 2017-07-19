using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models.Rooms
{
    public class RoomSecurity
    {
        public Guid Id { get; set; }
        public PrivacyRoomType PrivacyRule { get; set; }
        public string Data { get; set; }
    }
}
