using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TRUEbot.Bot.Services;

namespace TRUEbot.Bot.Data.Models
{
    public class EntityContextDesignTimeFactory : IDesignTimeDbContextFactory<EntityContext>
    {
        EntityContext IDesignTimeDbContextFactory<EntityContext>.CreateDbContext(string[] args)
        {
            var configuration = BotConfigurationBuilder.Build();

            var builder = new DbContextOptionsBuilder<EntityContext>();

            var connectionString = configuration.GetBotDbConnectionString();

            builder.UseSqlite(connectionString);

            return new EntityContext(builder.Options);
        }
    }
}
