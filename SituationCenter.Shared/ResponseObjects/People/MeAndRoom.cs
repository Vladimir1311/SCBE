using System;
using SituationCenter.Shared.ResponseObjects.Rooms;

namespace SituationCenter.Shared.ResponseObjects.People
{
    public class MeAndRoom : ResponseBase
    {
        public CompactRoomView Room { get; set; }
        public PersonView Me { get; set; }
    }
}