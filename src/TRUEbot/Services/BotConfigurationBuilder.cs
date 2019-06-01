using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TRUEbot.Services
{
    public static class BotConfigurationBuilder
    {
        public static IConfigurationRoot Build()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var pathToAppSettings = Directory.GetCurrentDirectory();

            return new ConfigurationBuilder()
                .SetBasePath(pathToAppSettings)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static string GetBotDbConnectionString(this IConfigurationRoot configuration)
        {
            return configuration.GetConnectionString("TrueBot");
        }
    }
}
