using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TRUEbot.Data;
using TRUEbot.Data.Models;

namespace TRUEbot.Services
{
    public interface IPlayerService
    {
        Task AddPlayer(string playerName, string alliance, string location);
        Task<PlayerDto> GetPlayerByName(string playerName);
        Task<List<PlayerDto>> GetPlayersInAlliance(string alliance);
        Task<List<PlayerDto>> GetPlayersInLocation(string location);
        Task<bool> TryUpdatePlayerName(string originalPlayerName, string newPlayerName);
        Task<bool> TryUpdatePlayerLocation(string playerName, string location);
        Task<bool> TryUpdatePlayerAlliance(string playerName, string alliance);
        Task<bool> TryDeletePlayer(string playerName);
    }

    public class PlayerService : IPlayerService
    {
        private readonly EntityContext _db;

        public PlayerService(EntityContext db)
        {
            _db = db;
        }

        public async Task AddPlayer(string playerName, string alliance, string location)
        {
            var player = new Player();

            player.AddedDate = DateTime.Now;

            _db.Players.Add(player);

            UpdatePlayer(player, playerName, alliance, location);

            await _db.SaveChangesAsync();
        }

        public async Task<bool> TryUpdatePlayerName(string originalPlayerName, string newPlayerName)
        {
            var normalized = originalPlayerName.ToUpper();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            UpdatePlayer(player, newPlayerName, player.Alliance, player.Location);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TryUpdatePlayerLocation(string playerName, string location)
        {
            var normalized = playerName.ToUpper();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            UpdatePlayer(player, player.Name, player.Alliance, location);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TryUpdatePlayerAlliance(string playerName, string alliance)
        {
            var normalized = playerName.ToUpper();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            UpdatePlayer(player, player.Name, alliance, player.Location);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TryDeletePlayer(string playerName)
        {
            var normalized = playerName.ToUpper();

            var player = await _db.Players.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            _db.Players.Remove(player);

            await _db.SaveChangesAsync();

            return true;
        }

        private static void UpdatePlayer(Player player, string name, string alliance, string location)
        {
            player.Name = name;
            player.NormalizedName = name.ToUpper();

            if (!string.IsNullOrWhiteSpace(alliance))
            {
                player.Alliance = alliance.ToUpper();
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                player.Location = location;
                player.NormalizedLocation = location.ToUpper();
            }

            player.UpdatedDate = DateTime.Now;
        }

        public async Task<PlayerDto> GetPlayerByName(string playerName)
        {
            var normalized = playerName.ToUpper();

            return await _db.Players
                .Where(x => x.NormalizedName == normalized)
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
            var normalized = alliance.ToUpper();

            return await _db.Players
                .Where(x => x.Alliance == normalized)
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
            var normalized = location.ToUpper();

            return await _db.Players
                .Where(x => x.NormalizedLocation == normalized)
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

    public class PlayerDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Alliance { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
