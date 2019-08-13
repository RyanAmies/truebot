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
    [Group("kill")]
    public class KillModule : ModuleBase
    {
        private readonly IKillService _killService;

        public KillModule(IKillService killService)
        {
            _killService = killService;
        }

        [Command("log"), Alias("add",""), Summary("Marks a player as killed")]
        [UsedImplicitly]
        public async Task Get(string playerName, int power)
        {
            if (Context.Message.Attachments.Any() == false)
            {
                await Context.AddRejection();

                await ReplyAsync($"You must post a picture of the kill, or a link to the image");
                return;
            }

            var fn = Context.Message.Attachments.First().Url;
            await Get(playerName, power,fn);
        }

        [Command("log"), Alias("add","") ,Summary("Marks a player as killed")]
        [UsedImplicitly]
        public async Task Get(string playerName, int power, string imageLink)
        {
            try
            {

                var result = await _killService.AddKill(playerName, Context.User.Username, power, imageLink);

                if (result == KillLogResult.CannotFindPlayer)
                {
                    await Context.AddRejection();
                    await ReplyAsync($"Cannot find player by that name. They must be added to the bot first with '!p add {playerName}'");

                    return;
                }

                if (result == KillLogResult.OK)
                {
                    await Context.AddConfirmation();
                }

                var summaryStats = await _killService.GetSummaryStatsForKiller(Context.User.Username, playerName);
                
                var summaryEmbed = BuildSummaryStatsEmbed(Context.User.Username, playerName, summaryStats);

                    await ReplyAsync(embed: summaryEmbed.Build());
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
            await Context.AddRejection();

            await ReplyAsync($"Use !killstats instead");
        }

        
        [Command("killerstats"), Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public Task KillerStats() => KillerStats(Context.User.Username, 1);

        [Command("killerstats"), Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public Task KillerStats(string playerName) => KillerStats(playerName, 1);

        [Command("killerstats"), Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public async Task KillerStats(string playerName, int days)
        {
            await Context.AddRejection();

            await ReplyAsync($"Use !killstats instead");
        }

        [Command("alliancestats"), Summary("Gets the stats for an alliance")]
        [UsedImplicitly]
        public Task AllianceStats(string allianceName) => AllianceStats(allianceName, 1);

        [Command("alliancestats"), Summary("Gets the stats for an alliance")]
        [UsedImplicitly]
        public async Task AllianceStats(string allianceName, int days)
        {
            await Context.AddRejection();

            await ReplyAsync($"Use !killstats instead");
        }

        [Command("topkills"), Alias("topkiller"), Summary("Gets the the top killers")]
        [UsedImplicitly]
        public Task TopKills() => TopKills(1);

        
        [Command("topkills"), Alias("topkiller"), Summary("Gets the the top killers")]
        [UsedImplicitly]
        public Task TopKills(int days) => TopKills(days, null);

        [Command("topkills"), Alias("topkiller"), Summary("Gets the stats for a killer")]
        [UsedImplicitly]
        public async Task TopKills( int days, string alliance)
        {
            await Context.AddRejection();

            await ReplyAsync($"Use !killstats instead");
        }

        [Command("toppower"), Summary("Gets the leaders for power destroyed in the last day")]
        [UsedImplicitly]
        public Task TopPower() => TopPower(1,null);

        [Command("toppower"), Summary("Gets the leaders for power destroyed in the last day")]
        [UsedImplicitly]
        public Task TopPower(int days) => TopPower(1, null);

        [Command("toppower"), Summary("Gets the leaders for power destroyed in the last day")]
        [UsedImplicitly]
        public async Task TopPower(int days, string alliance)
        {
            await Context.AddRejection();

            await ReplyAsync($"Use !killstats instead");
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
        
        private static EmbedBuilder BuildIndividualKillEmbed(KillDto killerStats)
        {
            var embed = new EmbedBuilder();

            var output = $"#{killerStats.Id} [{killerStats.Alliance}] {killerStats.Victim} Killed By {killerStats.KilledBy} on {killerStats.KilledOn.ToString("dd/MM HH:mm")} ({ killerStats.Power.ToString("N0")}) [Img]({killerStats.ImageLink})";

            embed.AddField($"Kill #{killerStats.Id}", output);
            
            return embed;
        }

    }
}
