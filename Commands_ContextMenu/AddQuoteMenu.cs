using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;

namespace Acorn.Commands_ContextMenu
{
    public class AddQuoteMenu
    {
        [Command("Add Quote")]
        [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
        [AllowedProcessors(typeof(MessageCommandProcessor))]
        public async Task AddQuote(MessageCommandContext context, DiscordMessage message)
        {
            var AddQuoteTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Adding a quote.");

            await context.RespondAsync(Program.quotesList.Add(context, message));
            AddQuoteTime.Stop();

            Program.quotesList.CountQuotes();

            Console.WriteLine($"  Quote-adding finished in {AddQuoteTime.ElapsedMilliseconds}ms.");
            if (AddQuoteTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Adding a quote took {AddQuoteTime.ElapsedMilliseconds}ms."); }
        }
    }
}
