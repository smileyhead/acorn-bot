using Acorn.Records;
using DSharpPlus;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tesseract;

namespace Acorn.Classes
{
    internal class QuotesList
    {
        private static List<Quote> Quotes;
        private static int ShuffledIndex;
        private static string QuotesPath;
        private static ulong? LastQuoter;
        public List<User>? Users;

        public QuotesList(DiscordClient client, string quotesPath)
        {
            QuotesPath = quotesPath;

            int error = 0;
            int invalidUserId = 0;
            int erroredUserId = 0;
            DiscordUser discordUser = null;

            Console.WriteLine($"  Reading from ‘{quotesPath}’.");
            using (FileStream readQuotes = File.OpenRead(quotesPath))
            {
                Quotes = JsonSerializer.Deserialize<List<Quote>>(readQuotes);
            }

            Console.Write("  Getting the number of unique users. ");
            Users = new List<User>(Quotes.Select(x => x.UserId).Distinct().Count());
            ulong[] distinctUserIds = Quotes.Select(x => x.UserId).Distinct().ToArray();
            for (int i = 0; i < distinctUserIds.Length; i++) { Users.Add(new User { Id = distinctUserIds[i] }); }
            Console.WriteLine($"Result: {Users.Count}");

            Console.Write("  Querying the API for global nicknames. ");
            for (int i = 0; i < Users.Count; i++)
            {
                try
                {
                    discordUser = client.GetUserAsync(Users[i].Id).Result;
                }
                catch (Exception e)
                {
                    if (e is BadRequestException) { error = 1; }
                    if (e is ServerErrorException) { error = 2; }
                }
                if (error != 0) { Users[i].Name = "Someone"; }
                else if (discordUser.GlobalName is null) Users[i].Name = discordUser.Username; //Fall back to username if global nickname isn't present
                else Users[i].Name = discordUser.GlobalName;
            }

            DiscordChannel debugChannel = client.GetChannelAsync(1337097452859428877).Result;
            Console.WriteLine($"Error?: {error}.");
            if (error == 1) { debugChannel.SendMessageAsync($"While caching a user ID, one or more of them turned out to be invalid. Index: {invalidUserId}"); }
            if (error == 2) { debugChannel.SendMessageAsync($"The Discord API has encountered an error, so one or more global nicknames have been set to Someone. Index: {erroredUserId}"); }

            Console.WriteLine("  Filling up quotes list with queried names.");
            for (int i = 0; i < Users.Count; i++)
            {
                for (int j = 0; j < Quotes.Count; j++)
                {
                    if (distinctUserIds[i] == Quotes[j].UserId) Quotes[j].Username = Users[i].Name;
                }
            }

            Console.WriteLine("  Filling up quote counters.");
            CountQuotes();

            Console.WriteLine("  Shuffling quotes.");
            Shuffle();
        }

        public int GetShuffledIndex() { return ShuffledIndex; }
        public int GetQuotesNo() { return Quotes.Count; }
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

        public void CountQuotes()
        {
            for (int i = 0; i < Users.Count; i++) { Users[i].QuoteCount = 0; }

            for (int i = 0; i < Quotes.Count; i++)
            {
                for (int j = 0; j < Users.Count; j++)
                {
                    if (Quotes[i].UserId == Users[j].Id) { Users[j].QuoteCount++; break; }
                }
            }
        }

        private void CreateBackup()
        {
            Console.WriteLine("  Backing up quotes.");
            File.Move(QuotesPath, $"{Program.backupsPath}{QuotesPath.Insert(QuotesPath.IndexOf('.'), $"-backup_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}")}");
            //quotes.json -> /backups/quotes-backup_yyyy-MM-dd-HH-mm-ss.json
        }

