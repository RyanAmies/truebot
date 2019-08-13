using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TRUEbot.Data;
using TRUEbot.Data.Models;
using TRUEbot.Extensions;

namespace TRUEbot.Services
{
    public interface IKillService
    {

        Task<KillLogResult> AddKill(string playerName, string userUsername, int power, string imageLink);
        Task<List<KillDto>> GetStatsForKiller(string userUsername, DateTime fromDate, DateTime toDate);
        Task<List<KillDto>> GetStatsForVictim(string playerName, DateTime fromDate, DateTime toDate);
        Task<List<KillDto>> GetStatsForVictimAlliance(string allianceName, DateTime fromDate, DateTime toDate);
        Task<List<LeaderboardDto>> GetStatsForKillCountLeaderboard(int leaderboardCount, DateTime fromDate,
            DateTime toDate, string alliance);
        Task<List<LeaderboardDto>> GetStatsForPowderDestroyedLeaderboard(int leaderboardCount, DateTime fromDate,
            DateTime toDate, string alliance);
        Task<KillDto> GetKillRecordById(int id);
        Task<DeleteKillResult> DeleteKillRecordById(int id);
        Task<SummaryStatsDto> GetSummaryStatsForKiller(string userUsername, string playerName);
    }

    public class KillService : IKillService
    {
        private readonly EntityContext _db;

        public KillService(EntityContext db)
        {
            _db = db;
        }

        public async Task<KillLogResult> AddKill(string playerName, string userUsername, int power, string imageLink)
        {
            var normalizedPlayerName = playerName.Normalise();
            var normalizedUsername = userUsername.Normalise();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalizedPlayerName);

            if (player == null)
                return KillLogResult.CannotFindPlayer;

            _db.Kills.Add(new Kill
            {
                KilledBy = userUsername,
                KilledByNormalised = normalizedUsername,
                Player = player,
                KilledOn = DateTime.Now,
                Power = power,
                ImageLink = imageLink
            });

            await _db.SaveChangesAsync();

            return KillLogResult.OK;
        }

        public Task<List<KillDto>> GetStatsForKiller(string userUsername, DateTime fromDate, DateTime toDate)
        {
            var normalizedUsername = userUsername.Normalise();

            return _db.Kills
                .Where(a => a.KilledByNormalised == normalizedUsername)
                .Where(a => a.KilledOn >= fromDate)
                .Where(a => a.KilledOn < toDate)
                .Select(a => new KillDto
                {
                    Id = a.Id,
                    Victim = a.Player.Name,
                    Alliance = a.Player.Alliance,
                    KilledOn = a.KilledOn,
                    KilledBy = a.KilledBy,
                    Power = a.Power,
                    ImageLink = a.ImageLink
                }).ToListAsync();
        }

        public Task<List<KillDto>> GetStatsForVictim(string playerName, DateTime fromDate, DateTime toDate)
        {
            var normalised = playerName.Normalise();

            return _db.Kills
                .Where(a => a.Player.NormalizedName == normalised)
                .Where(a => a.KilledOn >= fromDate)
                .Where(a => a.KilledOn < toDate)
                .Select(a => new KillDto
                {
                    Id = a.Id,
                    Victim = a.Player.Name,
                    Alliance = a.Player.Alliance,
                    KilledOn = a.KilledOn,
                    KilledBy = a.KilledBy,
                    Power = a.Power,
                    ImageLink = a.ImageLink
                }).ToListAsync();
        }

        public Task<List<KillDto>> GetStatsForVictimAlliance(string allianceName, DateTime fromDate, DateTime toDate)
        {
            var normalised = allianceName.Normalise();

            return _db.Kills
                .Where(a => a.Player.NormalizedAlliance == normalised)
                .Where(a => a.KilledOn >= fromDate)
                .Where(a => a.KilledOn < toDate)
                .Select(a => new KillDto
                {
                    Id = a.Id,
                    Victim = a.Player.Name,
                    Alliance = a.Player.Alliance,
                    KilledOn = a.KilledOn,
                    KilledBy = a.KilledBy,
                    Power = a.Power,
                    ImageLink = a.ImageLink
                }).ToListAsync();
        }

        public Task<List<LeaderboardDto>> GetStatsForKillCountLeaderboard(int leaderboardCount, DateTime fromDate,
            DateTime toDate, string alliance)
        {
            var normalised = alliance.Normalise();

            return _db.Kills
                .Where(a => a.KilledOn >= fromDate)
                .Where(a => a.KilledOn < toDate)
                .Where(a => alliance == null || a.Player.NormalizedAlliance == normalised)
                .GroupBy(a => new { a.KilledByNormalised, a.KilledBy })

                .Select(a => new LeaderboardDto
                {
                    Player = a.Key.KilledBy,
                    TotalPower = a.Select(q => q.Power).DefaultIfEmpty().Sum(),
                    TotalKills = a.Count(),
                }).OrderByDescending(a => a.TotalKills)
                .Take(leaderboardCount)
                .ToListAsync();
        }
        public Task<List<LeaderboardDto>> GetStatsForPowderDestroyedLeaderboard(int leaderboardCount, DateTime fromDate,
            DateTime toDate, string alliance)
        {
            var normalised = alliance.Normalise();

            return _db.Kills
                .Where(a => a.KilledOn >= fromDate)
                .Where(a => a.KilledOn < toDate)
                .Where(a => alliance == null || a.Player.NormalizedAlliance == normalised)
                .GroupBy(a => new { a.KilledByNormalised, a.KilledBy })
                .Select(a => new LeaderboardDto
                {
                    Player = a.Key.KilledBy,
                    TotalPower = a.Select(q => q.Power).DefaultIfEmpty().Sum(),
                    TotalKills = a.Count(),
                }).OrderByDescending(a => a.TotalPower)
                .Take(leaderboardCount)
                .ToListAsync();
        }

