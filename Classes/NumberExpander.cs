using System.Globalization;
using Dangl.Calculator;

namespace Acorn.Classes;

public abstract class NumberExpander
{
    public static string? Expand(string number)
    {
        number = number.ToLower().Trim();

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
}