        public (string message, string secondHalf, string alttext) Add(MessageCommandContext context, DiscordMessage message)
        {
            if (Quotes.Any(a => a.Link.Substring(a.Link.LastIndexOf('/') + 1) == message.Id.ToString()))
            {
                Console.WriteLine("  Error: Quote exists.");
                return ("This quote already exists!", "", "");
            }
            else if (message.Author.Id == 1335008063589191721)
            {
                Console.WriteLine("  Error: Quoting self.");
                return ("I'm sorry, but I can't quote myself – I don't want to sound arrogant!", "", "");
            }
            else if (message.Stickers.Count > 0)
            {
                Console.WriteLine("  Error: Message has sticker.");
                return ("I'm sorry, but due to Discord's limitations, I can't quote a sticker.\nIf you still wish to quote this message, consider taking a screenshot of it.", "", "");
            }
            else
            {
                Random random = new();
                DiscordUser? author = message.Author;
                IReadOnlyList<DiscordAttachment> attachments = message.Attachments;

                if (!File.Exists(QuotesPath)) RestoreBackup();
                CreateBackup();

                List<Quote> quotesUnshuffled = Quotes.OrderBy(o => o.Id).ToList();
                Quote newQuote = new();
                newQuote.Id = Quotes.Count;
                newQuote.Body = message.Content;
                newQuote.AltText = null;
                float[] confidences = new float[attachments.Count];
                bool transcriptionFailure = false;
                newQuote.UserId = author.Id;
                if (author.GlobalName is null) newQuote.Username = author.Username; //Fall back to username if global nickname isn't present
                else newQuote.Username = author.GlobalName;
                newQuote.Link = message.JumpLink.ToString();
                if (attachments.Count > 0)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        newQuote.Body += $"\n{attachments[i].Url}";
                        string transcriptionResult;
                        try
                        {
                            (transcriptionResult, confidences[i]) = TranscribeImage(attachments[i].Url).Result;
                        }
                        catch (Exception ex)
                        {
                            transcriptionFailure = true;
                            transcriptionResult = "";
                            confidences[i] = 0;
                        }
                        if (i == 0) newQuote.AltText = transcriptionResult;
                        else newQuote.AltText += $"\n{transcriptionResult}";
                    }
                }
                if (newQuote.Body[0] == '\n') { newQuote.Body = newQuote.Body.Substring(1); }
                quotesUnshuffled.Add(newQuote);
                Quotes.Insert(random.Next(ShuffledIndex, Quotes.Count), newQuote);
                WriteToFile(quotesUnshuffled);

                string answer = "Thank you for your ";
                if (random.NextDouble() >= 0.5) { answer += "contribution"; }
                else answer += "sacrifice";

                LastQuoter = context.User.Id;

                answer += ". Adding quote:\n\n";

                (DiscordMessageBuilder printedMessage, string secondHalf) = Print((quotesUnshuffled.Count - 1).ToString(), false, answer);

                string alttext = "";
                if (attachments.Count > 0)
                {
                    string alttextPrependage = $"-# Added alt text to quote `#{quotesUnshuffled.Count - 1}`:";
                    alttext += BlockQuoteify(newQuote.AltText, "> -#");

                    string alttextAppendage = "\n-# ";
                    if (attachments.Count > 1)
                    {
                        alttextAppendage += $"{confidences[0] * 100}%";
                        for (int i = 1; i < attachments.Count; i++)
                        {
                            alttextAppendage += $", {confidences[i] * 100}%";
                        }
                        alttextAppendage += " confidence, respectfully";
                    }
                    else alttextAppendage += $"{confidences[0] * 100}% confidence";
                    alttextAppendage += $" – use `.alttext {quotesUnshuffled.Count - 1} Your text here` to overwrite.";

                    if (alttextPrependage.Length + alttext.Length + alttextAppendage.Length > 2000)
                        alttext = Shorten(alttext, alttextPrependage, $"…\n-# (Not all text can be displayed. Character limit reached.){alttextAppendage}");
                    else alttext = alttextPrependage + alttext + alttextAppendage;
                }
                if (transcriptionFailure) alttext = $"-# Failed to transcribe an image. Use `.alttext {quotesUnshuffled.Count - 1} Your text here` to add manually.";

