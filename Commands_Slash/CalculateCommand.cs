using DSharpPlus.Commands;
using System.ComponentModel;
using System.Globalization;
using Dangl.Calculator;

namespace Acorn.Commands_Slash
{
    public class CalculateCommand
    {
        [Command("calculate"), Description("Evaluates a numerical expression.")]
        public static async ValueTask ExecuteAsync(CommandContext context, [Description("The expression to evaluate.")] string expression)
        {
            await context.DeferResponseAsync();

            Console.WriteLine("Evaluating an expression.");

            string answer;
            
            var calculation = Calculator.Calculate(expression);

            if (!calculation.IsValid) answer = $"I'm sorry, but your expression couldn't be parsed. There seems to be an error at position {calculation.ErrorPosition}.";
            else answer = $"You punch `{expression}` into a calculator.\n\nIt returns the following value: **{calculation.Result.ToString("##,#.####", CultureInfo.CreateSpecificCulture("en-US"))}**";
                //In the /convert and /exchange commands, it is expected to have decimal precision, even if the returned number
                //doesn't need it. Here, I deemed it better to not have decimal precision unless required.

            if (answer.Length > 2000)
                answer =
                    "You punch these numbers into a calculator.\n\nThe calculator explodes!\n-# I'm sorry, but the result has exceeded Discord's 2000-character message limit. Please try again with a different problem or use an external calculator.";

            await context.RespondAsync(answer);
        }
    }
}
