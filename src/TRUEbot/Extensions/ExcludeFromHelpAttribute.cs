using System;
using System.Collections.Generic;
using System.Text;

namespace TRUEbot.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExcludeFromHelpAttribute : Attribute
    {
    }
}
