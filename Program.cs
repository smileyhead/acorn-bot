using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Linq;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System;
using UnitsNet;

namespace Acorn
{
    public class HelpArticles
    {
        public string? Command { get; set; }
        public string? HelpText { get; set; }
    }

    public class Quotes
    {
        public int Id { get; set; }
        public string? Body { get; set; }
        public ulong UserId { get; set; }
        public string? Username { get; set; }
        public string? Link { get; set; }
    }

    public class Users
    {
        public ulong Id { get; set; }
        public required string Name { get; set; }
    }

    class Program
    {
        static string discordTokenPath = "tock.txt";
        static string quotesPath = "quotes.json";
        static string discordToken = File.ReadLines(discordTokenPath).First();
        static DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
        static DiscordClient debugClient = builder.Build();
        static DiscordChannel debugChannel;
        static List<Quotes> quotesShuffled = new List<Quotes>();
        static int quotesShuffledI = 0;

        async static void PrintDebugMessage(string message)
        {
            await debugChannel.SendMessageAsync(message);
        }

        static string PrintQuote(int id, bool isRandom)
        {
            Console.WriteLine("  Printing a quote.");

            string answer = "";

            List<Quotes> quotesList = quotesShuffled;
            if (!isRandom) { quotesList = quotesShuffled.OrderBy(o => o.Id).ToList(); Console.WriteLine("    Unshuffled quotes list created."); }

            answer += $"`#{quotesList[id].Id}` **{quotesList[id].Username}** [said]({quotesList[id].Link}):";
            string[] bodySplit = quotesList[id].Body.Split('\n');
            for (int i = 0; i < bodySplit.Count(); i++)
            {
                answer += $"\n> {bodySplit[i]}";
            }

            return answer;
        }

        static void CreateBackup()
        {
            Console.WriteLine("  Backing up quotes.");
            File.Move(quotesPath, "backups/" + quotesPath.Insert(quotesPath.IndexOf('.'), $"-backup_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}"));
            //quotes.json -> /backups/quotes-backup_yyyy-MM-dd-HH-mm-ss.json
        }

        static void ShuffleQuotes()
        {
            Console.Write("  Shuffling the shuffled quotes list. ");
            Random random = new Random();
            for (int i = 0; i < quotesShuffled.Count - 1; i++)
            {
                int r = random.Next(i, quotesShuffled.Count);
                (quotesShuffled[r], quotesShuffled[i]) = (quotesShuffled[i], quotesShuffled[r]);
            }
            quotesShuffledI = 0;
            Console.WriteLine($"Shuffled list index set to {quotesShuffledI}");
        }

        static void ReadQuotes(DiscordClient client)
        {
            int usersI = 0;
            int error = 0;
            int invalidUserId = 0;
            int erroredUserId = 0;
            DiscordUser discordUser = null;

            Console.WriteLine($"  Reading from ‘{quotesPath}’.");
            using (FileStream readQuotes = File.OpenRead(quotesPath))
            {
                quotesShuffled = JsonSerializer.Deserialize<List<Quotes>>(readQuotes);
            }

            Console.Write("  Getting number of unique user IDs in quotes list. ");
            usersI = quotesShuffled.Select(x => x.UserId).Distinct().Count();
            ulong[] distinctUserIds = new ulong[usersI];
            distinctUserIds = quotesShuffled.Select(x => x.UserId).Distinct().ToArray();
            string[] distinctUsernames = new string[usersI];
            Console.WriteLine($"Result: {usersI}");

            Console.WriteLine("  Filling up the users list.");
            List<Users> user = new List<Users>();

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
            
            Console.WriteLine($"Error?: {error}.");
            if (error == 1) { PrintDebugMessage($"While caching a user ID, one or more of them turned out to be invalid. Index: {invalidUserId}"); }
            if (error == 2) { PrintDebugMessage($"The Discord API has encountered an error, so one or more global nicknames have been set to Someone. Index: {erroredUserId}"); }

            Console.WriteLine("  Filling up quotes lists with queried names.");
            for (int i = 0; i < distinctUsernames.Count(); i++)
            {
                for (int j = 0; j < quotesShuffled.Count; j++)
                {
                    if (distinctUserIds[i] == quotesShuffled[j].UserId)
                    {
                        quotesShuffled[j].Username = distinctUsernames[i];
                    }
                }
            }

            ShuffleQuotes();
        }

