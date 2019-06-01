using Microsoft.EntityFrameworkCore;
using TRUEbot.Data.Models;

namespace TRUEbot.Data
{
    public class EntityContext : DbContext
    {
        public EntityContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=truebot.db");
        }
        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        { }

        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Models.System> Systems { get; set; }
        public virtual DbSet<Models.SystemLog> SystemLogs { get; set; }
        public virtual DbSet<Hit> Hits { get; set; }
        public virtual DbSet<Kill> Kills { get; set; }
    }
}
