using System;
using SituationCenterCore.Data;
namespace SituationCenterCore.Models.TokenAuthModels
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
