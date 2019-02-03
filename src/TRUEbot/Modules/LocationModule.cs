using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JetBrains.Annotations;
using Serilog;
using TRUEbot.Extensions;
using TRUEbot.Services;

namespace TRUEbot.Modules
{
    [Group("location"), Alias("sys","systems","system","l")]
    [UsedImplicitly]
    public class LocationModule : ModuleBase
    {
        private readonly IPlayerService _playerService;

        public LocationModule(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [Command,Summary("Gets all players in the location")]
        [UsedImplicitly]
        public async Task Get(string location)
        {
            try
            {
                var players = await _playerService.GetPlayersInLocation(location);

                if (!players.Any()) 
                {
                    await ReplyAsync("I couldn't find any players in this location");
                    return;
                }

                var locationEmbed = BuildEmbed(location, players);

                await ReplyAsync(embed: locationEmbed.Build());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed getting players in location {name}", location);
            }
        }

        [Command("rename"), Summary("Renames a location")]
        [UsedImplicitly]
        public async Task Rename(string originalName, string newName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalName) || string.IsNullOrWhiteSpace(newName))
                {
                    await ReplyAsync("Enter a name before renaming a location!");
                    return;
                }

                var response = await _playerService.TryUpdateLocationName(originalName, newName);

                if (response)
                {
                    await Context.AddConfirmation();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed renaming location {name} to new name: {newName}", originalName, newName);
            }
        }
       

        private static EmbedBuilder BuildEmbed(string location, List<PlayerDto> players)
        {
            var embed = new EmbedBuilder()
                .WithTitle(location);

            var output = string.Join(Environment.NewLine, players.OrderBy(a=>a.Name).Select(x => $"{x.Name} ({x.Alliance ?? "Unknown"})"));

            embed.AddField("Players", output);

            embed.WithFooter($"{players.Count} players").WithColor(new Color(95, 186, 125));

            return embed;
        }
    }
}
