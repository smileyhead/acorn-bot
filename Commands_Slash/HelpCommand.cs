using Acorn.AutoCompleteProviders;
using Acorn.Classes;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class HelpCommand
    {
        [Command("help"), Description("Prints the help article for a given command.")]
        public static async ValueTask ExecuteAsync
            (CommandContext context,
            [Description("The command which you need help with."), SlashAutoCompleteProvider<HelpCommandAutoCompleteProvider>] string command)
        {
            ExecTime HelpTime = new("Help article-returning", "Returning a help article");

            await context.RespondAsync(Program.helpArticlesList.GetHelp(command));

            HelpTime.Stop();
        }
    }
}
