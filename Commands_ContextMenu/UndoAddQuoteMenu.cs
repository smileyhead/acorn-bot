using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;

namespace Acorn.Commands_ContextMenu
{
    public class UndoAddQuoteMenu
    {
        [Command("Undo Last Quote")]
        [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
        [AllowedProcessors(typeof(MessageCommandProcessor))]
        public async Task UndoAddQuote(MessageCommandContext context)
        {
            var UndoAddQuoteTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Undoing the last quote.");

            await context.RespondAsync(Program.quotesList.Undo(context));

            UndoAddQuoteTime.Stop();
            Console.WriteLine($"  Quote-undoing finished in {UndoAddQuoteTime.ElapsedMilliseconds}ms.");
            if (UndoAddQuoteTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Undoing a quote took {UndoAddQuoteTime.ElapsedMilliseconds}ms."); }
        }
    }
}
