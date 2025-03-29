using Acorn.Classes;
using DSharpPlus.Commands;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class SearchQuoteCommand
    {
        [Command("searchquote"), Description("Searches for quotes that match the given query.")]
        public static async ValueTask ExecuteAsync(CommandContext context, [Description("The search query. At least 2 characters long.")] string query)
        {
            ExecTime SearchQuoteTime = new("Quote-searching", "Searching for quotes");

            await context.RespondAsync(Program.quotesList.Search(query));

            SearchQuoteTime.Stop();
        }
    }
}
