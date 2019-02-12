using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Serilog;
using Serilog;
using TRUEbot.Extensions;
using TRUEbot.Services;

namespace TRUEbot.Modules
{
    [Group("alliance"), Alias("a")]
    [UsedImplicitly]
    public class AllianceModule : ModuleBase
    {
        private readonly IPlayerService _playerService;

        public AllianceModule(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [Command, Summary("Gets all players in the alliance")]
        [UsedImplicitly]
        public async Task Get(string allianceName)
        {
            try
            {
                var players = await _playerService.GetPlayersInAlliance(allianceName);

                if (!players.Any())
                {
                    await ReplyAsync("I couldn't find any players in this alliance");
                    return;
                }

                var allianceEmbed = BuildEmbed(allianceName, players);

                foreach (var embed in allianceEmbed)
                {
                    await ReplyAsync(embed: embed.Build());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed getting players in alliance {name}", allianceName);
            }
        }

        [Command("rename"), Summary("Renames an alliance")]
        [UsedImplicitly]
        public async Task Rename(string originalName, string newName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalName) || string.IsNullOrWhiteSpace(newName))
                {
                    await ReplyAsync("Enter a name before renaming an alliance!");
                    return;
                }

                var response = await _playerService.TryUpdateAllianceName(originalName, newName);

                switch (response)
                {
                    case UpdatePlayerResult.OK:
                        await Context.AddConfirmation();
                        break;
                    case UpdatePlayerResult.CantFindPlayer:
                        await ReplyAsync("There are no players with that alliance name");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed renaming alliance {name} to new name: {newName}", originalName, newName);
            }
        }

        private static List<EmbedBuilder> BuildEmbed(string allianceName, List<PlayerDto> players)
        {
            const int LIMIT = 1020;

            var pageText = "";
            var builders = new List<EmbedBuilder>();

            foreach (var x in players.OrderBy(a => a.Name))
            {
                var text = $"{x.Name} in ";
                if (x.Location == null)
                    text += "Unknown";
                else
                    text += $"{x.Location} ({x.LocationLevel}) - {x.LocationFaction}";

                if (pageText.Length + text.Length > LIMIT)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle($"{allianceName} Players Page ");

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
                .WithTitle($"{allianceName} Players Page ");

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
