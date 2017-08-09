using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CCF.IPResolver.Adapter
{
    internal class CCFAdapterMiddleware<T>
    {
        private readonly RequestDelegate _next;
        private ServiceCode serviceCode;
        private string endPoint;

        public CCFAdapterMiddleware(RequestDelegate next, string endPoint, T serviceInstance)
        {
            _next = next;
            serviceCode = ServiceCode.Create<T>(serviceInstance);
            this.endPoint = endPoint.StartsWith("/") ? endPoint : "/" + endPoint;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine(context.Request.Path);
            Console.WriteLine(endPoint);
            var pathString = new PathString(endPoint);
            Console.WriteLine(pathString.Value);
            if (context.Request.Path.StartsWithSegments(pathString))
            {
                var res = serviceCode.Handle(context.Request.Form["simpleargs"], context.Request.Form.Files.Select(F => new StreamValue { Name = F.Name, Value = F.OpenReadStream() }));
                switch (res)
                {
                    case string stringResult:
                        await context.Response.WriteAsync(stringResult);
                        break;

                    case Stream streamResult:
                        await streamResult.CopyToAsync(context.Response.Body);
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                        break;
                }
                return;
            }
            await _next(context);
        }
    }
}
