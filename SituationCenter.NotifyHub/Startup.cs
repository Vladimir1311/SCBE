using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SituationCenter.NotifyHub.Middleware;
using SituationCenter.NotifyHub.Services.Implementations;
using SituationCenter.NotifyHub.Services.Interfaces;
using SituationCenter.NotifyHub.Models.Settings;
using System.Runtime.InteropServices.ComTypes;

namespace SituationCenter.NotifyHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AccessSettings>(Configuration.GetSection(nameof(AccessSettings)));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IWebSocketManager, WebSocketManager>();
            services.AddTransient<IWebSocketHandler, WebSocketHandler>();
            services.AddTransient<INotificator, WebSocketNotificator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.UseWebSocketMiddleware("/ws");

            app.Use(async (ctx, next) =>
            {

                if (ctx.Request.Headers.TryGetValue("Authorization", out var key))
                {
                    var keySettings = ctx.RequestServices.GetService<IOptions<AccessSettings>>();
                    if (key == keySettings.Value.AccessKey)
                    {
                        await next();
                        return; 
                    }
                }
                ctx.Response.StatusCode = 403;
            });
            app.UseMvc();
        }
    }
}
