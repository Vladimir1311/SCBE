using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SituationCenterBackServer.Models.TokenAuthModels
{
    public class AuthOptions
    {
        public const string ISSUER = "MaksAuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:51884/"; // потребитель токена
        const string KEY = "FirstTestVoiceChat";   // ключ для шифрации
        public const int LIFETIME = 60; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
