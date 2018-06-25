using CCF;
using CCF.Shared.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Service
{
    public static class Extensions
    {
        public static void AddTransientSafeCFF<T>(this IServiceCollection services, Func<IServiceProvider, T> AlternativeFactory) where T : class
        {
            services.AddTransient(SP =>
            {
                try
                {
                    return CCFServicesManager.GetService<T>();
                }
                catch
                {
                    return AlternativeFactory?.Invoke(SP);
                }
            });
        }
    }
}
