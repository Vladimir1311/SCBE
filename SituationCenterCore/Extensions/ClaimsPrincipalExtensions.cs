using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SituationCenterCore.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid Id(this ClaimsPrincipal principal)
        {
            var result = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(result, out var guid))
                return guid;
            throw new Exception();
        }
        public static Guid? RefreshTokenId(this ClaimsPrincipal principal)
        {
            var result = principal.Claims.SingleOrDefault(c => c.Type == "RefreshTokenId")?.Value;
            if (Guid.TryParse(result, out var guid))
                return guid;
            return null;
        }
    }
}
