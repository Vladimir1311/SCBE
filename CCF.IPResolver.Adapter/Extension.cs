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

        public static IApplicationBuilder UseAsServise<T>(this IApplicationBuilder app)
        {
            var service = app.ApplicationServices.GetService<T>();
            service = service == null ? throw new ArgumentException($"service {typeof(T).FullName} cannot be found in service provider") : service;
            return app.UseAsServise(service);
        }

        public static IServiceCollection AddCCFService<T>(this IServiceCollection services) where T : class
        {
            services.AddTransient(SP => CCFServicesManager.GetService<T>());
            return services;
        }
    }
}
