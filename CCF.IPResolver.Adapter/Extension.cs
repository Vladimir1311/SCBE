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
            var serviceInvoker = app.ApplicationServices.GetService<T>();
            CCFServicesManager.RegisterService(serviceInvoker);
            return app;
        }


        public static IServiceCollection AddCCFService<T>(this IServiceCollection services) where T : class
        {
            services.AddTransient(SP => CCFServicesManager.GetService<T>());
            return services;
        }
    }
}
