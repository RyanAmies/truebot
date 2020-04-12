using System;
using System.Collections.Generic;
using Discord;

namespace TRUEbot.Bot.Extensions
{
    public static class EmbedPaginator
    {
       
        public static List<EmbedBuilder> Paginate(List<string> dataRows, string title, string field, string footer)
        {
            const int LIMIT = 1020;

            var pageText = "";
            var builders = new List<EmbedBuilder>();

            foreach (var text in dataRows)
            {
                if (pageText.Length + text.Length > LIMIT)
                {
                    var embed = GetEmbedBuilder(title, field, footer, pageText);


                    builders.Add(embed);

                    pageText = text;
                }
                else
                {
                    pageText += Environment.NewLine + text;
                }

            }

            var finalEmbed = GetEmbedBuilder(title, field, footer, pageText);
            builders.Add(finalEmbed);

            var page = 1;
            var pages = builders.Count;

            if (builders.Count > 1)
            {
                foreach (var embedBuilder in builders)
                {
                    embedBuilder.Title += $" {page++} of {pages}";
                }
            }

            return builders;
        }

        private static EmbedBuilder GetEmbedBuilder(string title, string field, string footer, string pageText)
        {
            var embed = new EmbedBuilder().WithTitle(title);

            embed.AddField(field, pageText);
            embed.WithFooter(footer).WithColor(new Color(95, 186, 125));
            return embed;
        }
    }
}
