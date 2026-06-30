using Acorn.Records;
using Tomlyn.Serialization;

namespace Acorn.Classes;

public class QuotesQueue
{
    public List<int>? Ids { get; }
    public int Index { get; }

    public int Count() => Ids?.Count ?? 0;

    [TomlConstructor]
    public QuotesQueue(int[] ids, int index)
    {
        Ids = ids.ToList();
        Index = index;
    }

    public QuotesQueue(List<Quote> quotes, int index)
    {
        Ids = [];
        foreach (var quote in quotes) Ids.Add(quote.Id);
        Index = index;
    }
}