                    return (printedMessage.Content, secondHalf, alttext);
            }
        }

        private void WriteToFile(List<Quote> quote)
        {
            using FileStream writeQuotes = File.Create(QuotesPath);
            var options = new JsonSerializerOptions { WriteIndented = true };
            JsonSerializer.Serialize(writeQuotes, quote, options);
        }

        public string Undo(MessageCommandContext context)
        {
            if (LastQuoter == null)
            {
                Console.WriteLine("  Last quoter is null.");
                return "I'm sorry, but only the most recently added quote may be undone.";
            }
            else if (context.User.Id != LastQuoter)
            {
                Console.WriteLine("  Command user / Last quoter mismatch.");
                return "I'm sorry, but only the person who added the last quote may undo it.";
            }
            else
            {
                RestoreBackup();
                LastQuoter = null; //Prevent any further undo operations.
                return $"Quoting undone. The most recent quote is now `#{Quotes.Count - 1}`.";
            }
        }

        public string OverwriteAlttext(string quoteIdInput, string text)
        {
            int quoteId;
            if (quoteIdInput[0] == '#')
            {
                if (!Int32.TryParse(quoteIdInput.Substring(1), out quoteId)) return "Sorry, but the quote ID could not be parsed.";
            }
            else if (!Int32.TryParse(quoteIdInput, out quoteId)) return "Sorry, but the quote ID could not be parsed.";

            Quotes[Quotes.IndexOf(Quotes.Where(a => a.Id == quoteId).FirstOrDefault())].AltText = text;
            CreateBackup();
            List<Quote> quotesUnshuffled = Quotes.OrderBy(o => o.Id).ToList();
            WriteToFile(quotesUnshuffled);

            return "Alt text overwritten.";
        }

        private void RestoreBackup()
        {
            Console.WriteLine("  Restoring backup.");
            var directory = new DirectoryInfo(Program.backupsPath);
            var latestBackup = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
            File.Move($"{Program.backupsPath}{latestBackup.Name}", QuotesPath, true);

            Quotes.Remove(Quotes.OrderByDescending(q => q.Id).First());
        }

        public (DiscordMessageBuilder message, string secondHalf) Print(string id_input, bool isShuffled, string prefix)
        {
            string PurgeBeginning(string message) { if (message[0] == '\n' || message[0] == ' ') return message.Substring(1); return message; }
            string PurgeEnd(string message) { if (message[message.Length - 1] == '\n') return message.Remove(message.Length - 2); return message; }

            Console.WriteLine("  Printing a quote.");
            string messageContent = prefix;
            string secondHalf = "";
            int id = 0;

            if (isShuffled) id = ShuffledIndex;
            else
            {
                if (id_input[0] == '#') { id_input = id_input.Remove(0, 1); }

                if (id_input.ToLower() == "latest") { id = Quotes.Count - 1; }
                else if (!int.TryParse(id_input, out id)) { return (new DiscordMessageBuilder().WithContent("Error: Invalid format. For help, see: `/help searchquote`."), ""); }
                else if (id < 0 || id > Quotes.Count() - 1) { return (new DiscordMessageBuilder().WithContent("Error: The specified number falls outside the accepted range. For help, see: `/help searchquote`."), ""); }
            }

            List<Quote> quotesList = Quotes;
            if (!isShuffled) { quotesList = Quotes.OrderBy(o => o.Id).ToList(); Console.WriteLine("    Unshuffled quotes list created."); }

            messageContent += $"`#{quotesList[id].Id}` **{quotesList[id].Username}** [said]({quotesList[id].Link}):";
            messageContent += BlockQuoteify(quotesList[id].Body, ">");

            if (isShuffled) ShuffledIndex++;

            if (messageContent.Length > 2000)
            {
                var regex = Regex.Match(messageContent.Substring(messageContent.Length - messageContent.Length / 3), "([\n.?!;, ])");

                if (regex.Success)
                {
                    int splitIndex = messageContent.Length - messageContent.Length / 3 + regex.Index + 1;

                    if (messageContent[splitIndex] != '\n') secondHalf += "> ";
                    secondHalf += $"{PurgeBeginning(messageContent.Substring(splitIndex + 1))}";
                    messageContent = PurgeEnd(messageContent.Remove(splitIndex));
                }
                else
                {
                    int splitIndex = messageContent.Length - messageContent.Length / 3;
                    secondHalf = $"> {PurgeBeginning(messageContent.Substring(splitIndex + 1))}";
                    messageContent = PurgeEnd(messageContent.Remove(splitIndex));
                }

                secondHalf += "\n-# This message exceeded Discord's limit of 2,000 characters, so it has been split in two.";
            }

            return (new DiscordMessageBuilder().WithContent(messageContent), secondHalf);
        }

        public (DiscordMessageBuilder message, string secondHalf) QuoteBy(string authorId)
        {
            if (!ulong.TryParse(authorId, out ulong id))
            {
                return (new DiscordMessageBuilder().WithContent("Error: The author ID given could not be parsed. Did you choose from the list?"), "");
            }

            Random random = new Random();
            List<Quote> userQuotes = Quotes.FindAll(i => i.UserId == id);

            return Print(userQuotes[random.Next(0, userQuotes.Count - 1)].Id.ToString(), false, "");
        }

        public (DiscordMessageBuilder message, string secondHalf) Search(string query)
        {
            if (query.Length < 3)
            {
                return (new DiscordMessageBuilder().WithContent("Your query must be at least 3 characters long."), "");
            }
            else
            {
                string error = "";
                string answer = "";
                string quote = "";
                string username = "";
                List<Quote> quotesUnshuffled = Quotes.OrderBy(o => o.Id).ToList();
                List<SearchResult> results = new();

                for (int i = 0; i < quotesUnshuffled.Count; i++)
                {
                    if (quotesUnshuffled[i].Body.Contains(query, StringComparison.OrdinalIgnoreCase)) results.Add(new SearchResult(quotesUnshuffled[i], false));
                    if (quotesUnshuffled[i].AltText != null && quotesUnshuffled[i].AltText.Contains(query, StringComparison.OrdinalIgnoreCase)) results.Add(new SearchResult(quotesUnshuffled[i], true));
                }

                if (results.Count == 0) { return (new DiscordMessageBuilder().WithContent($"There are no results for ‘{query}’."), ""); }
                else if (results.Count == 1)
                {
                    (DiscordMessageBuilder message, string secondHalf) = Print(results[0].Quote.Id.ToString(), false, $"There is 1 result for ‘{query}’:\n\n");

                    return (message, secondHalf);
                }
                else
                {
                    answer += $"There are {results.Count} results for ‘{query}’:\n\n";

                    for (int i = 0; i < results.Count; i++)
                    {
                        answer += $"`#{results[i].Quote.Id}` **{results[i].Quote.Username}:** ";
                        if (results[i].IsAltText) quote = results[i].Quote.AltText;
                        else quote = results[i].Quote.Body;

                        if (quote.Length - quote.IndexOf(query) > 50)
                        {
                            if (quote.IndexOf(query) > 25)
                            {
                                quote = $"…{quote.Substring(quote.IndexOf(query) - 10)}";
                            }

                            if (quote.Length > 50) { quote = $"{quote.Substring(0, 50)}…"; }
                        }
                        else if (quote.Length > 50) quote = $"…{quote.Substring(quote.Length - 50)}";

                        if (results[i].IsAltText) answer += $"🖼️ ‘{FlushFormatting(quote)}’\n";
                        else answer += $"‘{FlushFormatting(quote)}’\n";

                        if (answer.Length > 2000)
                        {
                            return (new DiscordMessageBuilder().WithContent(Shorten(answer, "", "\n-# Not all results can be displayed. Character limit reached.")), "");
                        }
                    }

                    return (new DiscordMessageBuilder().WithContent(answer), "");
                }
            }
        }

        private string FlushFormatting(string input)
        {
            string pattern = "([*_~#`]|http)";
            string replacement = "\\$1";
            Regex regex = new Regex(pattern);

            input = regex.Replace(input, replacement);

            pattern = "(\n)";
            replacement = " ";
            regex = new Regex(pattern);

            return regex.Replace(input, replacement);
        }

        private string Shorten(string input, string prepend, string append)
        {
            while (prepend.Length + input.Length + append.Length > 2000)
            {
                input = input.Substring(0, input.LastIndexOf('\n'));
            }

            string output = prepend + input + append;

            return output;
        }

        private string BlockQuoteify(string input, string prepend)
        {
            string[] bodySplit = input.Split('\n');
            string output = "";
            for (int i = 0; i < bodySplit.Count(); i++)
            {
                output += $"\n{prepend} {bodySplit[i]}";
            }

            return output;
        }

        public async Task<(string text, float confidence)> TranscribeImage(string url)
        {
            HttpClient client = new HttpClient();
            byte[] image = await client.GetByteArrayAsync(url);

            using (TesseractEngine engine = new(@"./tessdata", "eng", EngineMode.Default))
            {
                using (Pix img = Pix.LoadFromMemory(image))
                {
                    using (Page page = engine.Process(img))
                    {
                        string txt = page.GetText();

                        //Cleaning up
                        txt = txt.Replace("|", "I");
                        for (int i = 0; i < 5; i++)
                        {
                            if (!txt.Contains("\n\n")) break;
                            txt = txt.Replace("\n\n", "\n");
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            if (!txt.Contains("\n \n")) break;
                            txt = txt.Replace("\n \n", "\n");
                        }
                        while (txt[txt.Length - 1] == '\n') txt = txt.Remove(txt.Length - 1);

                        return (txt, page.GetMeanConfidence());
                    }
                }
            }
        }
    }
}