        static void WriteToQuotes(List<Quotes> quote)
        {
            using FileStream writeQuotes = File.Create(quotesPath);
            JsonSerializer.Serialize(writeQuotes, quote);
        }

        static (string dice, char modifierType, int modifier, string answer, bool alreadyAnswered) CheckModifier(string dice)
        {
            Console.WriteLine("  Checking dice modifiers.");

            string answer = "";
            bool alreadyAnswered = false;
            char modifierType = '\0';
            int modifier = 0;

            if (dice.Contains('+')) { modifierType = '+'; }
            else if (dice.Contains('-')) { modifierType = '-'; }

            if (modifierType == '+' || modifierType == '-')
            {
                if (!Int32.TryParse(dice.Substring(dice.IndexOf(modifierType) + 1), out modifier))
                {
                    answer = "Error: Incorrect format. For help, see `/help r`.";
                    alreadyAnswered = true;
                }
                dice = dice.Remove(dice.IndexOf(modifierType));
            }

            return (dice, modifierType, modifier, answer, alreadyAnswered);
        }

        static string FormatNumber(int number)
        {
            return number.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
        }

        static string RollDice(int diceN, int sidesN, char modifierType, int modifier, bool hasReroll)
        {
            Console.WriteLine("  Rolling dice.");

            string answer = "";
            Random random = new Random();
            int[] rolls = new int[diceN];
            string separator = ", ";
            string[] numberNames = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"];
            bool emergencyStopActivated = false;
            bool emergencyStopActivatedThisRound = false;

            if (diceN == 1) { answer += $"You roll a {sidesN}-sided die…\n\n"; }
            else { answer += $"You roll {numberNames[diceN - 1]} {sidesN}-sided dice…\n\n"; }

            for (int i = 0; i < diceN; i++)
            {
                rolls[i] = random.Next(1, sidesN + 1);
                if (i == diceN - 1) { separator = " "; }
                emergencyStopActivatedThisRound = false;

                if (rolls[i] == sidesN) //Critical success
                {
                    answer += $"**{rolls[i]}**{separator}";
                }
                else if (rolls[i] == 1 && hasReroll) //Critical fail with reroll
                {
                    for (int j = 0; j < 10; j++)
                    {
                        rolls[i] = random.Next(1, sidesN + 1);
                        if (rolls[i] > 1) { break; }
                        else if (j == 9)
                        {
                            if (sidesN > 2) { rolls[i] = random.Next(2, sidesN + 1); }
                            else { rolls[i] = 2; }
                            emergencyStopActivatedThisRound = true;
                            emergencyStopActivated = true;
                            break;
                        }
                    }
                    if (emergencyStopActivatedThisRound) { answer += $"__*{rolls[i]}*__{separator}"; }
                    else { answer += $"__{rolls[i]}__{separator}"; }
                }
                else if (rolls[i] == 1 && !hasReroll) //Critical fail
                {
                    answer += $"__{rolls[i]}__{separator}";
                }
                else { answer += $"{rolls[i]}{separator}"; } //Normal roll
            }

            if (modifierType != '\0')
            {
                if (modifierType == '+') { answer += $"{modifierType} {FormatNumber(modifier)} → {FormatNumber(rolls.Sum() + modifier)}"; }
                else if (modifierType == '-') { answer += $"{modifierType} {FormatNumber(modifier)} → {FormatNumber(rolls.Sum() - modifier)}"; }
            }
            else if (diceN > 1) { answer += $"→ {FormatNumber(rolls.Sum())}"; }

            if (emergencyStopActivated) { answer += $"\n-# Note: The rerolling of one or more of these values exceeded 10 tries. For more information, see: `/help r`."; }
            return answer;
        }

