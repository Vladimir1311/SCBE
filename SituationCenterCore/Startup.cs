using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SituationCenterCore.Data;
using SituationCenterCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Models.TokenAuthModels;
using SituationCenterCore.Models.Rooms;
using SituationCenterCore.Models.Rooms.Security;
using SituationCenterBackServer.Interfaces;
using System;
using AutoMapper;
using SituationCenterCore.Middleware;
using SituationCenterCore.Models.Settings;
using SituationCenterCore.Services.Interfaces;
using SituationCenterCore.Services.Implementations;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using SituationCenterCore.Filters;
using SituationCenterCore.Services.Implementations.RealTime;
using SituationCenterCore.Services.Interfaces.RealTime;
using SituationCenterCore.Services.HostedServices;

namespace SituationCenterCore
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
            AddDataBase(services);

            services.Configure<ServiceBusSettings>(Configuration.GetSection(nameof(ServiceBusSettings)));
            services.Configure<JwtOptions>(Configuration.GetSection(nameof(JwtOptions)));

            services.AddTransient<IRepository, EntityRepository>();
            services.AddTransient<IRoomManager, RoomsManager>();
            services.AddTransient<IRoomSecurityManager, RoomSecurityManager>();
            services.AddTransient<IFileServerNotifier, ServiceBusFileServerNotifier>();

            services.AddIdentity<ApplicationUser, Role>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = false;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var jwtOptions = Configuration
                .GetSection(nameof(JwtOptions))
                .Get<JwtOptions>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions.Issuer,

                            ValidateAudience = true,

                            ValidAudience = jwtOptions.Audience,

                            ValidateLifetime = true,
                            RequireExpirationTime = true,

                            IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey(),

                            ValidateIssuerSigningKey = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            services.AddMvc(options =>
            {

                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Bearer")
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add<RefreshTokenFilter>();
            });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<IWebSocketManager, WebSocketManager>();
            services.AddTransient<IWebSocketHandler, WebSocketHandler>();
            services.AddTransient<INotificator, WebSocketNotificator>();
            services.AddScoped<IRoleAccessor, RoleAccessor>();
            services.AddAutoMapper();
            services.AddCors();
            services.AddHostedService<RefreshTokenRemover>();
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseCors(policy => policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials()
            );
            app.UseExceptionsHandlerMiddleware();

            app.Use((context, func) =>
            {
                context.RequestServices.GetService<IRoleAccessor>().SetDbContext(context.RequestServices.GetService<ApplicationDbContext>());
                return func();
            });
            app.UseAuthentication();
            app.UseWebSockets();
            app.UseWebSocketMiddleware("/ws");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
        private void AddDataBase(IServiceCollection services)
        {
#if RELEASE
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ReleaseDataBase")));
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DataBase")));
            else
                services
                    .AddEntityFrameworkNpgsql()
                    .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("PosgresDataBase")));
#endif
        }
    }
}
