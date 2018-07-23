using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SituationCenterCore.Data;
using SituationCenter.Shared.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace SituationCenterCore.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ValidateRefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateRefreshTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var refreshTokenString = httpContext.User.Claims.FirstOrDefault(c => c.Type == "RefreshTokenId")?.Value;
            if (refreshTokenString == null)
                await _next(httpContext);
            if (!Guid.TryParse(refreshTokenString, out var refreshTokenId) ||
                httpContext.RequestServices.GetService<ApplicationDbContext>().RemovedTokens.Any(rt => rt.Id == refreshTokenId))
                throw new StatusCodeException(StatusCode.IncorrectRefreshToken);
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ValidateRefreshTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidateRefreshToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateRefreshTokenMiddleware>();
        }
    }
}
