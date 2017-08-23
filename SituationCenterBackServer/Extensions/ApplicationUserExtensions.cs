using Common.People;
using SituationCenterBackServer.Models;

namespace SituationCenterBackServer.Extensions
{
    public static class ApplicationUserExtensions
    {
        public static PersonPresent ToPresent(this ApplicationUser user)
            => new PersonPresent(user.Name, user.Surname, user.PhoneNumber, user.Email);
    }
}