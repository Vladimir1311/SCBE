using System;

namespace SituationCenter.Shared.ResponseObjects.People
{
    public class MeAndRoom : ResponseBase
    {
        public Guid? RoomId { get; set; }
        public PersonView Me { get; set; }
    }
}