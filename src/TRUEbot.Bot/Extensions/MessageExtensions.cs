using System.Threading.Tasks;
using Discord;

namespace TRUEbot.Bot.Extensions
{
    public static class MessageExtensions
    {
        public static Task AddErrorEmote(this IUserMessage message)
        {
            return message.AddReactionAsync(new Emoji("❌"));
        }

        public static Task AddSuccessEmote(this IUserMessage message)
        {
            return message.AddReactionAsync(new Emoji("✅"));
        }
    }
}
