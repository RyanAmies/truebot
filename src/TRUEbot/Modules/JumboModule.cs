using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JetBrains.Annotations;
using Serilog;

namespace TRUEbot.Modules
{
    [Group("jumbo")]
    [UsedImplicitly]
    public class JumboModule : ModuleBase
    {
        [Command, Summary("Jumbo's an emoji (AKA makes an emoji quite big for chat for dramatic effect)")]
        [UsedImplicitly]
        public async Task Jumbo(string emoji)
        {
            string emojiUrl;

            if (Emote.TryParse(emoji, out var parsedEmoji))
            {
                emojiUrl = parsedEmoji.Url;
            }
            else
            {
                var codepoint = char.ConvertToUtf32(emoji, 0);
                var codepointHex = codepoint.ToString("X").ToLower();
                emojiUrl = $"https://raw.githubusercontent.com/twitter/twemoji/gh-pages/2/72x72/{codepointHex}.png";
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var req = await client.GetStreamAsync(emojiUrl);

                    await Context.Channel.SendFileAsync(req, Path.GetFileName(emojiUrl), Context.User.Mention);

                    await Context.Message.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed jumbo'ing emoji {emoji}", emoji);
            }
        }
    }
}
