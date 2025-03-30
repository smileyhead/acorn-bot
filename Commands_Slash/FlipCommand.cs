using Acorn.Classes;
using DSharpPlus.Commands;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class FlipCommand
    {
        [Command("flip"), Description("Flips a coin.")]
        public static async ValueTask ExecuteAsync(CommandContext context)
        {
            ExecTime FlipTime = new("Coin-flipping", "Flipping a coin");

            Random random = new Random();
            string answer = "You flip a coin…\n\nIt's ";
            if (random.NextDouble() >= 0.5) { answer += "heads!"; }
            else answer += "tails!";

            await context.RespondAsync(answer);

            FlipTime.Stop();
        }
    }
}
