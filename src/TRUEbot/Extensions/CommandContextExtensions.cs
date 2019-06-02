﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TRUEbot.Extensions
{
    public static class CommandContextExtensions
    {
        public static readonly Emoji CheckmarkEmoji = new Emoji("✅");
        public static readonly Emoji CrossEmoji = new Emoji("❌");

        public static Task AddConfirmation(this ICommandContext context)
            => context.Message.AddReactionAsync(CheckmarkEmoji);

        public static Task AddRejection(this ICommandContext context)
            => context.Message.AddReactionAsync(CrossEmoji);
    }
}
