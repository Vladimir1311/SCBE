using Microsoft.AspNetCore.Builder;
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
            string IPResolverHost)
        {
            using (var client = new HttpClient())
            {
                var jsonString = JsonConvert.SerializeObject(new
                {
                    token = GlobalTokens.RegisterServiseToken,
                    serviceType = serviceType.ToString()
                });
                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var result = client.PostAsync($"http://{IPResolverHost}/ip/register", content).Result;
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("Can't register service!!!");
            }
            return app;

        }

    }
}
