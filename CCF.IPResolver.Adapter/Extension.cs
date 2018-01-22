using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CCF.IPResolver.Adapter
{
    public static class Extension
    {
        public static CCFCollection AddCCF(this IServiceCollection services, CCFServicesManager.Params @params = null)
        {
            @params = @params ?? CCFServicesManager.Params.Default;
            return new CCFCollection(services, services.BuildServiceProvider().GetService<ILoggerFactory>(), @params);
        }
    }
}
