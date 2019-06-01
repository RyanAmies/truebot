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
    [Group("kill"), Alias("kills", "killed")]
    public class KillModule : ModuleBase
    {
        private readonly IPlayerService _playerService;
        private readonly IKillService _killService;

        public KillModule(IPlayerService playerService, IKillService killService)
        {
            _playerService = playerService;
            _killService = killService;
        }

        [Command("log"), Summary("Marks a player as killed")]
        [UsedImplicitly]
        public async Task Get(string playerName, int power)
        {
            if (Context.Message.Attachments.Any() == false)
            {
                await ReplyAsync($"You must post a picture of the kill, or a link to the image");
                return;
            }

            var fn = Context.Message.Attachments.First().Url;
            await Get(playerName, power,fn);
        }

        [Command("log") ,Summary("Marks a player as killed")]
        [UsedImplicitly]
        public async Task Get(string playerName, int power, string imageLink)
        {
            try
            {
                var result = await _killService.AddKill(playerName, Context.User.Username, power, imageLink);

                if (result == KillLogResult.CannotFindPlayer)
                {
                    await ReplyAsync($"Cannot find player by that name. They must be added to the bot first with '!add {playerName}'");
                    return;
                }

                if (result == KillLogResult.OK)
                {
                    await Context.AddConfirmation();
                }

                var toDate = DateTime.Now;
                var fromDate = DateTime.Now.AddDays(-1);

                var killersStats = await _killService.GetStatsForKiller(Context.User.Username, fromDate, toDate);

                var killerEmbed = BuildKillerEmbed(Context.User.Username, killersStats, fromDate, toDate);
                await ReplyAsync(embed: killerEmbed.Build());


                var victimStats = await _killService.GetStatsForVictim(playerName, fromDate, toDate);
                var victimEmbed = BuildVictimEmbed(victimStats, fromDate, toDate);
                await ReplyAsync(embed: victimEmbed.Build());

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }

        [Command("victimstats"), Summary("Gets the stats for a victim")]
        [UsedImplicitly]
        public Task VictimStats(string playerName) => VictimStats(playerName, 1);

        [Command("victimstats"), Summary("Gets the stats for a victim")]
        [UsedImplicitly]
        public async Task VictimStats(string playerName, int days)
        {
            try
            {

                var toDate = DateTime.Now;
                var fromDate = DateTime.Now.AddDays(-days);

                var victimStats = await _killService.GetStatsForVictim(playerName, fromDate, toDate);

                if (victimStats.Any())
                {
                    var victimEmbed = BuildVictimEmbed(victimStats, fromDate, toDate);
                    await ReplyAsync(embed: victimEmbed.Build());
                }
                else
                {
                    await ReplyAsync($"No confirmed kills for {playerName}");

                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }

        
        [Command("killerstats"), Summary("Gets the stats for a victim")]
        [UsedImplicitly]
        public Task KillerStats() => KillerStats(Context.User.Username, 1);

        [Command("killerstats"), Summary("Gets the stats for a victim")]
        [UsedImplicitly]
        public Task KillerStats(string playerName) => KillerStats(playerName, 1);

        [Command("killerstats"), Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public async Task KillerStats(string playerName, int days)
        {
            try
            {

                var toDate = DateTime.Now;
                var fromDate = DateTime.Now.AddDays(-days);

                var killerStats = await _killService.GetStatsForKiller(playerName, fromDate, toDate);
                if (killerStats.Any())
                {
                    var killerEmbed = BuildKillerEmbed(playerName, killerStats, fromDate, toDate);
                    await ReplyAsync(embed: killerEmbed.Build());
                }
                else
                {
                    await ReplyAsync($"No confirmed kills for {playerName}");

                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }

        [Command("alliancestats"), Summary("Gets the stats for an alliance")]
        [UsedImplicitly]
        public Task AllianceStats(string allianceName) => AllianceStats(allianceName, 1);

        [Command("alliancestats"), Summary("Gets the stats for an alliance")]
        [UsedImplicitly]
        public async Task AllianceStats(string allianceName, int days)
        {
            try
            {

                var toDate = DateTime.Now;
                var fromDate = DateTime.Now.AddDays(-days);

                var victimStats = await _killService.GetStatsForVictimAlliance(allianceName, fromDate, toDate);

                if (victimStats.Any())
                {
                    var victimEmbed = BuildAllianceEmbed(victimStats, fromDate, toDate);
                    await ReplyAsync(embed: victimEmbed.Build());
                }
                else
                {
                    await ReplyAsync($"No confirmed kills for {allianceName.ToUpper()}");

                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }

        [Command("topkills"), Summary("Gets the stats for a victim")]
        [UsedImplicitly]
        public Task TopKills() => TopKills(1);

        [Command("topkills"), Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public async Task TopKills( int days)
        {
            try
            {

                var toDate = DateTime.Now;
                var fromDate = DateTime.Now.AddDays(-days);

                var killerStats = await _killService.GetStatsForKillCountLeaderboard(10, fromDate, toDate);
                if (killerStats.Any())
                {
                    var killerEmbed = BuildKillCountLeaderBoardEmbed(killerStats, days,10);
                    await ReplyAsync(embed: killerEmbed.Build());
                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }

        [Command("toppower"), Summary("Gets the stats for a victim")]
        [UsedImplicitly]
        public Task TopPower() => TopPower(1);

        [Command("toppower"), Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public async Task TopPower(int days)
        {
            try
            {

                var toDate = DateTime.Now;
                var fromDate = DateTime.Now.AddDays(-days);

                var killerStats = await _killService.GetStatsForPowderDestroyedLeaderboard(10, fromDate, toDate);
                if (killerStats.Any())
                {
                    var killerEmbed = BuildPowerDestroyedLeaderBoardEmbed(killerStats, days, 10);
                    await ReplyAsync(embed: killerEmbed.Build());
                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }


        [Command("show"), Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public async Task Show( int id)
        {
            try
            {
                var killerStats = await _killService.GetKillRecordById(id);
                if (killerStats != null)
                {
                    var killerEmbed = BuildIndividualKillEmbed(killerStats);
                    await ReplyAsync(embed: killerEmbed.Build());
                }
                else
                {
                    await ReplyAsync($"Could not find kill #{id}");

                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }

        [Command("delete"), Summary("Deletes a kill record")]
        [UsedImplicitly]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Delete( int id)
        {
            try
            {
                var deleteResult = await _killService.DeleteKillRecordById(id);
                if (deleteResult == DeleteKillResult.Ok)
                {
                    await Context.AddConfirmation();
                }
                else
                {
                    await ReplyAsync($"Could not find kill #{id}");

                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed deleting kill");
            }
        }

     
        private static EmbedBuilder BuildKillCountLeaderBoardEmbed(List<LeaderboardDto> kills, int days, int leaderboardCount)
        {
            var embed = new EmbedBuilder();

            int index = 1;
            var output = string.Join(Environment.NewLine, kills.OrderByDescending(a => a.TotalKills).Select(x => $"{index++}. {x.Player} with {x.TotalKills} kills"));

            embed.AddField($"Top {leaderboardCount} Killers by Kill Count for the last {days} days", output);

            
            return embed;
        }
        private static EmbedBuilder BuildPowerDestroyedLeaderBoardEmbed( List<LeaderboardDto> kills, int days, int leaderboardCount)
        {
            var embed = new EmbedBuilder();


            int index = 1;
            var output = string.Join(Environment.NewLine, kills.OrderByDescending(a => a.TotalPower).Select(x => $"{index++}. {x.Player} with {x.TotalPower.Value.ToString("N0")} ({(x.TotalPower.Value / x.TotalKills).ToString("N0")} average)"));

            embed.AddField($"Top {leaderboardCount} Killers by Power Destroyed for the last {days} days", output);

            
            return embed;
        }
       

        private static EmbedBuilder BuildKillerEmbed(string player, List<KillDto> kills, DateTime from, DateTime to)
        {
            var victimName = kills.First().Victim;
            var victimAlliance = kills.First().Alliance;
            var embed = new EmbedBuilder();

            var output = string.Join(Environment.NewLine, kills.OrderByDescending(a => a.KilledOn).Select(x => $"#{x.Id} [{x.Alliance}] {x.Victim} on {x.KilledOn} ({ x.Power.ToString("N0")}) [Img]({x.ImageLink})"));

            embed.AddField($"Killer Stats for {player} since {from.ToString("dd/MM hh:mm")}", output);

            embed.WithFooter($"{kills.Count} confirmed kills. {kills.Select(a => a.Power).DefaultIfEmpty(0).Sum().ToString("N0")} power destroyed.").WithColor(new Color(95, 186, 125));

            return embed;
        }

        private static EmbedBuilder BuildVictimEmbed(List<KillDto> kills, DateTime from, DateTime to)
        {
            var victimName = kills.First().Victim;
            var victimAlliance = kills.First().Alliance;
            var embed = new EmbedBuilder();

            var output = string.Join(Environment.NewLine, kills.OrderByDescending(a => a.KilledOn).Select(x => $"#{x.Id} [{x.Alliance}] {x.Victim} Killed By {x.KilledBy} on {x.KilledOn.ToString("dd/MM hh:mm")} ({ x.Power.ToString("N0")}) [Img]({x.ImageLink})"));

            embed.AddField($"Victim Stats for [{victimAlliance}] {victimName} since {from.ToString("dd/MM hh:mm")}", output);

            embed.WithFooter($"{kills.Count} confirmed kills. {kills.Select(a => a.Power).DefaultIfEmpty(0).Sum().ToString("N0")} power destroyed.").WithColor(new Color(95, 186, 125));

            return embed;
        }

        private static EmbedBuilder BuildAllianceEmbed(List<KillDto> kills, DateTime from, DateTime to)
        {
            var victimAlliance = kills.First().Alliance;
            var embed = new EmbedBuilder();

            var output = string.Join(Environment.NewLine, kills.OrderByDescending(a => a.KilledOn).Select(x => $"#{x.Id} [{x.Alliance}] {x.Victim} Killed By {x.KilledBy} on {x.KilledOn.ToString("dd/MM hh:mm")} ({ x.Power.ToString("N0")}) [Img]({x.ImageLink})"));

            embed.AddField($"Victim Stats for [{victimAlliance}] since {from.ToString("dd/MM hh:mm")}", output);

            embed.WithFooter($"{kills.Count} confirmed kills. {kills.Select(a => a.Power).DefaultIfEmpty(0).Sum().ToString("N0")} power destroyed.").WithColor(new Color(95, 186, 125));

            return embed;
        }

        private static EmbedBuilder BuildIndividualKillEmbed(KillDto killerStats)
        {
            var embed = new EmbedBuilder();

            var output = $"#{killerStats.Id} [{killerStats.Alliance}] {killerStats.Victim} Killed By {killerStats.KilledBy} on {killerStats.KilledOn.ToString("dd/MM hh:mm")} ({ killerStats.Power.ToString("N0")}) [Img]({killerStats.ImageLink})";

            embed.AddField($"Kill #{killerStats.Id}", output);

            

            return embed;
        }

    }
}
