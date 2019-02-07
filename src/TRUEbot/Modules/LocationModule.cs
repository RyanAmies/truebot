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
       
        private static List<EmbedBuilder> BuildEmbed(string location, List<PlayerDto> players)
        {
            const int LIMIT = 1020;

            var pageText = "";
            var builders = new List<EmbedBuilder>();

            foreach (var playerText in players.OrderBy(a => a.Name).Select(x => $"{x.Name} [{x.Alliance ?? "Unknown"}]"))
            {
                if (pageText.Length + playerText.Length > LIMIT)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle($"Players in {location} Page ");
         
                    embed.AddField("Players", pageText);

                    embed.WithFooter($"{players.Count} players").WithColor(new Color(95, 186, 125));

                    builders.Add(embed);

                    pageText = "";
                }
                else
                {
                    pageText += Environment.NewLine + playerText;
                }
            }

            var finalEmbed = new EmbedBuilder()
                .WithTitle($"Players in {location} Page ");
         
            finalEmbed.AddField("Players", pageText);

            finalEmbed.WithFooter($"{players.Count} players").WithColor(new Color(95, 186, 125));
            
            builders.Add(finalEmbed);

            var page = 1;
            var pages = builders.Count;

            foreach (var embedBuilder in builders)
            {
                embedBuilder.Title += $"{page} of {pages}";
            }
         
            return builders;
        }
    }
}
