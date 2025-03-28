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
            var RandomQuoteTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Returning a random quote.");
            await context.RespondAsync(Program.quotesList.Print("", true));

            RandomQuoteTime.Stop();
            Console.WriteLine($"  Quote-returning finished in {RandomQuoteTime.ElapsedMilliseconds}ms. Random quote index is now {Program.quotesList.GetShuffledIndex()}.");
            if (RandomQuoteTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Returning a random quote took {RandomQuoteTime.ElapsedMilliseconds}ms."); }

            Program.quotesList.Reshuffle();
        }
    }
}
