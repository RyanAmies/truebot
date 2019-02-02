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
    [UsedImplicitly]
    public class LocationModule : ModuleBase
    {
        private readonly IPlayerService _playerService;

        public LocationModule(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [Command("location"), Summary("Gets all players in the location")]
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

        [Command("spot"), Summary("Spots players and updates their location")]
        [UsedImplicitly]
        public async Task Get(string playerName, string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(location))
                    return;

                var response = await _playerService.TryUpdatePlayerLocation(playerName, location);

                if (response)
                {
                    await Context.AddConfirmation();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed spotting player {name} to in location: {location}", playerName, location);
            }
        }

        private static EmbedBuilder BuildEmbed(string location, List<PlayerDto> players)
        {
            var embed = new EmbedBuilder()
                .WithTitle(location);

            var output = string.Join(", ", players.Select(x => x.Name));

            embed.AddField("Players", output);

            embed.WithFooter($"{players.Count} players").WithColor(new Color(95, 186, 125));

            return embed;
        }
    }
}
