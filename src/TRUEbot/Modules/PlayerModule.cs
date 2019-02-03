using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JetBrains.Annotations;
using Serilog;
using TRUEbot.Extensions;
using TRUEbot.Services;

namespace TRUEbot.Modules
{
    [Group("player"), Alias("p")]
    [UsedImplicitly]
    public class PlayerModule : ModuleBase
    {
        private readonly IPlayerService _playerService;

        public PlayerModule(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [Command, Summary("Gets a player by their name")]
        [UsedImplicitly]
        public async Task Get(string name)
        {
            try
            {
                var player = await _playerService.GetPlayerByName(name);

                if (player == null)
                {
                    await ReplyAsync("I couldn't find a player by that name");
                    return;
                }

                var playerEmbed = BuildEmbed(player);

                await ReplyAsync(embed: playerEmbed.Build());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed getting player by name {name}", name);
            }
        }

        [Command("add"), Summary("Adds a player with name")]
        [UsedImplicitly]
        public Task Add(string name) => Add(name, null, null);

        [Command("add"), Summary("Adds a player with name, alliance")]
        [UsedImplicitly]
        public Task Add(string name, string alliance) => Add(name, alliance, null);

        [Command("add"), Summary("Adds a player with name, alliance and location")]
        [UsedImplicitly]
        public async Task Add(string name, string alliance, string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    await ReplyAsync("Enter a name before adding a player!");
                    return;
                }

                var result = await _playerService.AddPlayer(name, alliance, location, Context.User.Username);

                if (result == PlayerCreationResult.OK)
                    await Context.AddConfirmation();
                else
                    await ReplyAsync("Player already exists please use `assign` to change alliance or `spot` to change location");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed getting player by name {name}", name);
            }
        }

        [Command("rename"), Summary("Renames a player")]
        [UsedImplicitly]
        public async Task Rename(string originalName, string newName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalName) || string.IsNullOrWhiteSpace(newName))
                {
                    await ReplyAsync("Enter a name before renaming a player!");
                    return;
                }

                var response = await _playerService.TryUpdatePlayerName(originalName, newName);

                if (response)
                {
                    await Context.AddConfirmation();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed renaming player {name} to new name: {newName}", originalName, newName);
            }
        }

        [Command("assign"), Summary("Assigns a player to an alliance")]
        [UsedImplicitly]
        public async Task Assign(string playerName, string alliance)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(alliance))
                    return;

                var response = await _playerService.TryUpdatePlayerAlliance(playerName, alliance);

                if (response)
                {
                    await Context.AddConfirmation();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed assigning player {name} to new alliance: {alliance}", playerName, alliance);
            }
        }

        [Command("spot"), Alias("location"), Summary("Updates a player's location")]
        [UsedImplicitly]
        public async Task Spot(string playerName, string location)
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

        [Command("delete"), Alias("remove", "del", "rem"), Summary("Removes a player from being tracked")]
        [UsedImplicitly]
        public async Task Delete(string playerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName))
                    return;

                var response = await _playerService.TryDeletePlayer(playerName);

                if (response)
                {
                    await Context.AddConfirmation();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed deleting player {name}", playerName);
            }
        }

        private static EmbedBuilder BuildEmbed(PlayerDto player)
        {
            var embed = new EmbedBuilder()
                .WithAuthor(x => x.WithName(player.Name));

            embed.AddField("Alliance", player.Alliance ?? "Unknown");

            embed.AddField("Location", player.Location ?? "Unknown");

            embed.WithFooter(GetPostedMeta(player)).WithColor(new Color(95, 186, 125));

            return embed;
        }

        private static string GetPostedMeta(PlayerDto player) => $"Tracking since {player.AddedDate:dd/MM/yy HH:mm}, last updated: {player.UpdatedDate:dd/MM/yy HH:mm}";
    }
}
