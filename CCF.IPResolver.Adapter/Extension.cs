using Microsoft.AspNetCore.Builder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CCF.IPResolver.Adapter
{
    public static class Extension
    {
        public static IApplicationBuilder UseAsServise<T>(this IApplicationBuilder app, string endPoint)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = client.GetStringAsync($"http://ipresolver.azurewebsites.net/ip/SetCCFEndPoint?interfaceName={typeof(T).FullName}&url={endPoint}").Result;
                    if (result == "OK")
                    {
                        Console.WriteLine($"Registrate as {typeof(T).FullName} success");
                        app.UseMiddleware<CCFAdapterMiddleware<T>>(endPoint);
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
            }
            catch
            {
                Console.WriteLine("http request error!!! :(");
            }
            return app;
        }
    }
}
