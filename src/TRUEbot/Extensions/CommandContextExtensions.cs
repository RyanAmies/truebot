using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TRUEbot.Extensions
{
    public static class CommandContextExtensions
    {
        private static readonly Emoji CheckmarkEmoji = new Emoji("✅");

        public static Task AddConfirmation(this ICommandContext context)
            => context.Message.AddReactionAsync(CheckmarkEmoji);
    }
}
