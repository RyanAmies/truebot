using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TRUEbot.Bot.Data;

namespace TRUEbot.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                using (var serviceScope = host.Services.CreateScope())
                {
                    var db = serviceScope.ServiceProvider.GetRequiredService<EntityContext>();
                    await db.Database.MigrateAsync();
                }

                await host.RunAsync();
            }
            catch (Exception e)
            {
                using var scope = host.Services.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogCritical(e, "Failed starting up application");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog((context, logger) =>
                        {
                            logger.WriteTo.File("logs\\log.txt", 
                                rollingInterval: RollingInterval.Day, 
                                restrictedToMinimumLevel: LogEventLevel.Information);
                        });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
