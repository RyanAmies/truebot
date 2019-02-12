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
    public interface IHitService
    {
        Task<bool> AddHitToPlayer(string playerName, string orderedBy, string reason);
        Task<bool> CompleteHitOnPlayer(string playerName, string completedBy);
        Task<List<HitDto>> GetOutstandingHits();
        
        Task<List<HitDto>> GetHitsCompletedByUserAsync(string username);
    }

    public class HitService : IHitService
    {
        private readonly EntityContext _db;

        public HitService(EntityContext db)
        {
            _db = db;
        }
        
        public async Task<bool> AddHitToPlayer(string playerName, string orderedBy, string reason)
        {
            var normalized = playerName.Normalise();

            var player = await _db.Players.Include(a=>a.System).FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (player == null)
                return false;

            var hit = await _db.Hits.Where(a=>a.CompletedOn == null).FirstOrDefaultAsync(x => x.PlayerId == player.Id);
            if (hit != null)
                return true;

            hit = new Hit();

            hit.PlayerId = player.Id;
            hit.OrderedBy = orderedBy;
            hit.OrderedOn = DateTime.Now;
            hit.Reason = reason;

            _db.Hits.Add(hit);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompleteHitOnPlayer(string playerName, string completedBy)
        {
            var normalized = playerName.Normalise();

            var hit = await _db.Hits.Where(a=>a.CompletedOn == null).FirstOrDefaultAsync(x => x.Player.NormalizedName == normalized);
            if (hit == null)
                return false;

            hit.CompletedOn = DateTime.Now;
            hit.CompletedBy = completedBy;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<HitDto>> GetOutstandingHits()
        {
            var hits = await _db.Hits.Where(x => x.CompletedOn == null).Select(x => new HitDto
            {
                Reason = x.Reason,
                Name = x.Player.Name,
                OrderedOn = x.OrderedOn,
                Alliance = x.Player.Alliance,
                Location = x.Player.Location,
                OrderedBy = x.OrderedBy
            }).ToListAsync();

            return hits;

        }


        public async Task<List<HitDto>> GetHitsCompletedByUserAsync(string username)
        {
            return await _db.Hits
                .Where(x => x.CompletedBy == username)
                .Select(x => new HitDto
                {
                    Reason = x.Reason,
                    Name = x.Player.Name,
                    OrderedOn = x.OrderedOn,
                    Alliance = x.Player.Alliance,
                    Location = x.Player.Location,
                    OrderedBy = x.OrderedBy,
                    CompletedOn = x.CompletedOn
                }).ToListAsync();
        }


     

    }

   
    public class HitDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Alliance { get; set; }
        public DateTime OrderedOn { get; set; }
        public string OrderedBy { get; set; }
        public string Reason { get; set; }
        public DateTime? CompletedOn { get; set; }
    }
}
