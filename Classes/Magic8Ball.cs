using Acorn.Records;

namespace Acorn.Classes
{
    public class Magic8Ball
    {
        private readonly List<Outlook> Outlooks = new(20);
        private readonly List<Outlook> NotZeroOutlooks = new(15);
        private List<int> lastZeroes = new(5);

        public Magic8Ball()
        {
            Outlooks =
            [
                new Outlook("It is certain.", false),
                new Outlook("It is decidedly so.", false),
                new Outlook("Without a doubt.", false),
                new Outlook("Yes, definitely.", false),
                new Outlook("You may rely on it.", false),
                new Outlook("As I see it, yes.", false),
                new Outlook("Most likely.", false),
                new Outlook("The outlook is good.", false),
                new Outlook("Yes.", false),
                new Outlook("Signs point to yes.", false),
                new Outlook("The reply is hazy – try again.", true),
                new Outlook("Ask again later.", true),
                new Outlook("Better not tell you now.", true),
                new Outlook("I cannot predict now.", true),
                new Outlook("Please concentrate and ask again.", true),
                new Outlook("Don't count on it.", false),
                new Outlook("My reply is no.", false),
                new Outlook("My sources say no.", false),
                new Outlook("The outlook is not so good.", false),
                new Outlook("Very doubtful.", false)
            ];
            NotZeroOutlooks = [.. Outlooks.Where(a => a.IsNeutral == false)];
            lastZeroes = new(5);
        }

        public string ReturnFortune(ulong userId, string input)
        {
            int seed = $"{userId}{input}".GetHashCode(); //Include the user ID so that different people get different answers
            Random random = new(seed);

            if (lastZeroes.Contains(seed)) return NotZeroOutlooks[random.Next(NotZeroOutlooks.Count)].Text;

            lastZeroes = [.. lastZeroes.TakeLast(4)];
            lastZeroes.Add(seed);
            return Outlooks[random.Next(Outlooks.Count)].Text;
        }
    }
}
