using Newtonsoft.Json;
using SituationCenter.Shared.Models.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using SituationCenter.Shared.ResponseObjects.People;

namespace SituationCenter.Shared.ResponseObjects.Rooms
{
    public class RoomView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UsersCount { get; set; }
        public int PeopleCountLimit { get; set; }
        public PrivacyRoomType PrivacyType { get; set; }
        public List<PersonView> Users { get; set; }
    }
}
