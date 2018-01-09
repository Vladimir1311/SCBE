using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCF;
using DocsToPictures.Models;
using DocsToPictures.Interfaces;
using System.Threading;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DocsToPictures
{
    class Program
    {
        static IServiceProvider provider;
        static Program()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new LoggerFactory()
                .AddConsole());
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(CCFServicesManager.Params.LocalHost);
            serviceCollection.AddSingleton<CCFServicesManager>();
            provider = serviceCollection.BuildServiceProvider();
        }

        static void Main(string[] args)
        {
            var ccfManager = provider.GetService<CCFServicesManager>();

            IDocumentProcessor processor = new DocumentProcessor();
            ccfManager.RegisterService(() => processor);
            while (true)
            {
                Console.WriteLine("Go to wait!");
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
}
