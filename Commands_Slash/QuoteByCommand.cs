using DSharpPlus.Commands;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class QuoteByCommand
    {
        [Command("quoteby"), Description("Prints a random quote from the chosen person.")]
        public static async ValueTask ExecuteAsync(CommandContext context, [Description("The author of the quote you wish to recall.")] string authorId)
        {
            var SpecificQuoteTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Returning a specific quote.");

            await context.RespondAsync(Program.quotesList.QuoteBy(authorId));

            SpecificQuoteTime.Stop();
            Console.WriteLine($"  Quote-returning finished in {SpecificQuoteTime.ElapsedMilliseconds}ms.");
            if (SpecificQuoteTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Returning a quote took {SpecificQuoteTime.ElapsedMilliseconds}ms."); }
        }
    }
}
