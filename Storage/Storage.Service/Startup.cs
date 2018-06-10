using CCF.IPResolver.Adapter;
using DocsToPictures.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SituationCenterBackServer.Interfaces;
using Storage.Interfaces;
using Storage.Service.Utilites;
using Swashbuckle.AspNetCore.Swagger;
using URSA;

namespace Storage.Service
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<StorageSetting>(Configuration.GetSection("Storage"));
            services.Configure<PageManagerSetting>(Configuration.GetSection("PageManagerSetting"));
            
            services.AddURSACollector();
            
            services.AddTransient<Interfaces.IFileSystem, RealFileSystem>();
            services.AddSingleton<IDocumentPageManager, DocumentPageManager>();
            services.AddTransient<IAccessValidator, FakeTrueAccessValidator>();
            
            services.AddTransientSafeCFF<IDocumentProcessor>(null);
            services.AddMvc();

            if (true)
                services.UseAsTransientServise<IStorage, DocumentSupportStorage>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("Storage", new Info { Title = "Storage API" });
                c.OperationFilter<FileUploadOperation>();
                c.OperationFilter<StorageControllerFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvcWithDefaultRoute();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/Storage/swagger.json", "Storage API");
                });
            }
        }
    }
}
