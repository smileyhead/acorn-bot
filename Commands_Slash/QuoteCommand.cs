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
            await context.DeferResponseAsync();

            (DiscordMessageBuilder message, string secondHalf) = Program.quotesList.Print("", true, "");

            await context.RespondAsync(message);

            if (secondHalf != "") { context.Channel.SendMessageAsync(secondHalf); }

            Program.quotesList.Reshuffle();

            Console.WriteLine($"{Program.quotesList.GetShuffledIndex()}/{Program.quotesList.GetQuotesNo()} quotes have been returned.");
        }
    }
}
