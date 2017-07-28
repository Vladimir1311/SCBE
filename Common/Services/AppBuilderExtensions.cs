using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Common.Services
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder RegisterAsService(this IApplicationBuilder app,
            ServiceTypes serviceType,
            string IPResolverHost,
            ILogger logger)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var jsonString = JsonConvert.SerializeObject(new
                    {
                        token = GlobalTokens.RegisterServiseToken,
                        serviceType = serviceType.ToString()
                    });
                    logger.LogDebug($"Registration string is {jsonString}");
                    StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    var result = client.PostAsync($"http://{IPResolverHost}/ip/register", content).Result;
                    logger.LogInformation($"result from registration {(int)result.StatusCode} {result.StatusCode}");
                    if (result.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception("Can't register service!!!");
                }
            }
            catch
            {
                Console.WriteLine("ERROR BLYAT");
            }
            return app;

        }

    }
}
