using DSharpPlus;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using System;
using System.Text.Json;

namespace Acorn
{
    internal class QuotesList
    {
        private static List<Quote> Quotes;
        private static int ShuffledIndex;
        private static string QuotesPath;

        public QuotesList(DiscordClient client, string quotesPath)
        {
            QuotesPath = quotesPath;

            int usersI = 0;
            int error = 0;
            int invalidUserId = 0;
            int erroredUserId = 0;
            DiscordUser discordUser = null;

            Console.WriteLine($"  Reading from ‘{quotesPath}’.");
            using (FileStream readQuotes = File.OpenRead(quotesPath))
            {
                Quotes = JsonSerializer.Deserialize<List<Quote>>(readQuotes);
            }

            Console.Write("  Getting number of unique user IDs in quotes list. ");
            usersI = Quotes.Select(x => x.UserId).Distinct().Count();
            ulong[] distinctUserIds = new ulong[usersI];
            distinctUserIds = Quotes.Select(x => x.UserId).Distinct().ToArray();
            string[] distinctUsernames = new string[usersI];
            Console.WriteLine($"Result: {usersI}");

            Console.WriteLine("  Filling up the users list.");
            List<User> user = new List<User>();

            Console.Write("  Querying the API for global nicknames. ");
            for (int i = 0; i < usersI; i++)
            {
                try
                {
                    discordUser = client.GetUserAsync(distinctUserIds[i]).Result;
                }
                catch (Exception e)
                {
                    if (e is BadRequestException) { error = 1; }
                    if (e is ServerErrorException) { error = 2; }
                }
                if (error != 0) { distinctUsernames[i] = "Someone"; }
                else { distinctUsernames[i] = discordUser.GlobalName; }
            }

            DiscordChannel debugChannel = client.GetChannelAsync(1337097452859428877).Result;
            Console.WriteLine($"Error?: {error}.");
            if (error == 1) { debugChannel.SendMessageAsync($"While caching a user ID, one or more of them turned out to be invalid. Index: {invalidUserId}"); }
            if (error == 2) { debugChannel.SendMessageAsync($"The Discord API has encountered an error, so one or more global nicknames have been set to Someone. Index: {erroredUserId}"); }

            Console.WriteLine("  Filling up quotes lists with queried names.");
            for (int i = 0; i < distinctUsernames.Count(); i++)
            {
                for (int j = 0; j < Quotes.Count; j++)
                {
                    if (distinctUserIds[i] == Quotes[j].UserId)
                    {
                        Quotes[j].Username = distinctUsernames[i];
                    }
                }
            }

            Shuffle();
        }

        public int GetShuffledIndex() { return ShuffledIndex; }
        //I know this isn't conventional in C#. I also don't care.

        private void Shuffle()
        {
            Console.Write("  Shuffling the shuffled quotes list. ");
            Random random = new Random();
            for (int i = 0; i < Quotes.Count - 1; i++)
            {
                int r = random.Next(i, Quotes.Count);
                (Quotes[r], Quotes[i]) = (Quotes[i], Quotes[r]);
            }
            ShuffledIndex = 0;
            Console.WriteLine($"Shuffled list index set to {ShuffledIndex}");
        }

        public void Reshuffle()
        {
            if (ShuffledIndex >= Quotes.Count)
            {
                Console.WriteLine("The shuffled quotes list has reached its end. Reshuffling.");
                Shuffle();
            }
        }

        private void CreateBackup()
        {
            Console.WriteLine("  Backing up quotes.");
            File.Move(QuotesPath, "backups/" + QuotesPath.Insert(QuotesPath.IndexOf('.'), $"-backup_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}"));
            //quotes.json -> /backups/quotes-backup_yyyy-MM-dd-HH-mm-ss.json
        }

