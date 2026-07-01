using Acorn.AutoCompleteProviders;
using Acorn.Classes;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class DadJokeCommand
    {
        [Command("dadjoke"), Description("Returns a random dad joke.")]
        public static async ValueTask ExecuteAsync(CommandContext context)
        {
            await context.DeferResponseAsync();

            Console.WriteLine("Returning a dad joke.");

            string answer;

            try
            {
                string responseBody = await Program.httpClient.GetStringAsync("https://icanhazdadjoke.com/");

                answer = $"I've got you, kid:\n\n{responseBody}";
            }
            catch (HttpRequestException e)
            {
                answer =
                    $"I'm sorry, kid. I've tried, but couldn't come up with one.\n\nThe API returned an error: {e.Message}";
            }

            await context.RespondAsync(answer);
        }
    }
}