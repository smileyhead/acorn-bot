using DSharpPlus.Commands;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class CharacterCommand
    {
        [Command("character"), Description("Prints a randomly-rolled Dungeons and Dragons character block.")]
        public static async ValueTask ExecuteAsync(CommandContext context)
        {
            await context.DeferResponseAsync();

            int[] RollCharacter(int[] set)
            {
                Random random = new Random();
                for (int i = 0; i < 4; i++) { set[i] = random.Next(1, 7); }
                return set;
            }

            string GetValues(int[] set, string answer)
            {
                int minI = Array.IndexOf(set, set.Min());
                string separator = ", ";

                for (int i = 0; i < set.Length; i++)
                {
                    if (i == set.Length - 1) { separator = " "; }

                    if (i == minI) { answer += $"{set[i]}{separator}"; }
                    else { answer += $"**{set[i]}**{separator}"; }
                }

                answer += $"→ {set.Sum() - set[minI]}\n";
                return answer;
            }

            int[] set = new int[4];
            int[] sums = new int[6];
            string answer = "";

            for (int i = 0; i < 6; i++)
            {
                RollCharacter(set);
                sums[i] = set.Sum() - set[Array.IndexOf(set, set.Min())];
                answer = GetValues(set, answer);
            }

            answer += $"\nTotal: {sums.Sum()}";
            await context.RespondAsync(answer);
        }
    }
}
