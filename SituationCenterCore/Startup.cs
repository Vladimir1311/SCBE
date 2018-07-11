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
using SituationCenterCore.Middleware;
using SituationCenterCore.Models.Settings;
using SituationCenterCore.Services.Interfaces;
using SituationCenterCore.Services.Implementations;

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

            services.AddTransient<IRepository, EntityRepository>();
            services.AddTransient<IRoomManager, RoomsManager>();
            services.AddTransient<IRoomSecurityManager, RoomSecurityManager>();
            services.AddTransient<IFileServerNotifier, ServiceBusFileServerNotifier>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = false;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = MockAuthOptions.ISSUER,

                            ValidateAudience = true,

                            ValidAudience = MockAuthOptions.AUDIENCE,

                            ValidateLifetime = true,

                            IssuerSigningKey = MockAuthOptions.GetSymmetricSecurityKey(),

                            ValidateIssuerSigningKey = true,
                        };
                    });

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddCors();
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
            app.UseStaticFiles();

            app.UseAuthentication();

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DataBase")));
#endif
        }
    }
}
