using Microsoft.AspNetCore.Blazor.Browser.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorFront.Services.General
{
    public class MyHttpClient : HttpClient
    {
        public MyHttpClient() : base(new MyBrowserHttpMessageHandler())
        {
            Console.WriteLine("I AM SENDER CONSTRUCTOR! ");
            BaseAddress = new Uri("http://localhost:5000/");
        }
        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("I AM SENDER! ");
            var result = default(HttpResponseMessage);
            try
            {
                result = await base.SendAsync(request, cancellationToken);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Not normal StatusCode!");
                }
                return result;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.StackTrace);
            }
            return result;
        }
    }
}
