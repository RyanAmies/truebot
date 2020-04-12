using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TRUEbot.Bot.Data;
using TRUEbot.Bot.Extensions;

namespace TRUEbot.Bot.Services
{
    public interface ILocationService
    {
        Task<LocationCreationResult> AddLocation(string locationName, string faction, int level);
        Task<LocationCreationResult> RenameLocation(string locationName, string newLocationName);
    }

    public class LocationService : ILocationService
    {
        private readonly EntityContext _db;

        public LocationService(EntityContext db)
        {
            _db = db;
        }

        public async Task<LocationCreationResult> AddLocation(string locationName, string faction, int level)
        {
            var normalized = locationName.Normalise();

            var system = await _db.Systems.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (system != null)
                return LocationCreationResult.Duplicate;

            system = new Data.Models.System
            {
                Faction = faction,
                Level = level,
                Name = locationName,
                NormalizedName = normalized
            };
            
            _db.Systems.Add(system);

            await _db.SaveChangesAsync();

            return LocationCreationResult.OK;
        }

        public async Task<LocationCreationResult> RenameLocation(string locationName, string newLocationName)
        {
            var normalized = locationName.Normalise();

            var system = await _db.Systems.FirstOrDefaultAsync(x => x.NormalizedName == normalized);

            if (system == null)
                return LocationCreationResult.CantFindSystem;

            system.Name = newLocationName;
            system.NormalizedName = newLocationName.Normalize();

            await _db.SaveChangesAsync();

            return LocationCreationResult.OK;
        }
    }


    public enum LocationCreationResult
    {
        OK,
        Duplicate,
        CantFindSystem
    }
}
