using DSharpPlus.Commands;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class FlipCommand
    {
        [Command("flip"), Description("Flips a coin.")]
        public static async ValueTask ExecuteAsync(CommandContext context)
        {
            var FlipTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Flipping a coin.");

            Random random = new Random();
            string answer = "You flip a coin…\n\nIt's ";
            if (random.NextDouble() >= 0.5) { answer += "heads!"; }
            else answer += "tails!";

            await context.RespondAsync(answer);

            FlipTime.Stop();
            Console.WriteLine($"  Coin-flipping finished in {FlipTime.ElapsedMilliseconds}ms.");
            if (FlipTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Flipping a coin took {FlipTime.ElapsedMilliseconds}ms."); }
        }
    }
}
