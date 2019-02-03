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
    [Group("alliance")]
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

                await ReplyAsync(embed: allianceEmbed.Build());
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

                if (response)
                {
                    await Context.AddConfirmation();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed renaming alliance {name} to new name: {newName}", originalName, newName);
            }
        }

        private static EmbedBuilder BuildEmbed(string allianceName, List<PlayerDto> players)
        {
            var embed = new EmbedBuilder()
                .WithTitle(allianceName);

            var output = string.Join(Environment.NewLine, players.OrderBy(a=>a.Name).Select(x => $"{x.Name} ({x.Location ?? "Unknown"})"));

            embed.AddField("Players", output);

            embed.WithFooter($"{players.Count} players").WithColor(new Color(95, 186, 125));

            return embed;
        }
    }
}
