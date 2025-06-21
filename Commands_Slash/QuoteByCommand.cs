using Acorn.AutoCompleteProviders;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class QuoteByCommand
    {
        [Command("quoteby"), Description("Prints a random quote from the chosen person.")]
        public static async ValueTask ExecuteAsync(CommandContext context,
            [Description("The author of the quote you wish to recall."), SlashAutoCompleteProvider<QuoteByCommandAutoCompleteProvider>] string authorId)
        {
            await context.DeferResponseAsync();

            (DiscordMessageBuilder message, string secondHalf) = Program.quotesList.QuoteBy(authorId);

            await context.RespondAsync(message);

            if (secondHalf != "") { context.Channel.SendMessageAsync(secondHalf); }
        }
    }
}
