using Acorn.AutoCompleteProviders;
using Acorn.Classes;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;

namespace Acorn.Commands_Slash
{
    public class ExchangeCommand
    {
        [Command("exchange"), Description("Converts a value between currencies.")]
        public static async ValueTask ExecuteAsync(CommandContext context,
            [Description("The value you wish to convert.")] string inputValue,
            [Description("The currency you with to convert from."), SlashAutoCompleteProvider<ExchangeCommandAutoCompleteProvider>] string inputCurrency,
            [Description("The currency you with to convert to, or Quick Reference."), SlashAutoCompleteProvider<ExchangeCommandAutoCompleteProvider>] string outputCurrency)
        {
            ExecTime ExchangeTime = new("Currency-exchanging", "Exchanging currencies");

            await context.RespondAsync(Program.exchange.DoExchange(inputValue, inputCurrency, outputCurrency));

            ExchangeTime.Stop();
        }
    }
}
