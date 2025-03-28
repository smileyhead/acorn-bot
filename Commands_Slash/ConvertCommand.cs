using Acorn.AutoCompleteProviders;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class ConvertCommand
    {
        [Command("convert"), Description("Converts a value between two units.")]
        public static async ValueTask ExecuteAsync(CommandContext context,
            [Description("The value you wish to convert.")] string inputValue,
            [Description("The unit you with to convert from."), SlashAutoCompleteProvider<ConvertCommandAutoCompleteProvider>] string inputUnit,
            [Description("The unit you wish to convert to."), SlashAutoCompleteProvider<ConvertCommandAutoCompleteProvider>] string outputUnit)
        {
            var ConvertTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Converting a value.");

            Classes.Convert convert = new();
            await context.RespondAsync(convert.DoConvert(inputValue, inputUnit, outputUnit));

            ConvertTime.Stop();
            Console.WriteLine($"  Value-converting finished in {ConvertTime.ElapsedMilliseconds}ms.");
            if (ConvertTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Converting a value took {ConvertTime.ElapsedMilliseconds}ms."); }
        }
    }
}
