using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCF.IPResolver.Adapter
{
    public class CCFCollection
    {
        private CCFServicesManager ccfManager;
        private readonly IServiceCollection services;

        public CCFCollection(IServiceCollection services, ILoggerFactory factory, CCFServicesManager.Params @params)
        {
            ccfManager = new CCFServicesManager(factory, @params);
            this.services = services;
        }
        private CCFCollection UseInstanceAsServise<T>(T service) where T : class
        {
            ccfManager.RegisterService(() => service);
            return this;
        }

        public CCFCollection UseAsServise<T, I>()
            where T : class
            where I : class, T =>
            UseAsTransientServise<T, I>();

        public CCFCollection UseAsTransientServise<T, I>()
            where T : class
            where I : class, T
        {
            services.AddTransient<T, I>();
            var provider = services.BuildServiceProvider();
            Func<T> serviceCreationFunc = provider.GetService<T>;

            ccfManager.RegisterService(serviceCreationFunc);
            return this;
        }


        public CCFCollection UseAsSingletonServise<T, I>()
            where T : class
            where I : class, T
        {
            services.AddSingleton<T, I>();
            var service = services.BuildServiceProvider().GetService<T>();
            return UseInstanceAsServise(service);
        }

        public CCFCollection AddCCFService<T>()
            where T : class
        {
            services.AddTransient(SP => ccfManager.GetService<T>());
            return this;
        }
    }
}
