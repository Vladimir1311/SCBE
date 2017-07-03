using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace SituationCenterBackServer.Models.TokenAuthModels
{
    public class AuthOptions
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(50);
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }

    }

}
