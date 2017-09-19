using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CCF.IPResolver.Adapter
{
    public static class Extension
    {

        public static IServiceCollection UseInstanceAsServise<T>(this IServiceCollection services, T service) where T : class
        {
            CCFServicesManager.RegisterService(() => service);
            return services;
        }

        public static IServiceCollection UseAsServise<T, I>(this IServiceCollection app) where T : class where I : class, T =>
            UseAsTransientServise<T, I>(app);

        public static IServiceCollection UseAsTransientServise<T, I>(this IServiceCollection services) where T : class where I : class, T
        {
            services.AddTransient<T, I>();
            Func<T> serviceCreationFunc = () =>
                services.FirstOrDefault(D => D.ImplementationType == typeof(I)).ImplementationInstance as T;

            CCFServicesManager.RegisterService(serviceCreationFunc);
            return services;
        }


        public static IServiceCollection UseAsSingleTonServise<T, I>(this IServiceCollection services) where T : class where I : class, T
        {
            services.AddSingleton<T, I>();
            var service = services.FirstOrDefault(D => D.ImplementationType == typeof(T)).ImplementationInstance as I; 
            return services.UseInstanceAsServise(service);
        }

        public static IServiceCollection AddCCFService<T>(this IServiceCollection services) where T : class
        {
            services.AddTransient(SP => CCFServicesManager.GetService<T>());
            return services;
        }
    }
}
