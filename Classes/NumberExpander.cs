using System.Globalization;
using System.Text.RegularExpressions;
using Dangl.Calculator;
using UnitsNet;
using UnitsNet.Units;

namespace Acorn.Classes;

public abstract class NumberExpander
{
    public static string? Expand(string number)
    {
        if (number.Length == 0) return number;

        number = number.ToLower().Replace(" ", "");

        string denomination = number.Substring(number.LastIndexOf(number.Last(char.IsDigit)) + 1);
        number = number.Substring(0, number.LastIndexOf(number.Last(char.IsDigit)) + 1);

        if (!double.TryParse(number, CultureInfo.InvariantCulture, out double inputDouble)) return number;

        return denomination switch
        {
            "h" => Calculator.Calculate($"{number}*10^2").Result.ToString(CultureInfo.InvariantCulture),
            "k" => Calculator.Calculate($"{number}*10^3").Result.ToString(CultureInfo.InvariantCulture),
            "m" => Calculator.Calculate($"{number}*10^6").Result.ToString(CultureInfo.InvariantCulture),
            "b" => Calculator.Calculate($"{number}*10^9").Result.ToString(CultureInfo.InvariantCulture),
            "t" => Calculator.Calculate($"{number}*10^12").Result.ToString(CultureInfo.InvariantCulture),
            "q" or "qa" => Calculator.Calculate($"{number}*10^15").Result.ToString(CultureInfo.InvariantCulture),
            "p" => Calculator.Calculate($"{number}*10^18").Result.ToString(CultureInfo.InvariantCulture),
            _ => number
        };
    }

    public static string? ExpandFtIn(string number)
    {
        if (number.Length == 0) return number;

        //Check for unitless case first (6 3)
        number = number.Trim();
        string?[] numberSplit = ["", "", "", ""];
        string[] numberShortSplit = number.Contains(' ')
            ? number.Split(' ')
            : ["0", "ft", number, "in"];
        if ((int.TryParse(numberShortSplit[0], out int firstInt) &&
             int.TryParse(numberShortSplit[1], out int secondInt)))
        {
            numberSplit[0] = firstInt.ToString();
            numberSplit[1] = "ft";
            numberSplit[2] = secondInt.ToString();
            numberSplit[3] = "in";
        }
        else //Carry on checking unit cases (6 feet 3 inches, 6' 3", etc.)
        {
            number = number.ToLower().Replace(" ", "").Replace("feet", "ft").Replace("foot", "ft")
                .Replace("inches", "in").Replace("inch", "in");
            if (char.IsDigit(number.Last())) number += "in";

            UnitParser unitParser = UnitParser.CreateDefault();

            string footRegex = "(foot)";
            foreach (string abbr in unitParser.Abbreviations.GetUnitAbbreviations(LengthUnit.Foot))
                footRegex += $"|({abbr})";

            string inchRegex = "(inch)";
            foreach (string abbr in unitParser.Abbreviations.GetUnitAbbreviations(LengthUnit.Inch))
                inchRegex += $"|({abbr})";

            var match = Regex.Match(number, footRegex);
            if (match.Success)
            {
                numberSplit[0] = number.Substring(0, number.Length - number[match.Index..].Length);
                numberSplit[1] = number.Substring(match.Index, match.Length);
            }

            match = Regex.Match(number, inchRegex);
            if (!match.Success) return number;

            numberSplit[2] = number.Substring(numberSplit[0].Length + numberSplit[1].Length,
                number.Length - numberSplit[0].Length - numberSplit[1].Length - match.Length);
            numberSplit[3] = number.Substring(match.Index);
        }

        numberSplit[0] = Expand(numberSplit[0]);
        numberSplit[2] = Expand(numberSplit[2]);

        if (!double.TryParse(numberSplit[0], out double outputFoot)) outputFoot = 0; //No feet specified
        return !double.TryParse(numberSplit[2], out double outputInch)
            ? number
            : (outputFoot * 12 + outputInch).ToString(CultureInfo.InvariantCulture);
    }
}