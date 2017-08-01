using Common.People;
using SituationCenterBackServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Extensions
{
    public static class ApplicationUserExtensions
    {

        public static PersonPresent ToPresent(this ApplicationUser user)
            => new PersonPresent(user.Name, user.Surname, user.PhoneNumber, user.Email);
    }
}
