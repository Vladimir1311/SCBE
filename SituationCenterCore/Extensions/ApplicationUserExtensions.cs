
using SituationCenter.Shared.Models.People;
using SituationCenterCore.Data;

namespace SituationCenterCore.Extensions
{
    public static class ApplicationUserExtensions
    {
        public static PersonPresent ToPresent(this ApplicationUser user)
            => new PersonPresent(user.Name, user.Surname, user.PhoneNumber, user.Email);
    }
}