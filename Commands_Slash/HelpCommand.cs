using Acorn.AutoCompleteProviders;
using Acorn.Classes;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class HelpCommand
    {
        [Command("help"), Description("Prints the help article for a given command.")]
        public static async ValueTask ExecuteAsync
            (CommandContext context,
            [Description("The command which you need help with."), SlashAutoCompleteProvider<HelpCommandAutoCompleteProvider>] string command)
        {
            await context.DeferResponseAsync();

            await context.RespondAsync(Program.helpArticlesList.GetHelp(command));
        }
    }
}
