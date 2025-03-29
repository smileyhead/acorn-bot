using Acorn.AutoCompleteProviders;
using Acorn.Classes;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class QuoteByCommand
    {
        [Command("quoteby"), Description("Prints a random quote from the chosen person.")]
        public static async ValueTask ExecuteAsync(CommandContext context,
            [Description("The author of the quote you wish to recall."), SlashAutoCompleteProvider<QuoteByCommandAutoCompleteProvider>] string authorId)
        {
            ExecTime SpecificQuoteTime = new("Quote-returning", "Returning a quote");

            await context.RespondAsync(Program.quotesList.QuoteBy(authorId));

            SpecificQuoteTime.Stop();
        }
    }
}
