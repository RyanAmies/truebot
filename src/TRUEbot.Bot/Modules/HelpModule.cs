using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace TRUEbot.Bot.Modules
{
    [Name(nameof(HelpModule)),Group("help")]
    public class HelpModule : InteractiveBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public HelpModule(CommandService service)
        {
            _service = service;
        }

        public List<(string, string)> HiddenMethodsFromHelp = new List<(string, string)>
        {
            (nameof(GeneralModule), nameof(GeneralModule.All)),
            (nameof(GeneralModule), nameof(GeneralModule.Normalise)),
            (nameof(HelpModule), null),
        };

        [Command]
        public async Task Help()
        {
            var pages = _service.Modules.Where(x => !HiddenMethodsFromHelp.Any((h) => h.Item1.Equals(x.Name) && h.Item2 == null)).Select(x => new PaginatedMessage.Page
            {
                Title = x.Name,
                Description = x.Summary ?? (x.Group != null ? $"All commands prefixed with {x.Group}" : "The following commands can be used"),
                Fields = x.Commands
                    .Where(f => !HiddenMethodsFromHelp.Any((h) => h.Item1.Equals(x.Name) && h.Item2.Equals(f.Name, StringComparison.OrdinalIgnoreCase)))
                    .Select(f => new EmbedFieldBuilder
                    {
                        Name = string.Join(", ", f.Aliases),
                        Value = BuildCommandInfo(f),
                    }).ToList(),
            }).ToList();

            string BuildCommandInfo(CommandInfo commandInfo)
            {
                var description = "";

                if (commandInfo.Summary != null)
                {
                    description += commandInfo.Summary;
                }

                description += Environment.NewLine + "Usage: `!" + commandInfo.Aliases.First() + " " + string.Join(" ", commandInfo.Parameters.Select(x => $"{{{x.Name}}}")) + "`";

                return description;
            }

            var pager = new PaginatedMessage
            {
                Pages = pages,
                Color = Color.DarkGreen,
                Description = "Default Embed Description",
                FooterOverride = null,
                Options = PaginatedAppearanceOptions.Default,
                TimeStamp = DateTimeOffset.UtcNow
            };

            await PagedReplyAsync(pager, new ReactionList
            {
                Forward = true,
                Backward = true,
                Jump = true,
                Trash = true
            });
        }
    }
}
