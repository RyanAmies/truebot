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
    public interface IPlayerService
    {
        Task<PlayerCreationResult> AddPlayer(string playerName, string alliance, string location,
            string addedByUsername, int? level);
        Task<PlayerDto> GetPlayerByName(string playerName);
        Task<List<PlayerDto>> GetPlayersInAlliance(string alliance);
        Task<List<PlayerDto>> GetPlayersInLocation(string location);
        Task<UpdatePlayerResult> TryUpdatePlayerName(string originalPlayerName, string newPlayerName);
        Task<UpdatePlayerResult> TryUpdatePlayerLocation(string playerName, string location);
        Task<UpdatePlayerResult> TryUpdatePlayerAlliance(string playerName, string alliance);
        Task<UpdatePlayerResult> TryDeletePlayer(string playerName);

        Task<List<PlayerDto>> GetPlayersReportedByUserAsync(string playerName);
        Task<UpdatePlayerResult> TryUpdateAllianceName(string originalAllianceName, string newAllianceName);

        Task<List<PlayerDto>> GetAllPlayersAsync();
        Task<bool> NormaliseSystemNames();
        Task<UpdatePlayerResult> TryUpdatePlayerLevel(string playerName, int level);
    }

    public class PlayerService : IPlayerService
    {
        private readonly EntityContext _db;

        public PlayerService(EntityContext db)
        {
            _db = db;
        }

        public async Task<PlayerCreationResult> AddPlayer(string playerName, string alliance, string location,string addedByUsername, int? level)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.Include(a => a.System).FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player != null)
                return PlayerCreationResult.Duplicate;

            player = new Player();

            player.AddedDate = DateTime.Now;
            player.AddedBy = addedByUsername;

            _db.Players.Add(player);

            Data.Models.System system = null;

            if (string.IsNullOrWhiteSpace(location) == false)
            {
                var systemName = location.Normalise();

                system = await _db.Systems.FirstOrDefaultAsync(a => a.NormalizedName == systemName);
                if (system == null)
                    return PlayerCreationResult.CantFindSystem;
            }

            UpdatePlayer(player, playerName, alliance, system,level);

            await _db.SaveChangesAsync();

            return PlayerCreationResult.OK;
        }

        public async Task<UpdatePlayerResult> TryUpdatePlayerName(string originalPlayerName, string newPlayerName)
        {
            var normalized = originalPlayerName.Normalise();

            var player = await _db.Players.Include(a => a.System).FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return UpdatePlayerResult.CantFindPlayer;

            UpdatePlayer(player, newPlayerName, player.Alliance, player.System, player.Level);

            await _db.SaveChangesAsync();

            return UpdatePlayerResult.OK;
        }



        public async Task<UpdatePlayerResult> TryUpdateAllianceName(string originalAllianceName, string newAllianceName)
        {
            var normalized = originalAllianceName.Normalise();

            var players = await _db.Players.Include(a => a.System).Where(x => x.NormalizedAlliance == normalized || x.Alliance == originalAllianceName).ToListAsync();

            if (players.Any() == false)
                return UpdatePlayerResult.CantFindPlayer;

            foreach (var player in players)
            {
                UpdatePlayer(player, player.Name, newAllianceName, player.System, player.Level);
            }

            await _db.SaveChangesAsync();

            return UpdatePlayerResult.OK;
        }

        public async Task<List<PlayerDto>> GetAllPlayersAsync()
        {
            return await _db.Players
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    Location = x.SystemId != null ? x.System.Name : null,
                    LocationFaction = x.SystemId != null ? x.System.Faction : null,
                    LocationLevel = x.SystemId != null ? x.System.Level : (int?)null,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).ToListAsync();
        }

        public async Task<bool> NormaliseSystemNames()
        {
            var systems = await _db.Systems.ToListAsync();
            foreach (var system in systems)
            {
                system.NormalizedName = system.Name.Normalise();
            }

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<UpdatePlayerResult> TryUpdatePlayerLevel(string playerName, int level)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.Include(a => a.System).FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return UpdatePlayerResult.CantFindPlayer;

            UpdatePlayer(player, player.Name, player.Alliance, player.System, level);

            await _db.SaveChangesAsync();

            return UpdatePlayerResult.OK;
        }

        public async Task<UpdatePlayerResult> TryUpdatePlayerLocation(string playerName, string location)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.Include(a => a.System).FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return UpdatePlayerResult.CantFindPlayer;

            Data.Models.System system = null;

            if (location != null)
            {
                var normalisedSystemName = location.Normalise();

                system = await _db.Systems.SingleOrDefaultAsync(a => a.NormalizedName.StartsWith(normalisedSystemName));
                if (system == null)
                    return UpdatePlayerResult.CantFindSystem;
            }

            UpdatePlayer(player, player.Name, player.Alliance, system, player.Level);

            await _db.SaveChangesAsync();

            return UpdatePlayerResult.OK;
        }

        public async Task<UpdatePlayerResult> TryUpdatePlayerAlliance(string playerName, string alliance)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.Include(a => a.System).FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return UpdatePlayerResult.CantFindPlayer;

            UpdatePlayer(player, player.Name, alliance, player.System, player.Level);

            await _db.SaveChangesAsync();

            return UpdatePlayerResult.OK;
        }

        public async Task<UpdatePlayerResult> TryDeletePlayer(string playerName)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.Include(a => a.System).FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return UpdatePlayerResult.CantFindPlayer;


            _db.Players.Remove(player);

            await _db.SaveChangesAsync();

            return UpdatePlayerResult.OK;
        }


        public async Task<List<PlayerDto>> GetPlayersReportedByUserAsync(string reportedBy)
        {
            return await _db.Players
                .Where(x => x.AddedBy == reportedBy)
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    Location = x.SystemId != null ? x.System.Name : null,
                    LocationFaction = x.SystemId != null ? x.System.Faction : null,
                    LocationLevel = x.SystemId != null ? x.System.Level : (int?)null,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).ToListAsync();
        }



        private static void UpdatePlayer(Player player, string name, string alliance, Data.Models.System system, int? level)
        {
            name = name.UnifyApostrophe();

            player.Name = name;
            player.NormalizedName = name.Normalise();
            player.Level = level;

            if (!string.IsNullOrWhiteSpace(alliance))
            {
                alliance = alliance.UnifyApostrophe();

                player.Alliance = alliance.ToUpperInvariant();
                player.NormalizedAlliance = alliance.Normalise();
            }

                player.SystemLogs.Add(new SystemLog
                {
                    Player = player,
                    System = system,
                    DateUpdated = DateTime.Now
                });
          

            player.System = system;

            player.UpdatedDate = DateTime.Now;
        }

        public async Task<PlayerDto> GetPlayerByName(string playerName)
        {
            var normalized = playerName.UnifyApostrophe().ToUpper();

            return await _db.Players
                .Where(x => x.NormalizedName.Contains(normalized))
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    PlayerLevel = x.Level,
                    Location = x.SystemId != null ? x.System.Name : null,
                    LocationFaction = x.SystemId != null ? x.System.Faction : null,
                    LocationLevel = x.SystemId != null ? x.System.Level : (int?)null,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                    SystemLogs = x.SystemLogs.Select(s=>new SystemLogDto
                    {
                        DateUpdated = s.DateUpdated,
                        SystemName = s.SystemId != null ? s.System.Name : null,
                        SystemFaction = s.SystemId != null ? s.System.Faction : null,
                        SystemLevel = s.SystemId != null ? s.System.Level : (int?) null,
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<List<PlayerDto>> GetPlayersInAlliance(string alliance)
        {
            var normalized = alliance.Normalise();

            return await _db.Players
                .Where(x => x.NormalizedAlliance==normalized)
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    PlayerLevel = x.Level,
                    Location = x.SystemId != null ? x.System.Name : null,
                    LocationFaction = x.SystemId != null ? x.System.Faction : null,
                    LocationLevel = x.SystemId != null ? x.System.Level : (int?)null,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).ToListAsync();
        }

        public async Task<List<PlayerDto>> GetPlayersInLocation(string location)
        {
            if (location == null)
            {

                return await _db.Players
                    .Where(x => x.System == null)
                    .Select(x => new PlayerDto
                    {
                        Name = x.Name,
                        PlayerLevel = x.Level,
                        Location = x.SystemId != null ? x.System.Name : null,
                        LocationFaction = x.SystemId != null ? x.System.Faction : null,
                        LocationLevel = x.SystemId != null ? x.System.Level : (int?)null,
                        Alliance = x.Alliance,
                        AddedDate = x.AddedDate,
                        UpdatedDate = x.UpdatedDate,
                    }).ToListAsync();
            }

            var normalized = location.Normalise();

            return await _db.Players
                .Where(x => x.System.NormalizedName.Contains(normalized))
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    PlayerLevel = x.Level,
                    Location = x.SystemId != null ? x.System.Name : null,
                    LocationFaction = x.SystemId != null ? x.System.Faction : null,
                    LocationLevel = x.SystemId != null ? x.System.Level : (int?)null,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).ToListAsync();
        }
    }

    public class SystemLogDto
    {
        public DateTime DateUpdated { get; set; }
        public string SystemFaction { get; set; }
        public string SystemName { get; set; }
        public int? SystemLevel { get; set; }
    }

    public enum PlayerCreationResult
    {
        OK,
        Duplicate,
        CantFindSystem
    }
    public enum UpdatePlayerResult
    {
        OK,
        CantFindPlayer,
        CantFindAlliance,
        CantFindSystem
    }
    public class PlayerDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string LocationFaction { get; set; }
        public int? LocationLevel { get; set; }
        public int? PlayerLevel { get; set; }
        public string Alliance { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<SystemLogDto> SystemLogs { get; set; } = new List<SystemLogDto>();
    }


}
