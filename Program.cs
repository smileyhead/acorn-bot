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
using System.Text.RegularExpressions;

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
        public string? Link { get; set; }
    }

    class Program
    {


        static string PrintQuote(string idInput, bool isRandom)
        {
            string answer = "";
            string quotesPath = "quotes.json";
            DiscordUser user = null;
            string discordToken = File.ReadLines("tock.txt").First();
            DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
            DiscordClient client = builder.Build();
            string error = "";
            string username = "";
            int id = 0;

            List<Quotes>? quote = null;
            using (FileStream readQuotes = File.OpenRead(quotesPath))
            {
                quote =
                    JsonSerializer.Deserialize<List<Quotes>>(readQuotes);
            }

            if (isRandom) //q called
            {
                Random random = new Random();
                id = random.Next(0, quote.Count);
            }
            else //sq called
            {
                if (idInput[0] == '#') { idInput = idInput.Remove(0, 1); }

                if (idInput.ToLower() == "latest") { id = quote[quote.Count - 1].Id; }
                else if (!Int32.TryParse(idInput, out id)) { answer = "Error: Invalid format. For help, see: `/help sq`."; }
                else if (id < 0 || id > quote.Count() - 1) { answer = "Error: The specified number falls outside the accepted range. For help, see: `/help sq`."; }
            }

            if (answer == "")
            {
                try
                {
                    user = client.GetUserAsync(quote[id].UserId).Result;
                }
                catch (Exception e)
                {
                    if (e is BadRequestException) { error = "\n-# Error: The user ID provided was not valid. Is it stored correctly? Falling back to placeholder name."; }
                    if (e is ServerErrorException) { error = "\n-# Error: Discord has encountered a server error. Falling back to placeholder name."; }
                }
                if (error != "") { username = "Someone"; }
                else { username = user.GlobalName; }

                answer += $"`#{quote[id].Id}` **{username}** [said](https://discord.com/channels/{quote[id].Link}):";
                string[] bodySplit = quote[id].Body.Split('\n');
                for (int i = 0; i < bodySplit.Count(); i++)
                {
                    answer += $"\n> {bodySplit[i]}";
                }

                answer += error;
            }

            return answer;
        }

        static void CreateBackup(string quotesPath)
        {
            File.Move(quotesPath, "/backups/" + quotesPath.Insert(quotesPath.IndexOf('.'), $"-backup_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}"));
            //quotes.json -> /backups/quotes-backup_yyyy-MM-dd-HH-mm-ss.json
        }

        static void WriteToQuotes(string quotesPath, List<Quotes> quote)
        {
            using FileStream writeQuotes = File.Create(quotesPath);
            JsonSerializer.SerializeAsync(writeQuotes, quote);
        }

        static (string dice, char modifierType, int modifier, string answer, bool alreadyAnswered) CheckModifier(string dice)
        {
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

            if (modifierType == '\0') { answer += $"→ {FormatNumber(rolls.Sum())}"; }
            else if (modifierType == '+') { answer += $"{modifierType} {FormatNumber(modifier)} → {FormatNumber(rolls.Sum() + modifier)}"; }
            else if (modifierType == '-') { answer += $"{modifierType} {FormatNumber(modifier)} → {FormatNumber(rolls.Sum() - modifier)}"; }

            if (emergencyStopActivated) { answer += $"\n-# Note: The rerolling of one or more of these values exceeded 10 tries. For more information, see: `/help r`."; }
            return answer;
        }

        static async Task Main(string[] args)
        {
            string discordTokenPath = "tock.txt";
            string discordToken = File.ReadLines(discordTokenPath).First();

            if (string.IsNullOrEmpty(discordToken))
            {
                Console.WriteLine("Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
                Environment.Exit(1);
            }

            DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);

            // Setup the commands extension
            builder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands([typeof(HelpCommand), typeof(RollDiceCommand), typeof(QuoteCommand), typeof(SpecificQuoteCommand), typeof(CharacterCommand), typeof(SearchQuoteCommand), typeof(AddQuoteMenu)]);
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

            // We can specify a status for our bot. Let's set it to "playing" and set the activity to "with fire".
            DiscordActivity status = new("with fire", DiscordActivityType.Playing);

            // Now we connect and log in.
            await client.ConnectAsync(status, DiscordUserStatus.Online);

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
                string quotesPath = "quotes.json";
                DiscordUser? author = message.Author;
                IReadOnlyList<DiscordAttachment> attachments = message.Attachments;

                List<Quotes>? quote = null;
                using (FileStream readQuotes = File.OpenRead(quotesPath))
                {
                    quote =
                        await JsonSerializer.DeserializeAsync<List<Quotes>>(readQuotes);
                }

                CreateBackup(quotesPath);

                Quotes newQuote = new Quotes();
                newQuote.Id = quote.Count;
                newQuote.Body = message.Content;
                newQuote.UserId = author.Id;
                newQuote.Link = message.JumpLink.ToString().Substring(30); //Remove the redundant parts (sure do hope this doesn't change in the future...)
                if (attachments.Count > 0)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        newQuote.Body += $"\n{attachments[i].Url}";
                    }
                }
                quote.Add(newQuote);

                WriteToQuotes(quotesPath, quote);

                string answer = "Adding quote:\n";
                await context.RespondAsync($"{answer}{PrintQuote((quote.Count - 1).ToString(), false)}");
            }
        }

        /*------------
        Slash Commands
        ------------*/
        public class HelpCommand
        {
            [Command("help"), Description("Prints the help article for a given command.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The command which you need help with.")] string command)
            {
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
            }
        }

        public class QuoteCommand
        {
            [Command("q"), Description("Prints a random quote from the collection.")]
            public static async ValueTask ExecuteAsync(CommandContext context)
            {
                await context.RespondAsync(PrintQuote("0", true));
            }
        }

        public class SpecificQuoteCommand
        {
            [Command("sq"), Description("Prints a specified quote from the collection.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of the quote you wish to recall.")] string quoteId)
            {
                await context.RespondAsync(PrintQuote(quoteId, false));
            }
        }

        public class SearchQuoteCommand
        {
            [Command("srchq"), Description("Searches for quotes that match the given query.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The search query. At least 3 characters long.")] string query)
            {
                string answer = "";
                string quotesPath = "quotes.json";

                if (query.Length < 3)
                {
                    await context.RespondAsync("Your query must be at least 3 characters long.");
                }
                else
                {
                    List<Quotes>? quote = null;
                    using (FileStream readQuotes = File.OpenRead(quotesPath))
                    {
                        quote =
                            JsonSerializer.Deserialize<List<Quotes>>(readQuotes);
                    }

                    DiscordUser user = null;
                    string discordToken = File.ReadLines("tock.txt").First();
                    DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
                    DiscordClient client = builder.Build();
                    string error = "";
                    string username = "";

                    int[] results = new int[quote.Count];
                    int resultsI = 0;
                    string[] currentResult = new string[3];
                    int highlightStart = 0;

                    for (int i = 0; i < quote.Count; i++)
                    {
                        if (quote[i].Body.Contains(query, StringComparison.OrdinalIgnoreCase))
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
                            try
                            {
                                user = client.GetUserAsync(quote[results[i]].UserId).Result;
                            }
                            catch (Exception e)
                            {
                                if (e is BadRequestException) { error = "\n-# Error: The user ID provided was not valid. Is it stored correctly? Falling back to placeholder name."; }
                                if (e is ServerErrorException) { error = "\n-# Error: Discord has encountered a server error. Falling back to placeholder name."; }
                            }
                            if (error != "") { username = "Someone"; }
                            else { username = user.GlobalName; }

                            answer += $"`#{results[i]}` **{username}:** ";

                            if (quote[results[i]].Body.Length <= 30) { answer += $"‘{quote[results[i]].Body}’\n"; }
                            else { answer += $"‘{quote[results[i]].Body.Substring(0, 29)}…’\n"; }
                        }

                        await context.RespondAsync(answer);
                    }
                }
            }
        }

        public class CharacterCommand
        {
            [Command("character"), Description("Prints a randomly-rolled Dungeons and Dragons character block.")]
            public static async ValueTask ExecuteAsync(CommandContext context)
            {
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
            }
        }

        public class RollDiceCommand
        {
            [Command("r"), Description("Rolls n x-sided dice. Example: ‘/r 2d4’. Details: ‘/help r’.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of dice to roll and the number of their sides. Example: ‘2d4’.")] string dice)
            {
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
            }
        }
    }
}