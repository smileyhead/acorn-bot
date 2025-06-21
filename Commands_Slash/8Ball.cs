using DSharpPlus.Commands;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class _8BallCommand
    {
        [Command("8ball"), Description("Tells your fortune.")]
        public static async ValueTask ExecuteAsync(CommandContext context, [Description("Your question to the magic 8-ball.")] string question)
        {
            await context.DeferResponseAsync();

            string answer = "";

            if (question == null) answer += "I'm sorry, but I can't tell your fortune if you don't ask me anything!";
            else
            {
                if (!question.EndsWith('?')) answer += $"That's not a question! But I'll accept it anyway.\n";
                answer += $"You turn over the magic 8-ball and ask, ‘{question}’…\n\nWhen you lift it back up it says, ‘{Program.magic8Ball.ReturnFortune(context.User.Id, question)}’";
            }

            await context.RespondAsync(answer);
        }
    }
}
