using System.Threading.Tasks;
using Discord.Commands;
using JetBrains.Annotations;

namespace TRUEbot.Modules
{
    [Group("ping")]
    [UsedImplicitly]
    public class PingModule : ModuleBase
    {
        [Command, Summary("Pings the bot and sends a response, if the bot is alive!")]
        [UsedImplicitly]
        public Task Ping()
        {
            return ReplyAsync("Beep boop");
        }
    }
}
