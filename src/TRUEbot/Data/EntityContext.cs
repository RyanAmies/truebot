using Microsoft.EntityFrameworkCore;
using TRUEbot.Data.Models;

namespace TRUEbot.Data
{
    public class EntityContext : DbContext
    {
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Models.System> Systems { get; set; }
        public virtual DbSet<Models.SystemLog> SystemLogs { get; set; }
        public virtual DbSet<Hit> Hits { get; set; }
    }
}
