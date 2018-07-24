using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SituationCenter.Shared.Models.Rooms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Requests.Room.CreateRoom
{
    public class CreateRoomRequest
    {
        public string Name { get; set; }
        public PrivacyRoomType PrivacyType { get; set; }
        public int PeopleCountLimit { get; set; } = 6;
        public string Password { get; set; }
        public List<Guid> InviteUsers { get; set; }
    }
}
