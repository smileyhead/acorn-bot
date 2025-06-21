using DSharpPlus.Commands;
using DSharpPlus.Entities;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class SearchQuoteCommand
    {
        [Command("searchquote"), Description("Searches for quotes that match the given query.")]
        public static async ValueTask ExecuteAsync(CommandContext context, [Description("The search query. At least 2 characters long.")] string query)
        {
            await context.DeferResponseAsync();

            (DiscordMessageBuilder message, string secondHalf) = Program.quotesList.Search(query);

            await context.RespondAsync(message);

            if (secondHalf != "") { context.Channel.SendMessageAsync(secondHalf); }
        }
    }
}
