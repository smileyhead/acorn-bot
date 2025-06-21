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
            await context.DeferResponseAsync();

            bool hasReroll = false;
            string[] dicePreSplit = new string[2];
            int[] diceSplit = new int[2];
            dice = dice.ToLower();

            if (dice.Contains("r1")) { hasReroll = true; dice = dice.Remove(dice.IndexOf("r1"), 2); }

            var declare = CheckModifier(dice);
            dice = declare.dice;
            char modifierType = declare.modifierType;
            int modifier = declare.modifier;
            string answer = declare.answer;
            bool alreadyAnswered = declare.alreadyAnswered;

            if (!dice.Contains('d')) { answer = "Error: Your input has no `d`. For help, see: `/help r`."; alreadyAnswered = true; }

            if (!alreadyAnswered)
            {
                if (dice[0] == 'd')
                {
                    if (!int.TryParse(dice.Substring(1), out diceSplit[1])) { answer = "Error: Invalid format. For help, see: `/help r`."; }
                    else if (diceSplit[1] < 1 || diceSplit[1] > 100) { answer = "Error: The number of dice or the number of sides falls outside of the accepted range. For help, see: `/help r`."; }
                    else { answer = RollDice(1, diceSplit[1], modifierType, modifier, hasReroll); }
                }
                else
                {
                    dicePreSplit = dice.Split('d');
                    if (!int.TryParse(dicePreSplit[0], out diceSplit[0])) { answer = "Error: Invalid format. For help, see: `/help r`."; }
                    else if (!int.TryParse(dicePreSplit[1], out diceSplit[1])) { answer = "Error: Invalid format. For help, see: `/help r`."; }
                    else if (diceSplit[0] < 1 || diceSplit[0] > 10 || diceSplit[1] < 1 || diceSplit[1] > 100) { answer = "Error: The number of dice or the number of sides falls outside of the accepted range. For help, see: `/help r`."; }
                    else { answer = RollDice(diceSplit[0], diceSplit[1], modifierType, modifier, hasReroll); }
                }
            }

            await context.RespondAsync(answer);

            static (string dice, char modifierType, int modifier, string answer, bool alreadyAnswered) CheckModifier(string dice)
            {
                Console.WriteLine("  Checking dice modifiers.");

                string answer = "";
                bool alreadyAnswered = false;
                char modifierType = '\0';
                int modifier = 0;

                if (dice.Contains('+')) { modifierType = '+'; }
                else if (dice.Contains('-')) { modifierType = '-'; }

                if (modifierType == '+' || modifierType == '-')
                {
                    if (!int.TryParse(dice.Substring(dice.IndexOf(modifierType) + 1), out modifier))
                    {
                        answer = "Error: Incorrect format. For help, see `/help r`.";
                        alreadyAnswered = true;
                    }
                    dice = dice.Remove(dice.IndexOf(modifierType));
                }

                return (dice, modifierType, modifier, answer, alreadyAnswered);
            }

            static string FormatNumber(int number)
            {
                return number.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
            }

            static string RollDice(int diceN, int sidesN, char modifierType, int modifier, bool hasReroll)
            {
                Console.WriteLine("  Rolling dice.");

                string answer = "";
                Random random = new Random();
                int[] rolls = new int[diceN];
                string separator = ", ";
                string[] numberNames = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"];
                bool emergencyStopActivated = false;
                bool emergencyStopActivatedThisRound = false;

                if (diceN == 1) { answer += $"You roll a {sidesN}-sided die…\n\n"; }
                else { answer += $"You roll {numberNames[diceN - 1]} {sidesN}-sided dice…\n\n"; }

                for (int i = 0; i < diceN; i++)
                {
                    rolls[i] = random.Next(1, sidesN + 1);
                    if (i == diceN - 1) { separator = " "; }
                    emergencyStopActivatedThisRound = false;

                    if (rolls[i] == sidesN) //Critical success
                    {
                        answer += $"**{rolls[i]}**{separator}";
                    }
                    else if (rolls[i] == 1 && hasReroll) //Critical fail with reroll
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            rolls[i] = random.Next(1, sidesN + 1);
                            if (rolls[i] > 1) { break; }
                            else if (j == 9)
                            {
                                if (sidesN > 2) { rolls[i] = random.Next(2, sidesN + 1); }
                                else { rolls[i] = 2; }
                                emergencyStopActivatedThisRound = true;
                                emergencyStopActivated = true;
                                break;
                            }
                        }
                        if (emergencyStopActivatedThisRound) { answer += $"__*{rolls[i]}*__{separator}"; }
                        else { answer += $"__{rolls[i]}__{separator}"; }
                    }
                    else if (rolls[i] == 1 && !hasReroll) //Critical fail
                    {
                        answer += $"__{rolls[i]}__{separator}";
                    }
                    else { answer += $"{rolls[i]}{separator}"; } //Normal roll
                }

                if (modifierType != '\0')
                {
                    if (modifierType == '+') { answer += $"{modifierType} {FormatNumber(modifier)} → {FormatNumber(rolls.Sum() + modifier)}"; }
                    else if (modifierType == '-') { answer += $"{modifierType} {FormatNumber(modifier)} → {FormatNumber(rolls.Sum() - modifier)}"; }
                }
                else if (diceN > 1) { answer += $"→ {FormatNumber(rolls.Sum())}"; }

                if (emergencyStopActivated) { answer += $"\n-# Note: The rerolling of one or more of these values exceeded 10 tries. For more information, see: `/help r`."; }
                return answer;
            }
        }
    }
}
