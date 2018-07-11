using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SituationCenter.Shared.Exceptions;
using SituationCenter.Shared.ResponseObjects;

namespace SituationCenterCore.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionsHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            ResponseBase responseObj = default;

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case StatusCodeException scException:
                        responseObj = ResponseBase.BadResponse(scException.StatusCode);
                        break;
                    case MultiStatusCodeException mscException:
                        responseObj = ResponseBase.BadResponse(mscException.Codes);
                        break;
                    case ArgumentException argException:
                        responseObj = ResponseBase.BadResponse(StatusCode.ArgumentsIncorrect);
                        break;
                    case NotImplementedException niException:
                        responseObj = ResponseBase.BadResponse(StatusCode.NotImplementFunction);
                        break;
                    default:
                        responseObj = ResponseBase.BadResponse(StatusCode.UnknownError);
                        break;
                }
            }

            if (httpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                responseObj = ResponseBase.BadResponse(StatusCode.Unauthorized);
            }

            if (responseObj != null)
            {
                httpContext.Response.StatusCode = 200;
                var toWrite = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseObj));
                httpContext.Response.ContentType = "application/json; charset=utf-8";
                httpContext.Response.Body.Write(toWrite, 0, toWrite.Length);
            }

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionsHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionsHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionsHandlerMiddleware>();
        }
    }
}
