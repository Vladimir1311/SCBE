using BlazorFront.Services.Account;
using BlazorFront.Services.General;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;

namespace BlazorFront
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
            {
                services.AddStorage();
                services.AddSingleton<UserState>();
                services.AddTransient<AccountBridge>();
                ReplaceHttpClient(services);
            });

            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }

        static void ReplaceHttpClient(IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(HttpClient));
            services.Remove(descriptor);
            services.AddSingleton<HttpClient, MyHttpClient>();
        }
    }
}
