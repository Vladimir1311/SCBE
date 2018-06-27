using Microsoft.AspNetCore.Blazor.Browser.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorFront.Services.General
{
    public class MyBrowserHttpMessageHandler : BrowserHttpMessageHandler
    {
        public MyBrowserHttpMessageHandler()
        {
            Console.WriteLine("I AM MESSAGE HANDLER CONSTRUCTOR");
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("I AM MESSAGE HANDLER CONSTRUCTOR SENDASYNC");

            try
            {
                Console.WriteLine("I AM MESSAGE HANDLER CONSTRUCTOR SENDASYNC");
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.StackTrace);
                return default;
            }
        }
    }
}
