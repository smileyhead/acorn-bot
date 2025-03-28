using Acorn.AutoCompleteProviders;
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
            var HelpTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Returning a help article.");

            await context.RespondAsync(Program.helpArticlesList.GetHelp(command));

            HelpTime.Stop();
            Console.WriteLine($"  Help article-returning finished in {HelpTime.ElapsedMilliseconds}ms.");
            if (HelpTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Returning a help article took {HelpTime.ElapsedMilliseconds}ms."); }
        }
    }
}
