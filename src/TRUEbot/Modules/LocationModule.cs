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
    [Group("location"), Alias("sys", "systems", "system", "l")]
    [UsedImplicitly]
    public class LocationModule : ModuleBase
    {
        private readonly IPlayerService _playerService;

        public LocationModule(IPlayerService playerService)
        {
            _playerService = playerService;
        }

       
        [Command, Summary("Gets all players in the location")]
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

                var locationEmbed = BuildEmbed(players);

                foreach (var embed in locationEmbed)
                {
                    await ReplyAsync(embed: embed.Build());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed getting players in location {name}", location);
            }
        }






        private static List<EmbedBuilder> BuildEmbed(List<PlayerDto> players)
        {
            const int LIMIT = 1020;

            var pageText = "";
            var builders = new List<EmbedBuilder>();
            var locationName = players.First().Location;
            var locationFaction = players.First().LocationFaction;
            var locationLevel = players.First().LocationLevel;


            foreach (var player in players.OrderBy(a => a.Alliance).ThenBy(a=>a.Name))
            {
                var text = (player.Alliance == null ? "" : $"[{player.Alliance}] ") + player.Name;

                if (pageText.Length + text.Length > LIMIT)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle($"Players in {locationName} ({locationLevel}) - {locationFaction} Page ");

                    embed.AddField("Players", pageText);


                    embed.WithFooter($"{players.Count} players").WithColor(new Color(95, 186, 125));

                    builders.Add(embed);

                    pageText = "";
                }
                else
                {
                    pageText += Environment.NewLine + text;
                }

            }

            var finalEmbed = new EmbedBuilder()
                .WithTitle($"Players in {locationName} ({locationLevel}) - {locationFaction} Page ");

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
