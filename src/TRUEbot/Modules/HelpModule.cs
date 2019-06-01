using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace TRUEbot.Modules
{
    [Group("help")]
    public class HelpModule : InteractiveBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public HelpModule(CommandService service)
        {
            _service = service;
        }

        [Command]
        public async Task Help()
        {
            var pagesss = _service.Modules.Select(x => new PaginatedMessage.Page
            {
                Title = x.Name,
                Description = x.Summary ?? (x.Group != null ? $"All commands prefixed with {x.Group}" : "The following commands can be used"),
                Fields = x.Commands
                    .Select(f => new EmbedFieldBuilder
                    {
                        Name = string.Join(", ", f.Aliases),
                        Value = f.Summary ?? "No summary",
                    }).ToList(),
            }).ToList();

            var pager = new PaginatedMessage
            {
                Pages = pagesss,
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
