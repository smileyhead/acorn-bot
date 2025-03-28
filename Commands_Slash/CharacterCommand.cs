using Acorn.Classes;
using DSharpPlus.Commands;
using System.ComponentModel;
using System.Globalization;

namespace Acorn.Commands_Slash
{
    public class CharacterCommand
    {
        [Command("character"), Description("Prints a randomly-rolled Dungeons and Dragons character block.")]
        public static async ValueTask ExecuteAsync(CommandContext context)
        {
            var CharacterTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Generating a character.");

            Character character = new();
            await context.RespondAsync(character.Roll());

            CharacterTime.Stop();
            Console.WriteLine($"  Character-generating finished in {CharacterTime.ElapsedMilliseconds}ms.");
            if (CharacterTime.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"Generating a character took {CharacterTime.ElapsedMilliseconds}ms."); }
        }
    }
}