        public string Add(MessageCommandContext context, DiscordMessage message)
        {
            if (Quotes.Any(a => a.Link.Substring(a.Link.LastIndexOf('/') + 1) == message.Id.ToString()))
            {
                Console.WriteLine("  Error: Quote exists.");
                return "This quote already exists!";
            }
            else
            {
                Random random = new();
                DiscordUser? author = message.Author;
                IReadOnlyList<DiscordAttachment> attachments = message.Attachments;

                CreateBackup();

                List<Quote> quotesUnshuffled = Quotes.OrderBy(o => o.Id).ToList();
                Quote newQuote = new();
                newQuote.Id = Quotes.Count;
                newQuote.Body = message.Content;
                newQuote.UserId = author.Id;
                newQuote.Username = author.GlobalName;
                newQuote.Link = message.JumpLink.ToString();
                if (attachments.Count > 0)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        newQuote.Body += $"\n{attachments[i].Url}";
                    }
                }
                if (newQuote.Body[0] == '\n') { newQuote.Body = newQuote.Body.Substring(1); }
                quotesUnshuffled.Add(newQuote);
                Quotes.Insert(random.Next(ShuffledIndex, Quotes.Count), newQuote);
                WriteToFile(quotesUnshuffled);

                string answer = "Thank you for your ";
                if (random.NextDouble() >= 0.5) { answer += "contribution"; }
                else answer += "sacrifice";

                answer += ". Adding quote:\n\n";
                return $"{answer}{Print((quotesUnshuffled.Count - 1).ToString(), false)}";
            }
        }

        private void WriteToFile(List<Quote> quote)
        {
            using FileStream writeQuotes = File.Create(QuotesPath);
            JsonSerializer.Serialize(writeQuotes, quote);
        }

        public string Print(string id_input, bool isShuffled)
        {
            Console.WriteLine("  Printing a quote.");
            string answer = "";
            int id = 0;

            if (isShuffled) id = ShuffledIndex;
            else
            {
                if (id_input[0] == '#') { id_input = id_input.Remove(0, 1); }

                if (id_input.ToLower() == "latest") { id = Quotes.Count - 1; }
                else if (!Int32.TryParse(id_input, out id)) { answer = "Error: Invalid format. For help, see: `/help sq`."; }
                else if (id < 0 || id > Quotes.Count() - 1) { answer = "Error: The specified number falls outside the accepted range. For help, see: `/help sq`."; }
            }

            List<Quote> quotesList = Quotes;
            if (!isShuffled) { quotesList = Quotes.OrderBy(o => o.Id).ToList(); Console.WriteLine("    Unshuffled quotes list created."); }

            answer += $"`#{quotesList[id].Id}` **{quotesList[id].Username}** [said]({quotesList[id].Link}):";
            string[] bodySplit = quotesList[id].Body.Split('\n');
            for (int i = 0; i < bodySplit.Count(); i++)
            {
                answer += $"\n> {bodySplit[i]}";
            }

            if (isShuffled) ShuffledIndex++;
            return answer;
        }

        public string Search(string query)
        {
            if (query.Length < 3)
            {
                return "Your query must be at least 3 characters long.";
            }
            else
            {
                string error = "";
                string answer = "";
                string username = "";
                List<Quote> quotesUnshuffled = Quotes.OrderBy(o => o.Id).ToList();

                int[] results = new int[quotesUnshuffled.Count];
                int resultsI = 0;

                for (int i = 0; i < quotesUnshuffled.Count; i++)
                {
                    if (quotesUnshuffled[i].Body.Contains(query, StringComparison.OrdinalIgnoreCase))
                    {
                        results[resultsI] = i;
                        resultsI++;
                    }
                }

                if (resultsI == 0) { return $"There are no results for ‘{query}’."; }
                else
                {
                    if (resultsI == 1) { answer += $"There is 1 result for ‘{query}’:\n\n"; }
                    else { answer += $"There are {resultsI} results for ‘{query}’:\n\n"; }

                    for (int i = 0; i < resultsI; i++)
                    {
                        answer += $"`#{quotesUnshuffled[results[i]].Id}` **{quotesUnshuffled[results[i]].Username}:** ";

                        if (quotesUnshuffled[results[i]].Body.Length <= 51) { answer += $"‘{quotesUnshuffled[results[i]].Body}’\n"; }
                        else { answer += $"‘{quotesUnshuffled[results[i]].Body.Substring(0, 50)}…’\n"; }
                    }

                    return answer;
                }
            }
        }
    }
}
