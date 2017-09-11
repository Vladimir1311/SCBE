using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CCF.IPResolver.Adapter
{
    public static class Extension
    {
        public static IApplicationBuilder UseAsServise<T>(this IApplicationBuilder app, T service)
        {
            service = service == null ? throw new ArgumentNullException($"service cannot be null {nameof(service)}") : service;
            CCFServicesManager.RegisterService(service);
            return app;
        }


        public static IServiceCollection AddCCFService<T>(this IServiceCollection services) where T : class
        {
            services.AddTransient(SP => CCFServicesManager.GetService<T>());
            return services;
        }
    }
}
