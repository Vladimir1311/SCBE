using BlazorFront.Services.Account;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            });

            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }
    }
}
