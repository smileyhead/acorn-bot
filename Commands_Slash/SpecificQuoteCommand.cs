using Acorn.Classes;
using DSharpPlus.Commands;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class SpecificQuoteCommand
    {
        [Command("specificquote"), Description("Prints a specified quote from the collection.")]
        public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of the quote you wish to recall.")] string quoteId)
        {
            ExecTime SpecificQuoteTime = new("Quote-returning", "Returning a quote");

            await context.RespondAsync(Program.quotesList.Print(quoteId, false));

            SpecificQuoteTime.Stop();
        }
    }
}
