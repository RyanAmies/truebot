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
        Task<List<LeaderboardDto>> GetStatsForKillCountLeaderboard(int leaderboardCount, DateTime fromDate, DateTime toDate);
        Task<List<LeaderboardDto>> GetStatsForPowderDestroyedLeaderboard(int leaderboardCount, DateTime fromDate, DateTime toDate);
        Task<KillDto> GetKillRecordById(int id);
        Task<DeleteKillResult> DeleteKillRecordById(int id);
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

        public Task<List<LeaderboardDto>> GetStatsForKillCountLeaderboard(int leaderboardCount, DateTime fromDate, DateTime toDate)
        {
            return _db.Kills
                .Where(a => a.KilledOn >= fromDate)
                .Where(a => a.KilledOn < toDate)
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
        public Task<List<LeaderboardDto>> GetStatsForPowderDestroyedLeaderboard(int leaderboardCount, DateTime fromDate, DateTime toDate)
        {
            return _db.Kills
                .Where(a => a.KilledOn >= fromDate)
                .Where(a => a.KilledOn < toDate)
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
