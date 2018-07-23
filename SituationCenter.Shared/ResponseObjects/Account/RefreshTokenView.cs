using System;

namespace SituationCenter.Shared.ResponseObjects.Account
{
    public class RefreshTokenView
    {
        public Guid Id { get; set; }

        public string UserAgent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }

}
