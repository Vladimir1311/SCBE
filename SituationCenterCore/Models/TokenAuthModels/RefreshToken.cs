using System;
using SituationCenterCore.Data;
namespace SituationCenterCore.Models.TokenAuthModels
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string TokenContent { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreateTime {get; set;}
        public DateTime LastUpdateTime { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
