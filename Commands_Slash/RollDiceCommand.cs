using Acorn.Classes;
using DSharpPlus.Commands;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class RollDiceCommand
    {
        [Command("roll"), Description("Rolls n x-sided dice. Example: ‘/r 2d4’. Details: ‘/help r’.")]
        public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of dice to roll and the number of their sides. Example: ‘2d4’.")] string dice)
        {
            var RollTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Rolling dice.");

            Dice dice_command = new();
            await context.RespondAsync(dice_command.Roll(dice));

            RollTime.Stop();
            Console.WriteLine($"  Character-generating finished in {RollTime.ElapsedMilliseconds}ms.");
            if (RollTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Generating a character took {RollTime.ElapsedMilliseconds}ms."); }
        }
    }
}
