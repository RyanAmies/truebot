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
        Task<PlayerCreationResult> AddPlayer(string playerName, string alliance, string location, string addedByUsername);
        Task<PlayerDto> GetPlayerByName(string playerName);
        Task<List<PlayerDto>> GetPlayersInAlliance(string alliance);
        Task<List<PlayerDto>> GetPlayersInLocation(string location);
        Task<bool> TryUpdatePlayerName(string originalPlayerName, string newPlayerName);
        Task<bool> TryUpdateLocationName(string originalLocationName, string newLocation);
        Task<bool> TryUpdatePlayerLocation(string playerName, string location);
        Task<bool> TryUpdatePlayerAlliance(string playerName, string alliance);
        Task<bool> TryDeletePlayer(string playerName);

       Task<List<PlayerDto>> GetPlayersReportedByUserAsync(string playerName);
        Task<bool> TryUpdateAllianceName(string originalAllianceName, string newAllianceName);
        
        Task<List<PlayerDto>> GetAllPlayersAsync();
    }

    public class PlayerService : IPlayerService
    {
        private readonly EntityContext _db;

        public PlayerService(EntityContext db)
        {
            _db = db;
        }
        
        public async Task<PlayerCreationResult> AddPlayer(string playerName, string alliance, string location, string addedByUsername)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player != null)
                return PlayerCreationResult.Duplicate;

            player = new Player();

            player.AddedDate = DateTime.Now;
            player.AddedBy = addedByUsername;

            _db.Players.Add(player);

            UpdatePlayer(player, playerName, alliance, location);

            await _db.SaveChangesAsync();

            return PlayerCreationResult.OK;
        }

        public async Task<bool> TryUpdatePlayerName(string originalPlayerName, string newPlayerName)
        {
            var normalized = originalPlayerName.Normalise();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            UpdatePlayer(player, newPlayerName, player.Alliance, player.Location);

            await _db.SaveChangesAsync();

            return true;
        }
        
        public async Task<bool> TryUpdateLocationName(string originalLocationName, string newLocation)
        {
            var normalized = originalLocationName.Normalise();

            var players = await _db.Players.Where(x => x.NormalizedLocation == normalized|| x.Location == originalLocationName).ToListAsync();

            foreach (var player in players)
            {
                UpdatePlayer(player, player.Name, player.Alliance, newLocation);
            }

            await _db.SaveChangesAsync();

            return true;
        } 

        public async Task<bool> TryUpdateAllianceName(string originalAllianceName, string newAllianceName)
        {
            var normalized = originalAllianceName.Normalise();

            var players = await _db.Players.Where(x => x.NormalizedAlliance == normalized || x.Alliance == originalAllianceName).ToListAsync();

            foreach (var player in players)
            {
                UpdatePlayer(player, player.Name, newAllianceName, player.Location);
            }

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<PlayerDto>> GetAllPlayersAsync()
        {
            return await _db.Players
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    Location = x.Location,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).ToListAsync();
        }

        public async Task<bool> TryUpdatePlayerLocation(string playerName, string location)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            UpdatePlayer(player, player.Name, player.Alliance, location);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TryUpdatePlayerAlliance(string playerName, string alliance)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            UpdatePlayer(player, player.Name, alliance, player.Location);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TryDeletePlayer(string playerName)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            _db.Players.Remove(player);

            await _db.SaveChangesAsync();

            return true;
        }


        public async Task<List<PlayerDto>> GetPlayersReportedByUserAsync(string reportedBy)
        {
            return await _db.Players
                .Where(x => x.AddedBy == reportedBy)
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    Location = x.Location,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).ToListAsync();
        }

       

        private static void UpdatePlayer(Player player, string name, string alliance, string location)
        {
            name = name.UnifyApostrophe();

            player.Name = name;
            player.NormalizedName = name.Normalise();

            if (!string.IsNullOrWhiteSpace(alliance))
            {
                alliance = alliance.UnifyApostrophe();

                player.Alliance = alliance.ToUpperInvariant();
                player.NormalizedAlliance = alliance.Normalise();
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                location = location.UnifyApostrophe();


                player.Location = location;
                player.NormalizedLocation = location.Normalise();
            }

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
                    Location = x.Location,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).FirstOrDefaultAsync();
        }

        public async Task<List<PlayerDto>> GetPlayersInAlliance(string alliance)
        {
            var normalized = alliance.Normalise();

            return await _db.Players
                .Where(x => x.NormalizedAlliance.Contains(normalized))
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    Location = x.Location,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).ToListAsync();
        }

        public async Task<List<PlayerDto>> GetPlayersInLocation(string location)
        {
            var normalized = location.Normalise();

            return await _db.Players
                .Where(x => x.NormalizedLocation.Contains(normalized))
                .Select(x => new PlayerDto
                {
                    Name = x.Name,
                    Location = x.Location,
                    Alliance = x.Alliance,
                    AddedDate = x.AddedDate,
                    UpdatedDate = x.UpdatedDate,
                }).ToListAsync();
        }
    }

    public enum PlayerCreationResult
    {
        OK,
        Duplicate
    }

    public class PlayerDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Alliance { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

   
}
