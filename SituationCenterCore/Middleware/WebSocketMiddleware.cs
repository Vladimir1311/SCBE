using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SituationCenterCore.Services.Interfaces.RealTime;
using SituationCenterCore.Extensions;

namespace SituationCenterCore.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string startRoute;

        public WebSocketMiddleware(RequestDelegate next, string startRoute)
        {
            _next = next;
            this.startRoute = startRoute;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest && httpContext.Request.Path.StartsWithSegments(startRoute))
            {
                var userId = Guid.Parse(httpContext.Request.Query["userId"]);
                var authResult = httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (authResult.IsFaulted)
                    return;
                await httpContext.RequestServices.GetService<IWebSocketHandler>()
                                 .Handle(await httpContext.WebSockets.AcceptWebSocketAsync(), userId);
                return;
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder builder, string startRoute)
        {
            return builder.UseMiddleware<WebSocketMiddleware>(startRoute);
        }
    }
}
