using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.TextCommands;

namespace Acorn.Commands_Text
{
    public static class AlttextCommand
    {
        [Command("alttext")]
        public static async ValueTask TextOnlyAsync(TextCommandContext context, string quoteId, [RemainingText] string input)
        {
            Console.WriteLine("Overwriting alt text.");

            await context.RespondAsync(Program.quotesList.OverwriteAlttext(quoteId, input));
        }
    }
}