        static async Task Main(string[] args)
        {
            var initTime = System.Diagnostics.Stopwatch.StartNew();
            debugChannel = debugClient.GetChannelAsync(1337097452859428877).Result;
            PrintDebugMessage($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Initialising…");

            if (string.IsNullOrEmpty(discordToken))
            {
                PrintDebugMessage("Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
                initTime.Stop();
                Environment.Exit(1);
            }
            

            // Setup the commands extension
            builder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands(
                    [typeof(HelpCommand), typeof(RollDiceCommand), typeof(QuoteCommand), typeof(SpecificQuoteCommand), typeof(CharacterCommand), 
                    typeof(FlipCommand), typeof(ConvertCommand), typeof(SearchQuoteCommand), typeof(AddQuoteMenu)]);
                TextCommandProcessor textCommandProcessor = new(new()
                {
                    // The default behavior is that the bot reacts to direct
                    // mentions and to the "!" prefix. If you want to change
                    // it, you first set if the bot should react to mentions
                    // and then you can provide as many prefixes as you want.
                    PrefixResolver = new DefaultPrefixResolver(false, ".").ResolvePrefixAsync,
                });

                // Add text commands with a custom prefix (?ping)
                extension.AddProcessor(textCommandProcessor);
            }, new CommandsConfiguration()
            {
                // The default value is true, however it's shown here for clarity
                RegisterDefaultCommandProcessors = true,
                //DebugGuildId = Environment.GetEnvironmentVariable("DEBUG_GUILD_ID") ?? 0,
            });

            DiscordClient client = builder.Build();

            Console.WriteLine("Initialising quotes...");
            ReadQuotes(client);
            Console.WriteLine("Quotes loaded.");

            // We can specify a status for our bot. Let's set it to "playing" and set the activity to "with fire".
            DiscordActivity status = new("with fire", DiscordActivityType.Playing);

            // Now we connect and log in.
            await client.ConnectAsync(status, DiscordUserStatus.Online);

            initTime.Stop();
            PrintDebugMessage($"Initialising finished in {initTime.ElapsedMilliseconds}ms.");

            // And now we wait infinitely so that our bot actually stays connected.
            await Task.Delay(-1);
        }

