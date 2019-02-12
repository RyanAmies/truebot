using System;
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
        public Task Add(string name) => Add(name, null, null,null);

        [Command("add"), Summary("Adds a player with name, alliance")]
        [UsedImplicitly]
        public Task Add(string name, string alliance) => Add(name, alliance, null,null);

        [Command("add"), Summary("Adds a player with name, alliance and location")]
        [UsedImplicitly]
        public async Task Add(string name, string alliance, string location, int? level)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    await ReplyAsync("Enter a name before adding a player!");
                    return;
                }

                var result = await _playerService.AddPlayer(name, alliance, location, Context.User.Username, level);

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

                switch (response)
                {
                    case UpdatePlayerResult.OK:
                        await Context.AddConfirmation();
                        break;
                    case UpdatePlayerResult.CantFindPlayer:
                        await ReplyAsync("Unable to find a player with that name");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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

                switch (response)
                {
                    case UpdatePlayerResult.OK:
                        await Context.AddConfirmation();
                        break;
                    case UpdatePlayerResult.CantFindPlayer:
                        await ReplyAsync("Unable to find a player with that name");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed assigning player {name} to new alliance: {alliance}", playerName, alliance);
            }
        }

        [Command("level"), Summary("Assigns a player to a level")]
        [UsedImplicitly]
        public async Task Assign(string playerName, int level)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    await ReplyAsync("Enter a player name");
                    return;
                }

                var response = await _playerService.TryUpdatePlayerLevel(playerName, level);

                switch (response)
                {
                    case UpdatePlayerResult.OK:
                        await Context.AddConfirmation();
                        break;
                    case UpdatePlayerResult.CantFindPlayer:
                        await ReplyAsync("Unable to find a player with that name");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed setting level for player {name}", playerName);
            }
        }

        [Command("spot"), Alias("location"), Summary("Updates a player's location")]
        [UsedImplicitly]
        public async Task Spot(string playerName, string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    await ReplyAsync("Invalid player name. Try !spot \"player\" \"system\"");

                    return;
                }

                if (string.IsNullOrWhiteSpace(location))
                {
                    await ReplyAsync("Invalid location name. Try !spot \"player\" \"system\"");

                    return;
                }


                var response = await _playerService.TryUpdatePlayerLocation(playerName, location);

                switch (response)
                {
                    case UpdatePlayerResult.OK:
                        await Context.AddConfirmation();
                        break;
                    case UpdatePlayerResult.CantFindPlayer:
                        await ReplyAsync("Unable to find a player with that name");
                        break;
                    case UpdatePlayerResult.CantFindSystem:
                        await ReplyAsync("Unable to find a system with that name");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed spotting player {name} to in location: {location}", playerName, location);
            }
        }

        [Command("missing"), Summary("Clears a players location")]
        [UsedImplicitly]
        public async Task Missing(string playerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    await ReplyAsync("Invalid player name. Try !spot \"player\" \"system\"");

                    return;
                }
                

                var response = await _playerService.TryUpdatePlayerLocation(playerName, null);

                switch (response)
                {
                    case UpdatePlayerResult.OK:
                        await Context.AddConfirmation();
                        break;
                    case UpdatePlayerResult.CantFindPlayer:
                        await ReplyAsync("Unable to find a player with that name");
                        break;
                    case UpdatePlayerResult.CantFindSystem:
                        await ReplyAsync("Unable to find a system with that name");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed spotting player {name} to in location: {location}", playerName);
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

                switch (response)
                {
                    case UpdatePlayerResult.OK:
                        await Context.AddConfirmation();
                        break;
                    case UpdatePlayerResult.CantFindPlayer:
                        await ReplyAsync("Unable to find a player with that name");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

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

            embed.AddField("Location", player.Location == null ?"Unknown": $"{player.Location} ({player.LocationLevel}) - {player.LocationFaction}" );

            embed.AddField("Level", player.PlayerLevel != null ?player.PlayerLevel.Value.ToString(): "Unknown");

            if (player.SystemLogs.Any())
            {
                var locationLogs = player.SystemLogs.OrderByDescending(a=>a.DateUpdated).Select(s =>
                        $"{s.SystemName} ({s.SystemLevel}) - {s.SystemFaction} on {s.DateUpdated:dd/MM/yy HH:mm}"+Environment.NewLine).ToList();
                embed.AddField("Previous Locations", string.Join("",locationLogs));

            }

            embed.WithFooter(GetPostedMeta(player)).WithColor(new Color(95, 186, 125));

            return embed;
        }

        private static string GetPostedMeta(PlayerDto player) => $"Tracking since {player.AddedDate:dd/MM/yy HH:mm}, last updated: {player.UpdatedDate:dd/MM/yy HH:mm}";
    }
}
