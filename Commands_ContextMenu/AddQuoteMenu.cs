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
            await context.DeferResponseAsync();

            var AddQuoteTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Adding a quote.");

            (string firstHalf, string secondHalf, string alttext) = Program.quotesList.Add(context, message);

            await context.RespondAsync(firstHalf);
            AddQuoteTime.Stop();

            if (secondHalf != "") context.Channel.SendMessageAsync(secondHalf);
            if (alttext != "") context.Channel.SendMessageAsync(alttext);

            Program.quotesList.CountQuotes();

            Console.WriteLine($"  Quote-adding finished in {AddQuoteTime.ElapsedMilliseconds}ms.");
        }
    }
}
