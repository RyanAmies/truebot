using System.Threading.Tasks;
using Discord.Commands;
using JetBrains.Annotations;

namespace TRUEbot.Bot.Modules
{
    public class DebugModule : ModuleBase
    {
        [Command("ping")]
        public Task Ping()
        {
            return ReplyAsync("🏓 Pong!");
        }
    }
}