using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SituationCenterBackServer.Data;
using SituationCenterBackServer.Models;
using SituationCenterBackServer.Services;
using Microsoft.IdentityModel.Tokens;
using SituationCenterBackServer.Models.TokenAuthModels;
using SituationCenterBackServer.Models.VoiceChatModels;
using SituationCenterBackServer.Logging;
using SituationCenterBackServer.Models.VoiceChatModels.Connectors;
using SituationCenterBackServer.Models.StorageModels;
using SituationCenterBackServer.Models.Options;
using System.Text;
using Common.Services;
using SituationCenterBackServer.Models.RoomSecurity;

namespace SituationCenterBackServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DataBaseConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();


            services.Configure<UnrealAPIConfiguration>(Configuration.GetSection("UnrealAPI"));
            services.Configure<DocumentsHandlerConfiguration>(Configuration.GetSection("DocumentsHandler"));
            services.Configure<AuthOptions>(Configuration.GetSection("TokenAuthentication"));


            services.AddSingleton<IRoomManager, RoomsManager>();
            services.AddTransient<IRoomSecurityManager, RoomSecurityManager>();
            //services.AddSingleton<IConnector, UdpConnector>();
            //services.AddSingleton<IStableConnector, TCPConnector>();
            services.AddSingleton<IDocumentHandlerService, DocumentsHandler>();
            services.AddSingleton<IBuffer, ASPNETBufferService>();

            //Storage
            services.AddSingleton<IStorageManager, InProjectSavingStorageManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //loggerFactory.AddDebug(LogLevel.Trace);
            loggerFactory.AddProvider(new SocketLoggerProvider());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }



            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value));
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                    ValidateAudience = true,
                    ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.RegisterAsService(ServiceTypes.Core, Configuration.GetConnectionString("IPResolverHost"));
            InitiUsers(app.ApplicationServices);
        }

        private void InitiUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if(roleManager.FindByNameAsync("Administrator").Result == null)
                roleManager.CreateAsync(new IdentityRole("Administrator")).Wait();
            IdentityResult identity = null;
            ApplicationUser administrator = new ApplicationUser()
            {
                UserName = "maksalmak@gmail.com",
                Email = "maksalmak@gmail.com"
            };
            if (userManager.FindByEmailAsync("maksalmak@gmail.com").Result == null)
            {
                identity = userManager.CreateAsync(administrator, "CaPOnidolov2_").Result;
                userManager.AddToRoleAsync(administrator, "Administrator").Wait();
            }
        }
    }
}
