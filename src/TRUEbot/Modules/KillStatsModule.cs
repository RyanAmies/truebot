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
    [Group("killstats")]
    public class KillStatsModule : ModuleBase
    {
        private readonly IKillService _killService;

        public KillStatsModule(IKillService killService)
        {
            _killService = killService;
        }

      
        [Command("victims"), Alias("victimstats"),Summary("Gets the stats for a victim")]
        [UsedImplicitly]
        public Task VictimStats(string playerName) => VictimStats(playerName, 1);

        [Command("victims"), Alias("victimstats"),Summary("Gets the stats for a victim")]
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
                    var victimEmbed = BuildVictimEmbed(victimStats, fromDate);
                    foreach (var embed in victimEmbed)
                    {
                        await ReplyAsync(embed: embed.Build());
                    }

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

        
        [Command("killer"), Alias("killerstats"),Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public Task KillerStats() => KillerStats(Context.User.Username, 1);

        [Command("killer"),Alias("killerstats"),Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public Task KillerStats(string playerName) => KillerStats(playerName, 1);

        [Command("killer"),Alias("killerstats"), Summary("Gets the stats for a killer")]
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
                    foreach (var embed in killerEmbed)
                    {
                        await ReplyAsync(embed: embed.Build());
                    }
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

        [Command("alliance"),Alias("alliancestats"), Summary("Gets the stats for an alliance")]
        [UsedImplicitly]
        public Task AllianceStats(string allianceName) => AllianceStats(allianceName, 1);

        [Command("alliance"),Alias("alliancestats"), Summary("Gets the stats for an alliance")]
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
                   var allianceEmbed = BuildAllianceEmbed(victimStats, fromDate, toDate);
                   foreach (var embed in allianceEmbed)
                   {
                       await ReplyAsync(embed: embed.Build());
                   }

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

        [Command("kills"), Alias("topkiller"), Summary("Gets the the top killers")]
        [UsedImplicitly]
        public Task TopKills() => TopKills(1);

        
        [Command("kills"), Alias("topkiller"), Summary("Gets the the top killers")]
        [UsedImplicitly]
        public Task TopKills(int days) => TopKills(days, null);

        [Command("kills"), Alias("topkiller"), Summary("Gets the stats for a killer"), Priority(-1)]
        [UsedImplicitly]
        public async Task TopKills( int days, string alliance)
        {
            try
            {

                var toDate = DateTime.Now;
                var fromDate = DateTime.Now.AddDays(-days);

                var killerStats = await _killService.GetStatsForKillCountLeaderboard(10, fromDate, toDate, alliance);
                if (killerStats.Any())
                {
                    var killerEmbed = BuildKillCountLeaderBoardEmbed(killerStats, days,10, alliance);
                    foreach (var embedBuilder in killerEmbed)
                    {
                        await ReplyAsync(embed: embedBuilder.Build());
                    }
                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }

        [Command("power"),Alias("toppower"),Summary("Gets the leaders for power destroyed in the last day")]
        [UsedImplicitly]
        public Task TopPower() => TopPower(1,null);

        [Command("power"),Alias("toppower"), Summary("Gets the leaders for power destroyed in the last day")]
        [UsedImplicitly]
        public Task TopPower(int days) => TopPower(1, null);

        [Command("power"),Alias("toppower"), Summary("Gets the leaders for power destroyed in the last day")]
        [UsedImplicitly]
        public async Task TopPower(int days, string alliance)
        {
            try
            {

                var toDate = DateTime.Now;
                var fromDate = DateTime.Now.AddDays(-days);

                var killerStats = await _killService.GetStatsForPowderDestroyedLeaderboard(10, fromDate, toDate,alliance);
                if (killerStats.Any())
                {
                    var killerEmbed = BuildPowerDestroyedLeaderBoardEmbed(killerStats, days, 10, alliance);
                    foreach (var embedBuilder in killerEmbed)
                    {
                        await ReplyAsync(embed: embedBuilder.Build());
                    }
                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed logging kill");
            }
        }


        [Command("show"), Summary("Shows a specific kill record")]
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

     
      
        private static List<EmbedBuilder> BuildKillCountLeaderBoardEmbed(List<LeaderboardDto> kills, int days,
            int leaderboardCount, string alliance)
        {
            var title = $"Top {leaderboardCount} Killers by Kill Count for the last {days} days";
            if (alliance != null)
                title += " VS " + alliance;

            var footer = $"{kills.Sum(a=>a.TotalKills)} kills. {kills.Sum(a=>a.TotalPower).ToString("N0")} power destroyed.";

            int index = 1;

            var dataRows = kills.OrderByDescending(a => a.TotalKills).Select(x => $"{index++}. {x.Player} with {x.TotalKills.ToString("N0")} kills ({(x.TotalPower / x.TotalKills).ToString("N0")} average)").ToList();
            
            return EmbedPaginator.Paginate(dataRows,title,"Top Killers",footer);
        }

        

        private static List<EmbedBuilder> BuildPowerDestroyedLeaderBoardEmbed(List<LeaderboardDto> kills, int days,
            int leaderboardCount, string alliance)
        {
            var title = $"Top {leaderboardCount} Killers by Power Destroyed for the last {days} days";
            if (alliance != null)
                title += " VS " + alliance;

            var footer = $"{kills.Sum(a=>a.TotalKills)} kills. {kills.Sum(a=>a.TotalPower).ToString("N0")} power destroyed.";

            int index = 1;

            var dataRows = kills.OrderByDescending(a => a.TotalPower).Select(x => $"{index++}. {x.Player} with {x.TotalPower.ToString("N0")} power ({(x.TotalPower / x.TotalKills).ToString("N0")} average)").ToList();
            
            return EmbedPaginator.Paginate(dataRows,title,"Top Killers",footer);
        }
       

        private static EmbedBuilder BuildSummaryStatsEmbed(string killer, string victim,
            SummaryStatsDto summaryStats)
        {
            var embed = new EmbedBuilder().WithTitle($"Confirming Kill Number #{summaryStats.LastKillId} by {killer}").WithColor(new Color(95, 186, 125));

            embed.AddField($"{killer} Last 24 Hours", $"Kill Count: {summaryStats.TotalKills24Hours} {Environment.NewLine}Power Destroyed: {summaryStats.TotalPower24Hours.ToString("N0")} {Environment.NewLine}Average: {(summaryStats.TotalPower24Hours / summaryStats.TotalKills24Hours).ToString("N0")}");
            embed.AddField($"{killer} Global", $"Kill Count: {summaryStats.TotalKillsAllTime} {Environment.NewLine}Power Destroyed: {summaryStats.TotalPowerAllTime.ToString("N0")} {Environment.NewLine}Average: {(summaryStats.TotalPowerAllTime / summaryStats.TotalKillsAllTime).ToString("N0")}");
            
            embed.AddField($"{victim} Last 24 Hours", $"Death Count: {summaryStats.VictimKills24Hours} {Environment.NewLine}Power Destroyed: {summaryStats.VictimPower24Hours.ToString("N0")} {Environment.NewLine}");
            embed.AddField($"{victim} Global", $"Death Count: {summaryStats.VictimKillsAllTime} {Environment.NewLine}Power Destroyed: {summaryStats.VictimPowerAllTime.ToString("N0")} {Environment.NewLine}");

            embed.AddField($"{summaryStats.VictimAllianceName} Last 24 Hours", $"Death Count: {summaryStats.VictimAllianceKills24Hours} {Environment.NewLine}Power Destroyed: {summaryStats.VictimAlliancePower24Hours.ToString("N0")} {Environment.NewLine}");
            embed.AddField($"{summaryStats.VictimAllianceName} Global", $"Death Count: {summaryStats.VictimAllianceKillsAllTime} {Environment.NewLine}Power Destroyed: {summaryStats.VictimAlliancePowerAllTime.ToString("N0")} {Environment.NewLine}");

            return embed;
        }


        private static List<EmbedBuilder> BuildKillerEmbed(string player, List<KillDto> kills, DateTime from, DateTime to)
        {
            var title =$"Killer Stats for {player} since {from.ToString("dd/MM HH:mm")}";
            var footer = $"{kills.Count} kills. {kills.Sum(a=>a.Power).ToString("N0")} power destroyed.";
            
            var dataRows = kills.OrderByDescending(a => a.KilledOn).Select(x => $"#{x.Id} [{x.Alliance}] {x.Victim} on {x.KilledOn.ToString("dd/MM HH:mm")} ({ x.Power.ToString("N0")}) [Img]({x.ImageLink})").ToList();
            
            return EmbedPaginator.Paginate(dataRows,title,"Confirmed Kills",footer);
        }
    
       
        private static List<EmbedBuilder> BuildVictimEmbed(List<KillDto> kills, DateTime from)
        {
            var victimName = kills.First().Victim;
            var victimAlliance = kills.First().Alliance;

            var title = $"Victim Stats for [{victimAlliance}] {victimName} since {from.ToString("dd/MM HH:mm")}";
            var footer = $"{kills.Count} kills. {kills.Sum(a=>a.Power).ToString("N0")} power destroyed.";
            
            var dataRows = kills.OrderByDescending(a => a.KilledOn).Select(x => $"#{x.Id} [{x.Alliance}] {x.Victim} Killed By {x.KilledBy} on {x.KilledOn.ToString("dd/MM HH:mm")} ({x.Power.ToString("N0")}) [Img]({x.ImageLink})").ToList();
            
            return EmbedPaginator.Paginate(dataRows,title,"Confirmed Kills",footer);
        }

        private static List<EmbedBuilder> BuildAllianceEmbed(List<KillDto> kills, DateTime from, DateTime to)
        {
            var victimAlliance = kills.First().Alliance;

            var title = $"Victim Stats for [{victimAlliance}] since {from.ToString("dd/MM HH:mm")}";
            var footer = $"{kills.Count} kills. {kills.Sum(a=>a.Power).ToString("N0")} power destroyed.";
            
            var dataRows = kills.OrderByDescending(a => a.KilledOn).Select(x => $"#{x.Id} [{x.Alliance}] {x.Victim} Killed By {x.KilledBy} on {x.KilledOn.ToString("dd/MM HH:mm")} ({x.Power.ToString("N0")}) [Img]({x.ImageLink})").ToList();
            
            return EmbedPaginator.Paginate(dataRows,title,"Confirmed Kills",footer);
        }



        private static EmbedBuilder BuildIndividualKillEmbed(KillDto killerStats)
        {
            var embed = new EmbedBuilder();

            var output = $"#{killerStats.Id} [{killerStats.Alliance}] {killerStats.Victim} Killed By {killerStats.KilledBy} on {killerStats.KilledOn.ToString("dd/MM HH:mm")} ({ killerStats.Power.ToString("N0")}) [Img]({killerStats.ImageLink})";

            embed.AddField($"Kill #{killerStats.Id}", output);
            
            return embed;
        }

    }
}
