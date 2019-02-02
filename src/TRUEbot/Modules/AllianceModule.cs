using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Serilog;
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

                if (!EnumerableExtensions.Any(players)) 
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

        private static EmbedBuilder BuildEmbed(string allianceName, List<PlayerDto> players)
        {
            var embed = new EmbedBuilder()
                .WithTitle(allianceName);

            var output = string.Join(", ", players.Select(x => x.Name));

            embed.AddField("Players", output);

            embed.WithFooter($"{players.Count} players").WithColor(new Color(95, 186, 125));

            return embed;
        }
    }
}
