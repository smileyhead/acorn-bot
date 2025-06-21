using Acorn.AutoCompleteProviders;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;
using System.Globalization;
using UnitsNet;

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
            await context.DeferResponseAsync();

            bool alreadyAnswered = false;
            bool infiniteValue = false;

            if (inputValue == "∞")
            {
                infiniteValue = true;
                inputValue = "1";
            }

            if (!double.TryParse(inputValue, CultureInfo.InvariantCulture, out double inputDouble))
            {
                await context.RespondAsync($"Error: The input value could not be parsed.");
                alreadyAnswered = true;
            }

            string[] inputUnitSplit = inputUnit.Split('.');
            IQuantity? inputQuantity = null;
            if (!alreadyAnswered)
            {
                try { Quantity.TryFrom(value: inputDouble, quantityName: inputUnitSplit[0], unitName: inputUnitSplit[1], out inputQuantity); }
                catch
                {
                    await context.RespondAsync($"Error: The input type `{inputUnit}` is invalid.");
                    alreadyAnswered = true;
                }
            }

            string[] outputUnitSplit = outputUnit.Split('.');
            IQuantity? outputQuantity = null;
            if (!alreadyAnswered)
            {
                try { Quantity.TryFrom(value: 0, quantityName: outputUnitSplit[0], unitName: outputUnitSplit[1], out outputQuantity); }
                catch
                {
                    await context.RespondAsync($"Error: The output type `{outputUnit}` is invalid.");
                    alreadyAnswered = true;
                }
            }

            if (!alreadyAnswered && inputQuantity.QuantityInfo.UnitType != outputQuantity.QuantityInfo.UnitType)
            {
                await context.RespondAsync($"Error: The input and output categories `{inputQuantity.QuantityInfo.UnitType}`, `{outputQuantity.QuantityInfo.UnitType}` do not match.");
                alreadyAnswered = true;
            }

            else if (!alreadyAnswered)
            {
                string inputNumberFormatted = inputQuantity.ToString("N2", CultureInfo.CreateSpecificCulture("en-US"));
                string outputNumberFormatted = inputQuantity.ToUnit(outputQuantity.Unit).ToString("N2", CultureInfo.CreateSpecificCulture("en-US"));

                if (char.IsDigit(inputNumberFormatted[inputNumberFormatted.Length - 1])) { inputNumberFormatted += $" {inputUnitSplit[1]}"; }
                if (char.IsDigit(outputNumberFormatted[outputNumberFormatted.Length - 1])) { outputNumberFormatted += $" {outputUnitSplit[1]}"; }

                if (infiniteValue)
                {
                    inputNumberFormatted = "∞" + inputNumberFormatted.Substring(inputNumberFormatted.IndexOf(' '));
                    outputNumberFormatted = "∞" + outputNumberFormatted.Substring(outputNumberFormatted.IndexOf(' '));
                }

                string answer = $"**{inputNumberFormatted}** equals **{outputNumberFormatted}**.";

                await context.RespondAsync(answer);
            }
        }
    }
}
