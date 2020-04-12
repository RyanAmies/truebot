using System.IO;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TRUEbot.Bot.Services;

namespace TRUEbot.Bot
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBotClient(this IServiceCollection collection)
        {
            var pathToAppSettings = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(pathToAppSettings)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Production.json", true)
                .Build();

            return collection
                .AddOptions()
                .Configure<BotConfiguration>(configuration)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<InteractiveService>()
                .AddSingleton<CommandService>()
                .AddScoped<IHitService, HitService>()
                .AddScoped<IKillService, KillService>()
                .AddScoped<IPlayerService, PlayerService>()
                .AddScoped<ILocationService, LocationService>()
                .AddSingleton<IBotClient, BotClient>();
        }
    }
}
