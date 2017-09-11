using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SituationCenter.Shared.Exceptions;
using SituationCenter.Shared.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationCenterCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiExceptionsHandler(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<ApiExceptionsMiddleware>();
            return appBuilder;
        }

        private class ApiExceptionsMiddleware
        {
            private readonly RequestDelegate _next;

            public ApiExceptionsMiddleware(RequestDelegate next)
            {
                _next = next;
            }
            public Task Invoke(HttpContext context)
            {
                if (!context.Request.Path.StartsWithSegments(new PathString("/api/v1")))
                    return _next(context);

                ResponseBase responseObj = null;
                try
                {
                    var t = _next(context);
                    return t;
                }
                catch (StatusCodeException scException)
                {
                    responseObj = ResponseBase.BadResponse(scException.StatusCode);
                }
                catch (MultiStatusCodeException mscException)
                {
                    responseObj = ResponseBase.BadResponse(mscException.Codes);
                }
                catch (ArgumentException)
                {
                    responseObj = ResponseBase.BadResponse(StatusCode.ArgumentsIncorrect);
                }
                catch (NotImplementedException)
                {
                    responseObj = ResponseBase.BadResponse(StatusCode.NotImplementFunction);
                }
                catch
                {
                    responseObj = ResponseBase.BadResponse(StatusCode.UnknownError);
                }
                var toWrite = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseObj));
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.Body.Write(toWrite, 0, toWrite.Length);
                return Task.CompletedTask;
            }
        }
    }
}
