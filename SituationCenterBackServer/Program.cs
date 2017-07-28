using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SituationCenterBackServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string urls = "http://*:80";
            if (args.Length >= 1 && args[0] == "release")
                urls = "http://localhost:5000";
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(urls)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
