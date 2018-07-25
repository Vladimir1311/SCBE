using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SituationCenter.Shared.ResponseObjects.People
{
    public class PersonView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public List<RoleView> Roles { get; set; }
    }
}