        public Task<KillDto> GetKillRecordById(int id)
        {
            return _db.Kills
                .Where(a => a.Id == id)
                .Select(a => new KillDto
                {
                    Id = a.Id,
                    Victim = a.Player.Name,
                    Alliance = a.Player.Alliance,
                    KilledOn = a.KilledOn,
                    KilledBy = a.KilledBy,
                    Power = a.Power,
                    ImageLink = a.ImageLink
                }).SingleOrDefaultAsync();
        }

        public async Task<DeleteKillResult> DeleteKillRecordById(int id)
        {
            var kill = await _db.Kills
                .Where(a => a.Id == id)
                .SingleOrDefaultAsync();

            if (kill == null)
                return DeleteKillResult.Failed;

            _db.Kills.Remove(kill);

            return DeleteKillResult.Ok;

        }

        public async Task<SummaryStatsDto> GetSummaryStatsForKiller(string userUsername, string playerName)
        {
            var normalisedUser = userUsername.Normalise();
            var normalisedPlayer = playerName.Normalise();

            var yesterday = DateTime.Now.AddDays(-1);

            var dto =await  _db.Kills
                .Where(a=>a.KilledByNormalised == normalisedUser)
                .GroupBy(a=>a.KilledBy)
                .Select(a => new SummaryStatsDto
                {
                    TotalKills24Hours = a.Where(r=>r.KilledOn>=yesterday).Count(),
                    TotalKillsAllTime = a.Count(),
                    
                    TotalPower24Hours = a.Where(r=>r.KilledOn>=yesterday).Select(t=>t.Power).DefaultIfEmpty().Sum(),
                    TotalPowerAllTime = a.Select(r=>r.Power).DefaultIfEmpty().Sum(),
                    
                }).SingleAsync();

            var player =await  _db.Players.Where(a => a.NormalizedName == normalisedPlayer).SingleAsync();
            dto.VictimKills24Hours = await _db.Kills
                .Where(a => a.Player == player)
                .Where(a => a.KilledOn >= yesterday)
                .CountAsync();
            dto.VictimPower24Hours = await _db.Kills
                .Where(a => a.Player == player)
                .Where(a => a.KilledOn >= yesterday)
                .Select(a=>a.Power).SumAsync();

            dto.VictimKillsAllTime = await _db.Kills
                .Where(a => a.Player == player)
                .CountAsync();
            dto.VictimPowerAllTime = await _db.Kills
                .Where(a => a.Player == player)
                .Select(a=>a.Power).SumAsync();

            dto.VictimAllianceKills24Hours = await _db.Kills
                .Where(a => a.Player.NormalizedAlliance == player.NormalizedAlliance)
                .Where(a => a.KilledOn >= yesterday)
                .CountAsync();
            dto.VictimAlliancePower24Hours = await _db.Kills
                .Where(a => a.Player.NormalizedAlliance == player.NormalizedAlliance)
                .Where(a => a.KilledOn >= yesterday)
                .Select(a=>a.Power).SumAsync();

            dto.VictimAllianceKillsAllTime = await _db.Kills
                .Where(a => a.Player.NormalizedAlliance == player.NormalizedAlliance)
                .CountAsync();
            dto.VictimAlliancePowerAllTime = await _db.Kills
                .Where(a => a.Player.NormalizedAlliance == player.NormalizedAlliance)
                .Select(a=>a.Power).SumAsync();

            dto.VictimAllianceName = player.Alliance;

            dto.LastKillId = await _db.Kills.Where(a => a.KilledByNormalised == normalisedUser)
                .OrderByDescending(a => a.KilledOn).Select(a => a.Id).FirstAsync();
            return dto;
        }
    }

    public class SummaryStatsDto
    {
        public int TotalKillsAllTime { get; set; }
        public int TotalKills24Hours { get; set; }
        public int TotalPowerAllTime { get; set; }
        public int TotalPower24Hours { get; set; }

        public int VictimKills24Hours { get; set; }
        public int VictimKillsAllTime { get; set; }
        public int VictimPower24Hours { get; set; }
        public int VictimPowerAllTime { get; set; }

        public int VictimAllianceKills24Hours { get; set; }
        public int VictimAlliancePower24Hours { get; set; }
        public int VictimAllianceKillsAllTime { get; set; }
        public int VictimAlliancePowerAllTime { get; set; }

        public string VictimAllianceName { get; set; }
        public int LastKillId { get; set; }
    }

    public enum DeleteKillResult
    {
        Ok,
        Failed
    }

    public class LeaderboardDto
    {
        public string Player { get; set; }
        public int TotalPower { get; set; }
        public int TotalKills { get; set; }
    }

    public enum KillLogResult
    {
        OK,
        CannotFindPlayer
    }
    public class KillDto
    {
        public int Id { get; set; }
        public string Alliance { get; set; }
        public string Victim { get; set; }
        public DateTime KilledOn { get; set; }
        public string KilledBy { get; set; }
        public int Power { get; set; }
        public string ImageLink { get; set; }
    }
}
