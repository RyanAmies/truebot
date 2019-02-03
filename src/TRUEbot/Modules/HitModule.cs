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
    [Group("hit"), Alias("hits")]
    public class HitModule : ModuleBase
    {
        private readonly IPlayerService _playerService;

        public HitModule(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [Command("add"), Summary("Marks a player as needing to be hit")]
        [UsedImplicitly]
        public Task Add(string name) => Get(name, null);


        [Command("add"), Summary("Marks a player as needing to be hit")]
        [UsedImplicitly]
        public async Task Get(string playerName, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    await ReplyAsync("Enter a name before hitting a player!");
                    return;
                }

                var player = await _playerService.GetPlayerByName(playerName);

                if (player == null)
                {
                    await ReplyAsync("Can't find that player!");
                    return;
                }

                var success = await _playerService.AddHitToPlayer(playerName, Context.User.Username, reason);

                if (success)
                    await Context.AddConfirmation();
                else
                    await ReplyAsync("Couldn't order hit on player");


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed ordering hit on player {name}", playerName);
            }
        }
        [Command(), Alias("who", "current"), Summary("Marks a player as needing to be hit")]
        [UsedImplicitly]
        public async Task Get()
        {
            try
            {


                var hits = await _playerService.GetOutstandingHits();

                if (hits.Any() == false)
                {
                    await ReplyAsync("Not hits outstanding!");
                    return;
                }

                var emblem = BuildEmbed(hits);
                await ReplyAsync(embed: emblem.Build());

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed getting outstanding hits");
            }
        }

        [Command("complete"), Summary("Marks a hit as completed")]
        [UsedImplicitly]
        public async Task Complete(string playerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    await ReplyAsync("Enter a name before marking a player as hit!");
                    return;
                }

                var player = await _playerService.GetPlayerByName(playerName);

                if (player == null)
                {
                    await ReplyAsync("Can't find that player!");
                    return;
                }

                var success = await _playerService.CompleteHitOnPlayer(playerName, Context.User.Username);

                if (success)
                    await Context.AddConfirmation();
              


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed completing a hit on player {name}", playerName);
            }
        }

        [Command("stats"), Summary("Gets the number of hits a user has completed")]
        [UsedImplicitly]
        public Task Stats() => Stats(Context.User.Username);

        [Command("stats"), Summary("Gets the number of hits a user has completed")]
        [UsedImplicitly]
        public async Task Stats(string username)
        {
            try
            {
                var hits = await _playerService.GetHitsCompletedByUserAsync(username);

                if (!hits.Any()) 
                {
                    await ReplyAsync($"{username} hasn't completed any hits yet");
                    return;
                }

                var locationEmbed = BuildHitsEmbed(username, hits);

                await ReplyAsync(embed: locationEmbed.Build());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed getting reported players for user {username}", username);
            }
        }


        private static EmbedBuilder BuildEmbed(List<HitDto> players)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Outstanding Hits");

            var output = string.Join(Environment.NewLine, players.OrderBy(a => a.Name).Select(x => $"{x.Name} [{x.Alliance ?? "Unknown"}] ({x.Location ?? "Unknown Location"}) Ordered By {x.OrderedBy} For Reason {x.Reason ?? "Unknown"}"));

            embed.AddField("Players", output);

            embed.WithFooter($"{players.Count} players").WithColor(new Color(95, 186, 125));

            return embed;
        }  
        
        private static EmbedBuilder BuildHitsEmbed(string username, List<HitDto> players)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"Completed Hits For {username}");

            var output = string.Join(Environment.NewLine, players.OrderByDescending(a => a.CompletedOn).Select(x => $"{x.Name} [{x.Alliance ?? "Unknown"}] Ordered By {x.OrderedBy} Completed on {x.CompletedOn.Value.ToString("dd/MM/yy")}"));

            embed.AddField("Players", output);

            embed.WithFooter($"{players.Count} hits").WithColor(new Color(95, 186, 125));

            return embed;
        }
    }
}
