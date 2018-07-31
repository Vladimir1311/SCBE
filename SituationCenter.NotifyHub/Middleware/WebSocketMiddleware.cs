using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SituationCenter.NotifyHub.Services.Interfaces;

namespace SituationCenter.NotifyHub.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string startRoute;

        public WebSocketMiddleware(RequestDelegate next, string startRoute)
        {
            this.next = next;
            this.startRoute = startRoute;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest && httpContext.Request.Path.StartsWithSegments(startRoute))
            {
                var userId = Guid.Parse(httpContext.Request.Query["userId"]);
                await httpContext.RequestServices.GetService<IWebSocketHandler>()
                                 .Handle(await httpContext.WebSockets.AcceptWebSocketAsync(), userId);
                return;
            }
            await next(httpContext);
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
