using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;



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


        public static IServiceCollection AddCCFService<T>(this IServiceCollection services) where T : class
        {
            using (var client = new HttpClient())
            {
                var result = client.GetStringAsync($"http://ipresolver.azurewebsites.net/ip/GetCCFEndPoint?interfaceName={typeof(T)}").Result;
                if (result == "") throw new Exception($"No availabale service for {typeof(T)}");
                var worker = RemoteWorker.Create<T>(result);
                services.AddSingleton(worker);
            }

            return services;
        }
    }
}
