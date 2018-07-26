using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace SituationCenterCore.Models.TokenAuthModels
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = "very 123strong 65secret 235key_=1@";
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromSeconds(10);

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}