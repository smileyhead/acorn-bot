using System.Globalization;
using UnitsNet;

namespace Acorn
{
    internal class Convert
    {
        public string DoConvert(string inputValue, string inputUnit, string outputUnit)
        {
            if (!double.TryParse(inputValue, CultureInfo.InvariantCulture, out double inputDouble))
            {
                return $"Error: The input value could not be parsed.";
            }

            string[] inputUnitSplit = inputUnit.Split('.');
            IQuantity? inputQuantity = null;
            if (!Quantity.TryFrom(value: inputDouble, quantityName: inputUnitSplit[0], unitName: inputUnitSplit[1], out inputQuantity))
            {
                return $"Error: The input type `{inputUnit}` is invalid.";
            }

            string[] outputUnitSplit = outputUnit.Split('.');
            IQuantity? outputQuantity = null;
            if (Quantity.TryFrom(value: 0, quantityName: outputUnitSplit[0], unitName: outputUnitSplit[1], out outputQuantity))
            {
                return $"Error: The output type `{outputUnit}` is invalid.";
            }

            if (inputQuantity.QuantityInfo.UnitType != outputQuantity.QuantityInfo.UnitType)
            {
                return $"Error: The input and output categories `{inputQuantity.QuantityInfo.UnitType}`, `{outputQuantity.QuantityInfo.UnitType}` do not match.";
            }

            else
            {
                string inputNumberFormatted = inputQuantity.ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));
                string outputNumberFormatted = inputQuantity.ToUnit(outputQuantity.Unit).ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));

                if (char.IsDigit(inputNumberFormatted[inputNumberFormatted.Length - 1])) { inputNumberFormatted += $" {inputUnitSplit[1]}"; }
                if (char.IsDigit(outputNumberFormatted[outputNumberFormatted.Length - 1])) { outputNumberFormatted += $" {outputUnitSplit[1]}"; }

                string answer = $"**{inputNumberFormatted}** equals **{outputNumberFormatted}**.";

                return answer;
            }
        }
    }
}
