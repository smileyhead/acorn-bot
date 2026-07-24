using Acorn.AutoCompleteProviders;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Acorn.Classes;
using UnitsNet;
using UnitsNet.Units;

namespace Acorn.Commands_Slash
{
    public class ConvertCommand
    {
        [Command("convert"), Description("Converts a value between two units.")]
        public static async ValueTask ExecuteAsync(CommandContext context,
            [Description("The value you wish to convert.")]
            string inputValue,
            [Description("The unit you with to convert from."),
             SlashAutoCompleteProvider<ConvertCommandAutoCompleteProvider>]
            string inputUnit,
            [Description("The unit you wish to convert to."),
             SlashAutoCompleteProvider<ConvertCommandAutoCompleteProvider>]
            string outputUnit)
        {
            await context.DeferResponseAsync();

            Console.WriteLine("Converting a value.");

            bool infiniteValue = false;
            bool inputFtIn = false;
            bool outputFtIn = false;
            IQuantity? inputQuantity;
            IQuantity? outputQuantity;

            if (inputValue.Contains('∞'))
            {
                infiniteValue = true;
                inputValue = "1";
            }

            if (inputUnit.EndsWith("FeetInches"))
            {
                inputFtIn = true;
                inputUnit = "Length.Inch";
                inputValue = NumberExpander.ExpandFtIn(inputValue) ?? inputValue;
            }
            else inputValue = NumberExpander.Expand(inputValue) ?? inputValue;

            if (outputUnit.EndsWith("FeetInches"))
            {
                outputFtIn = true;
                outputUnit = "Length.Inch";
            }

            if (!double.TryParse(inputValue, CultureInfo.InvariantCulture, out double inputDouble))
            {
                await context.RespondAsync($"Error: The input value could not be parsed.");
                Console.WriteLine(inputValue);
                return;
            }

            string[] inputUnitSplit = inputUnit.Split('.');
            try
            {
                Quantity.TryFrom(value: inputDouble, quantityName: inputUnitSplit[0], unitName: inputUnitSplit[1],
                    out inputQuantity);
            }
            catch
            {
                await context.RespondAsync($"Error: The input type `{inputUnit}` is invalid.");
                return;
            }

            string[] outputUnitSplit = outputUnit.Split('.');
            try
            {
                Quantity.TryFrom(value: 0, quantityName: outputUnitSplit[0], unitName: outputUnitSplit[1],
                    out outputQuantity);
            }
            catch
            {
                await context.RespondAsync($"Error: The output type `{outputUnit}` is invalid.");
                return;
            }

            if (inputQuantity.QuantityInfo.UnitType != outputQuantity.QuantityInfo.UnitType)
            {
                await context.RespondAsync(
                    $"Error: The input and output categories `{inputQuantity.QuantityInfo.UnitType}`, `{outputQuantity.QuantityInfo.UnitType}` do not match.");
                return;
            }

            string inputNumberFormatted;
            string outputNumberFormatted;
            if (inputFtIn)
            {
                Length.TryParseFeetInches(inputQuantity.Value + "in", out Length inputNumberFtIn);
                inputNumberFormatted = inputNumberFtIn.FeetInches.ToString();
            }
            else inputNumberFormatted = inputQuantity.ToString("N2", CultureInfo.CreateSpecificCulture("en-US"));

            if (outputFtIn)
            {
                Length outputNumberFtIn = Length.FromInches(inputQuantity.ToUnit(outputQuantity.Unit).Value);
                outputNumberFormatted = FormatArchitecturalString(
                    outputNumberFtIn.FeetInches.ToArchitecturalString(100));
            }
            else
                outputNumberFormatted = inputQuantity.ToUnit(outputQuantity.Unit)
                    .ToString("N2", CultureInfo.CreateSpecificCulture("en-US"));

            if (char.IsDigit(inputNumberFormatted[inputNumberFormatted.Length - 1]))
            {
                inputNumberFormatted += $" {inputUnitSplit[1]}";
            }

            if (char.IsDigit(outputNumberFormatted[outputNumberFormatted.Length - 1]))
            {
                outputNumberFormatted += $" {outputUnitSplit[1]}";
            }

            if (infiniteValue)
            {
                inputNumberFormatted = "∞" + inputNumberFormatted.Substring(inputNumberFormatted.IndexOf(' '));
                outputNumberFormatted = "∞" + outputNumberFormatted.Substring(outputNumberFormatted.IndexOf(' '));
            }

            string answer = $"**{inputNumberFormatted}** equals **{outputNumberFormatted}**.";

            await context.RespondAsync(answer);
        }

        private static string FormatArchitecturalString(string input)
        {
            Regex zeroSpaceFinder = new Regex(@"\d+ \d+");
            var match = zeroSpaceFinder.Match(input);
            if (match.Success)
                input = $"{input[..match.Index]}{match.Value.Replace(' ', '​')}{input[(match.Index + match.Length)..]}";

            input = input.Replace("' -", " ft").Replace('/', '⁄').Replace("\"", " in");
            
            return input;
        }
    }
}