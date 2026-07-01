namespace Acorn.Classes;

public abstract class NumberExpander
{
    public static string Expand(string number) => number.ToLower()
        .Replace("h",  "00")
        .Replace("k",  "000")
        .Replace("m",  "000000")
        .Replace("b",  "000000000")
        .Replace("t",  "000000000000")
        .Replace("q",  "000000000000000")
        .Replace("qa", "000000000000000000")
        .Replace("p",  "000000000000000000");
}