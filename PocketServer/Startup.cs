using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using PocketServer.Extensions;
using PocketServer.DataAccess;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using PocketServer.DataAccess.Core;
using PocketServer.DataAccess.Services;
using PocketServer.WebSockets;

namespace PocketServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add the DatabaseContext
            services.AddDbContext<DatabaseContext>(
                options => options.UseMySql(
                    Configuration.GetConnectionString("AlertDatabase"),
                    mySqlOptions => mySqlOptions.ServerVersion(new Version(15, 1, 0), ServerType.MariaDb))
                .UseLazyLoadingProxies());

            // Add the generic repository
            services.AddScoped(typeof(IRepository<>), typeof(PocketRepository<>));

            services.AddSingleton<WebSocketHandler>();

            // Add the services
            services.AddScoped<UserService>();
            services.AddScoped<DeviceService>();
            services.AddScoped<HeartbeatService>();
            services.AddScoped<AlertService>();

            // Add controllers
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("AllowAll");
            app.AddWebSockets();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
