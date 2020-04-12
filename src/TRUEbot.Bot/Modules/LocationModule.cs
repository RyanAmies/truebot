using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JetBrains.Annotations;
using Serilog;
using TRUEbot.Bot.Extensions;
using TRUEbot.Bot.Services;

namespace TRUEbot.Bot.Modules
{
    [Name("Location"),Group("location"), Alias("sys", "systems", "system", "l")]
    [UsedImplicitly]
    public class LocationModule : ModuleBase
    {
        private readonly ILocationService _locationService;
        private readonly IPlayerService _playerService;

        public LocationModule(ILocationService locationService, IPlayerService playerService)
        {
            _locationService = locationService;
            _playerService = playerService;
        }
        [Command, Summary("Gets all players in the location")]
        [UsedImplicitly]
        public async Task Get(string location)
        {
            try
            {
                if (location.ToLowerInvariant() == "missing")
                    location = null;

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

        [Command("add"), Summary("Add a system. Admin only feature")]
        [UsedImplicitly]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Add(string locationName, string faction, int level)
        {
            try
            {
                var deleteResult = await _locationService.AddLocation(locationName, faction, level);
                switch (deleteResult)
                {
                    case LocationCreationResult.OK:
                        await Context.AddConfirmation();

                        break;
                    case LocationCreationResult.Duplicate:
                        await Context.AddRejection();
                        await ReplyAsync($"Duplicate system");
                        break;
                    case LocationCreationResult.CantFindSystem:
                        await Context.AddRejection();
                        await ReplyAsync($"Could not find system");

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed adding system");
            }
        }


        [Command("rename"), Summary("Renames a location. Admin only feature")]
        [UsedImplicitly]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Rename(string locationName, string newLocationName)
        {
            try
            {
                var deleteResult = await _locationService.RenameLocation(locationName, newLocationName);
                if (deleteResult == LocationCreationResult.OK)
                {
                    await Context.AddConfirmation();
                }
                else
                {
                    await ReplyAsync($"Could not find system");

                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed renaming system");
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
            
            var title = $"Players in {locationName} ({locationLevel}) - {locationFaction} Page ";
            if (locationName == null)
                title = $"Players with no location Page ";

            foreach (var player in players.OrderBy(a => a.Alliance).ThenBy(a=>a.Name))
            {
                var text = (player.Alliance == null ? "" : $"[{player.Alliance}] ") + player.Name;

                if (player.PlayerLevel != null)
                    text += $"({player.PlayerLevel})";
                

                if (pageText.Length + text.Length > LIMIT)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle(title);

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
