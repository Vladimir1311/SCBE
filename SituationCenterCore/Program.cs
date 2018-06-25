using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SituationCenterCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args);
            builder.UseConfiguration(new ConfigurationBuilder()
                                .AddJsonFile("appsettings.Secret.json", optional: true)
                                .Build())
                    .UseStartup<Startup>();
            return builder.Build();
        }
    }
}