        /*-------------------
        Context Menu Commands
        -------------------*/
        public class AddQuoteMenu
        {
            [Command("Add Quote")]
            [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
            [AllowedProcessors(typeof(MessageCommandProcessor))]
            public async Task AddQuote(MessageCommandContext context, DiscordMessage message)
            {
                var AddQuoteTime = System.Diagnostics.Stopwatch.StartNew();
                Random random = new Random();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Adding quote.");

                if (quotesShuffled.Any(a => a.Link.Substring(a.Link.LastIndexOf('/') + 1) == message.Id.ToString()))
                {
                    await context.RespondAsync("This quote already exists!");
                    Console.WriteLine("  Error: Quote exists.");
                }
                else
                {
                    DiscordUser? author = message.Author;
                    IReadOnlyList<DiscordAttachment> attachments = message.Attachments;

                    CreateBackup();

                    List<Quotes> quotesUnshuffled = quotesShuffled.OrderBy(o => o.Id).ToList();
                    Quotes newQuote = new Quotes();
                    newQuote.Id = quotesUnshuffled.Count;
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
                    quotesShuffled.Insert(random.Next(quotesShuffledI, quotesShuffled.Count), newQuote);
                    WriteToQuotes(quotesUnshuffled);

                    string answer = "Thank you for your ";
                    if (random.NextDouble() >= 0.5) { answer += "contribution"; }
                    else answer += "sacrifice";

                    answer += ". Adding quote:\n\n";
                    await context.RespondAsync($"{answer}{PrintQuote(quotesUnshuffled.Count - 1, false)}");
                }

                AddQuoteTime.Stop();
                Console.WriteLine($"  Quote-adding finished in {AddQuoteTime.ElapsedMilliseconds}ms.");
                if (AddQuoteTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Adding a quote took {AddQuoteTime.ElapsedMilliseconds}ms."); }
            }
        }

        /*------------
        Slash Commands
        ------------*/
        public class HelpCommand
        {
            [Command("help"), Description("Prints the help article for a given command.")]
            public static async ValueTask ExecuteAsync
                (CommandContext context, 
                [Description("The command which you need help with."), SlashAutoCompleteProvider<HelpCommandAutoCompleteProvider>] string command)
            {
                var HelpTime = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Returning a help article.");

                string helpArticlesPath = "help.json";
                string answer = "";

                List<HelpArticles>? helpArticle = null;
                using (FileStream readHelpArticles = File.OpenRead(helpArticlesPath))
                {
                    helpArticle =
                        await JsonSerializer.DeserializeAsync<List<HelpArticles>>(readHelpArticles);
                }

                for (int i = 0; i < helpArticle.Count; i++)
                {
                    if (command == helpArticle[i].Command) { answer = helpArticle[i].HelpText; break; }

                    if (i == helpArticle.Count - 1) { answer = "Command not found."; break; }
                }

                await context.RespondAsync(answer);

                HelpTime.Stop();
                Console.WriteLine($"  Help article-returning finished in {HelpTime.ElapsedMilliseconds}ms.");
                if (HelpTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Returning a help article took {HelpTime.ElapsedMilliseconds}ms."); }
            }
        }

        public class QuoteCommand
        {
            [Command("quote"), Description("Prints a random quote from the collection.")]
            public static async ValueTask ExecuteAsync(CommandContext context)
            {
                var RandomQuoteTime = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Returning a random quote.");

                await context.RespondAsync(PrintQuote(quotesShuffledI, true));
                quotesShuffledI++;

                if (quotesShuffledI >= quotesShuffled.Count) { ShuffleQuotes(); }

                RandomQuoteTime.Stop();
                Console.WriteLine($"  Quote-returning finished in {RandomQuoteTime.ElapsedMilliseconds}ms. Random quote index is now {quotesShuffledI}.");
                if (RandomQuoteTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Returning a random quote took {RandomQuoteTime.ElapsedMilliseconds}ms."); }
            }
        }

        public class SpecificQuoteCommand
        {
            [Command("specificquote"), Description("Prints a specified quote from the collection.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of the quote you wish to recall.")] string quoteId)
            {
                var SpecificQuoteTime = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Returning a specific quote.");
                int id = 0;
                string answer = "";

                if (quoteId[0] == '#') { quoteId = quoteId.Remove(0, 1); }

                if (quoteId.ToLower() == "latest") { id = quotesShuffled.Count - 1; }
                else if (!Int32.TryParse(quoteId, out id)) { answer = "Error: Invalid format. For help, see: `/help sq`."; }
                else if (id < 0 || id > quotesShuffled.Count() - 1) { answer = "Error: The specified number falls outside the accepted range. For help, see: `/help sq`."; }

                if (answer == "") { await context.RespondAsync(PrintQuote(id, false)); }
                else { await context.RespondAsync(answer); }

                SpecificQuoteTime.Stop();
                Console.WriteLine($"  Quote-returning finished in {SpecificQuoteTime.ElapsedMilliseconds}ms.");
                if (SpecificQuoteTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Returning a quote took {SpecificQuoteTime.ElapsedMilliseconds}ms."); }
            }
        }

        public class SearchQuoteCommand
        {
            [Command("searchquote"), Description("Searches for quotes that match the given query.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The search query. At least 3 characters long.")] string query)
            {
                var SearchQuoteTime = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Searching for quotes.");

                if (query.Length < 3)
                {
                    await context.RespondAsync("Your query must be at least 3 characters long.");
                }
                else
                {
                    string error = "";
                    string answer = "";
                    string username = "";
                    List<Quotes> quotesUnshuffled = quotesShuffled.OrderBy(o => o.Id).ToList();

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

                    if (resultsI == 0) { await context.RespondAsync($"There are no results for ‘{query}’."); }
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

                        await context.RespondAsync(answer);
                    }
                }

                SearchQuoteTime.Stop();
                Console.WriteLine($"  Quote-searching finished in {SearchQuoteTime.ElapsedMilliseconds}ms.");
                if (SearchQuoteTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Searching for quotes took {SearchQuoteTime.ElapsedMilliseconds}ms."); }
            }
        }

        public class CharacterCommand
        {
            [Command("character"), Description("Prints a randomly-rolled Dungeons and Dragons character block.")]
            public static async ValueTask ExecuteAsync(CommandContext context)
            {
                var CharacterTime = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Generating a character.");

                int[] RollCharacter(int[] set)
                {
                    Random random = new Random();
                    for (int i = 0; i < 4; i++) { set[i] = random.Next(1, 7); }
                    return set;
                }

                string GetValues(int[] set, string answer)
                {
                    int minI = Array.IndexOf(set, set.Min());
                    string separator = ", ";

                    for (int i = 0; i < set.Length; i++)
                    {
                        if (i == set.Length - 1) { separator = " "; }

                        if (i == minI) { answer += $"{set[i]}{separator}"; }
                        else { answer += $"**{set[i]}**{separator}"; }
                    }

                    answer += $"→ {set.Sum() - set[minI]}\n";
                    return answer;
                }

                int[] set = new int[4];
                int[] sums = new int[6];
                string answer = "";

                for (int i = 0; i < 6; i++)
                {
                    RollCharacter(set);
                    sums[i] = set.Sum() - set[Array.IndexOf(set, set.Min())];
                    answer = GetValues(set, answer);
                }

                answer += $"\nTotal: {sums.Sum()}";
                await context.RespondAsync(answer);

                CharacterTime.Stop();
                Console.WriteLine($"  Character-generating finished in {CharacterTime.ElapsedMilliseconds}ms.");
                if (CharacterTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Generating a character took {CharacterTime.ElapsedMilliseconds}ms."); }
            }
        }

        public class RollDiceCommand
        {
            [Command("roll"), Description("Rolls n x-sided dice. Example: ‘/r 2d4’. Details: ‘/help r’.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of dice to roll and the number of their sides. Example: ‘2d4’.")] string dice)
            {
                var RollTime = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Rolling dice.");

                bool hasReroll = false;
                string[] dicePreSplit = new string[2];
                int[] diceSplit = new int[2];
                dice = dice.ToLower();

                if (dice.Contains("r1")) { hasReroll = true; dice = dice.Remove(dice.IndexOf("r1"), 2); }

                var declare = CheckModifier(dice);
                dice = declare.dice;
                char modifierType = declare.modifierType;
                int modifier = declare.modifier;
                string answer = declare.answer;
                bool alreadyAnswered = declare.alreadyAnswered;

                if (!dice.Contains('d')) { answer = "Error: Your input has no `d`. For help, see: `/help r`."; alreadyAnswered = true; }

                if (!alreadyAnswered)
                {
                    if (dice[0] == 'd')
                    {
                        if (!Int32.TryParse(dice.Substring(1), out diceSplit[1])) { answer = "Error: Invalid format. For help, see: `/help r`."; }
                        else if (diceSplit[1] < 1 || diceSplit[1] > 100) { answer = "Error: The number of dice or the number of sides falls outside of the accepted range. For help, see: `/help r`."; }
                        else { answer = RollDice(1, diceSplit[1], modifierType, modifier, hasReroll); }
                    }
                    else
                    {
                        dicePreSplit = dice.Split('d');
                        if (!Int32.TryParse(dicePreSplit[0], out diceSplit[0])) { answer = "Error: Invalid format. For help, see: `/help r`."; }
                        else if (!Int32.TryParse(dicePreSplit[1], out diceSplit[1])) { answer = "Error: Invalid format. For help, see: `/help r`."; }
                        else if (diceSplit[0] < 1 || diceSplit[0] > 10 || diceSplit[1] < 1 || diceSplit[1] > 100) { answer = "Error: The number of dice or the number of sides falls outside of the accepted range. For help, see: `/help r`."; }
                        else { answer = RollDice(diceSplit[0], diceSplit[1], modifierType, modifier, hasReroll); }
                    }
                }

                await context.RespondAsync(answer);

                RollTime.Stop();
                Console.WriteLine($"  Character-generating finished in {RollTime.ElapsedMilliseconds}ms.");
                if (RollTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Generating a character took {RollTime.ElapsedMilliseconds}ms."); }
            }
        }

        public class FlipCommand
        {
            [Command("flip"), Description("Flips a coin.")]
            public static async ValueTask ExecuteAsync(CommandContext context)
            {
                var FlipTime = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Flipping a coin.");

                Random random = new Random();
                string answer = "You flip a coin…\n\nIt's ";
                if (random.NextDouble() >= 0.5) { answer += "heads!"; }
                else answer += "tails!";

                await context.RespondAsync(answer);

                FlipTime.Stop();
                Console.WriteLine($"  Coin-flipping finished in {FlipTime.ElapsedMilliseconds}ms.");
                if (FlipTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Flipping a coin took {FlipTime.ElapsedMilliseconds}ms."); }
            }
        }

        public class ConvertCommand
        {
            [Command("convert"), Description("Converts a value between two units.")]
            public static async ValueTask ExecuteAsync(CommandContext context,
                [Description("The value you wish to convert.")] string inputValue,
                [Description("The unit you with to convert from."), SlashAutoCompleteProvider<ConvertCommandAutoCompleteProvider>] string inputUnit,
                [Description("The unit you wish to convert to."), SlashAutoCompleteProvider<ConvertCommandAutoCompleteProvider>] string outputUnit)
            {
                var ConvertTime = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: Converting a value.");

                bool alreadyAnswered = false;
                inputValue = inputValue.Replace('.', ',');
                if (!double.TryParse(inputValue, out double inputDouble))
                {
                    await context.RespondAsync($"Error: The input value could not be parsed.");
                    alreadyAnswered = true;
                }

                string[] inputUnitSplit = inputUnit.Split('.');
                IQuantity? inputQuantity = null;
                if (!alreadyAnswered && !Quantity.TryFrom(value: inputDouble, quantityName: inputUnitSplit[0], unitName: inputUnitSplit[1], out inputQuantity))
                {
                    await context.RespondAsync($"Error: The input type `{inputUnit}` is invalid.");
                    alreadyAnswered = true;
                }

                string[] outputUnitSplit = outputUnit.Split('.');
                IQuantity? outputQuantity = null;
                if (!alreadyAnswered && !Quantity.TryFrom(value: 0, quantityName: outputUnitSplit[0], unitName: outputUnitSplit[1], out outputQuantity))
                {
                    await context.RespondAsync($"Error: The output type `{outputUnit}` is invalid.");
                    alreadyAnswered = true;
                }

                if (!alreadyAnswered && inputQuantity.QuantityInfo.UnitType != outputQuantity.QuantityInfo.UnitType)
                {
                    await context.RespondAsync($"Error: The input and output categories `{inputQuantity.QuantityInfo.UnitType}`, `{outputQuantity.QuantityInfo.UnitType}` do not match.");
                    alreadyAnswered = true;
                }

                if (!alreadyAnswered)
                {
                    string inputNumberFormatted = inputQuantity.ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));
                    string outputNumberFormatted = inputQuantity.ToUnit(outputQuantity.Unit).ToString("G2", CultureInfo.CreateSpecificCulture("en-US"));

                    if (char.IsDigit(inputNumberFormatted[inputNumberFormatted.Length - 1])) { inputNumberFormatted += $" {inputUnitSplit[1]}"; }
                    if (char.IsDigit(outputNumberFormatted[outputNumberFormatted.Length - 1])) { outputNumberFormatted += $" {outputUnitSplit[1]}"; }

                    string answer = $"**{inputNumberFormatted}** equals **{outputNumberFormatted}**.";

                    await context.RespondAsync(answer);
                }

                ConvertTime.Stop();
                Console.WriteLine($"  Value-converting finished in {ConvertTime.ElapsedMilliseconds}ms.");
                if (ConvertTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Converting a value took {ConvertTime.ElapsedMilliseconds}ms."); }
            }
        }
    }
}