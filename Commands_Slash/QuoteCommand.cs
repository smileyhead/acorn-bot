using Acorn.Classes;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class QuoteCommand
    {
        [Command("quote"), Description("Prints a random quote from the collection.")]
        public static async ValueTask ExecuteAsync(CommandContext context)
        {
            ExecTime RandomQuoteTime = new("Quote-returning", "Returning a quote");

            (DiscordMessageBuilder message, string secondHalf) = Program.quotesList.Print("", true, "");

            await context.RespondAsync(message);

            RandomQuoteTime.Stop();

            if (secondHalf != "") { context.Channel.SendMessageAsync(secondHalf); }

            Program.quotesList.Reshuffle();
        }
    }
}
