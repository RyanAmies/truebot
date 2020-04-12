using System;

namespace TRUEbot.Bot.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExcludeFromHelpAttribute : Attribute
    {
    }
}
