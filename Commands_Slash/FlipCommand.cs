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
            await context.DeferResponseAsync();

            Random random = new Random();
            string answer = "You flip a coin…\n\nIt's ";
            if (random.NextDouble() >= 0.5) { answer += $"{new CoinEmote().GetEmote(true)} heads!"; }
            else answer += $"{new CoinEmote().GetEmote(false)} tails!";

            await context.RespondAsync(answer);
        }
    }
}
