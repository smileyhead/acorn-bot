using Acorn.Classes;
using DSharpPlus.Commands;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class QuoteCommand
    {
        [Command("quote"), Description("Prints a random quote from the collection.")]
        public static async ValueTask ExecuteAsync(CommandContext context)
        {
            ExecTime RandomQuoteTime = new("Quote-returning", "Returning a quote");

            await context.RespondAsync(Program.quotesList.Print("", true));

            RandomQuoteTime.Stop();

            Program.quotesList.Reshuffle();
        }
    }
}
