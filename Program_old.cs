//using DSharpPlus;
//using DSharpPlus.Commands;
//using DSharpPlus.Commands.Processors.SlashCommands;
//using DSharpPlus.Commands.Processors.TextCommands;
//using DSharpPlus.Commands.Processors.TextCommands.Parsing;
//using DSharpPlus.Entities;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.ComponentModel;
//using System.Security.Cryptography.X509Certificates;
//using System.Linq;
//using System.Runtime.Intrinsics.Arm;
//using DSharpPlus.Exceptions;
//using DSharpPlus.Commands.Processors.UserCommands;
//using DSharpPlus.Commands.Processors.MessageCommands;
//using DSharpPlus.Commands.Trees.Metadata;

//namespace Acorn_old
//{
//    public struct HelpArticles
//    {
//        public string command;
//        public string helpText;
//    }

//    public class Quotes
//    {
//        public int id { get; set; }
//        public string body { get; set; }
//        public ulong userId { get; set; }
//        public string link { get; set; }
//    }

//    class Program
//    {
//        static async Task Main_old(string[] args)
//        {
//            string discordToken = File.ReadLines("tock.txt").First();
//            if (string.IsNullOrWhiteSpace(discordToken))
//            {
//                Console.WriteLine("Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
//                Environment.Exit(1);
//            }


//            DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);

//            // Setup the commands extension
//            builder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
//            {
//                extension.AddCommands([typeof(HelpCommand), typeof(RollDiceCommand), typeof(QuoteCommand), typeof(SpecificQuoteCommand), typeof(AddQuoteMenu)]);
//                TextCommandProcessor textCommandProcessor = new(new()
//                {
//                    // The default behavior is that the bot reacts to direct
//                    // mentions and to the "!" prefix. If you want to change
//                    // it, you first set if the bot should react to mentions
//                    // and then you can provide as many prefixes as you want.
//                    PrefixResolver = new DefaultPrefixResolver(false, ".").ResolvePrefixAsync,
//                });

//                // Add text commands with a custom prefix (?ping)
//                extension.AddProcessor(textCommandProcessor);
//            }, new CommandsConfiguration()
//            {
//                // The default value is true, however it's shown here for clarity
//                RegisterDefaultCommandProcessors = true,
//                //DebugGuildId = Environment.GetEnvironmentVariable("DEBUG_GUILD_ID") ?? 0,
//            });

//            DiscordClient client = builder.Build();

//            // We can specify a status for our bot. Let's set it to "playing" and set the activity to "with fire".
//            DiscordActivity status = new("with fire", DiscordActivityType.Playing);

//            // Now we connect and log in.
//            await client.ConnectAsync(status, DiscordUserStatus.Online);

//            // And now we wait infinitely so that our bot actually stays connected.
//            await Task.Delay(-1);
//        }

//        public class AddQuoteMenu
//        {
//            [Command("Add Quote")]
//            [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
//            [AllowedProcessors(typeof(MessageCommandProcessor))]
//            public async Task AddQuote(MessageCommandContext context, DiscordMessage message)
//            {
//                string answer = "";
//                DiscordUser author = message.Author;
//                IReadOnlyList<DiscordAttachment> attachment = message.Attachments;

//                List<Quotes> quote = null;

//                string json;
//                using (StreamReader reader = new StreamReader("quotes.json"))
//                {
//                    json = reader.ReadToEnd();
//                    quote = JsonSerializer.Deserialize<List<Quotes>>(json);
//                }

//                Quotes newQuote = new Quotes();
//                newQuote.id = quote.Count;
//                newQuote.body = message.Content;
//                newQuote.userId = author.Id;
//                newQuote.link = $"{message.JumpLink.ToString().Substring(30)}\n"; //Keep only the non-redundant parts (sure do hope this doesn't change in the future...)
//                if (attachment.Count() > 0)
//                {
//                    for (int i = 0; i < attachment.Count(); i++)
//                    {
//                        newQuote.body += $"\n{attachment[i].Url}";
//                    }
//                }
//                int quoteNumber = quote.Count;
//                quote.Add(newQuote);

//                json = JsonSerializer.Serialize(quote);

//                answer += "The following quote has been added to the database:\n";

//                string discordToken = File.ReadLines("tock.txt").First();
//                DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
//                DiscordClient client = builder.Build();
//                string error = "";
//                string username = "";
//                DiscordUser data = null;

//                try
//                {
//                    data = client.GetUserAsync(quote[quoteNumber].userId).Result;
//                }
//                catch (Exception e)
//                {
//                    if (e is BadRequestException) { error = "\n-# Error: The user ID provided was not valid. Is it stored correctly? Falling back to placeholder name."; }
//                    if (e is ServerErrorException) { error = "\n-# Error: Discord has encountered a server error. Falling back to placeholder name."; }
//                }

//                if (error != "") { username = "Someone"; }
//                else { username = data.GlobalName; }

//                string[] bodySplit = new string[quote[quoteNumber].body.Count(x => x == '\n')];
//                bodySplit = quote[quoteNumber].body.Split('\n');

//                answer += $"`#{quoteNumber}` **{username}** [said](https://discord.com/channels/{quote[quoteNumber].link}):\n";
//                char separator = '\n';
//                for (int i = 0; i < bodySplit.Count(); i++)
//                {
//                    if (i == bodySplit.Count() - 1) { separator = ' '; }
//                    answer += $"> {bodySplit[i]}{separator}";
//                }
//                answer += error;

//                await context.RespondAsync(answer);
//            }
//        }

//        public class HelpCommand
//        {
//            [Command("help"), Description("Prints the help article for a given command.")]
//            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The command which you need help with.")] string command)
//            {
//                using (StreamReader reader = new StreamReader("help.json"))
//                {
//                    bool alreadyAnswered = false;
//                    string json = reader.ReadToEnd();
//                    List<HelpArticles> helpArticle = JsonSerializer.Deserialize<List<HelpArticles>>(json);

//                    for (int i = 0; i < helpArticle.Count; i++)
//                    {
//                        if (helpArticle[i].command == command)
//                        {
//                            await context.RespondAsync(helpArticle[i].helpText);
//                            alreadyAnswered = true;
//                        }
//                        else if (!alreadyAnswered && i == helpArticle.Count - 1) { await context.RespondAsync("Command not found."); }
//                    }
//                }

//            }
//        }

//        public class QuoteCommand
//        {
//            [Command("q"), Description("Prints a random quote from the collection.")]
//            public static async ValueTask ExecuteAsync(CommandContext context)
//            {
//                using (StreamReader reader = new StreamReader("quotes.json"))
//                {
//                    string json = reader.ReadToEnd();
//                    Quotes quote = JsonSerializer.Deserialize<Quotes>(json);
                    
//                    Random random = new Random();
//                    int quoteNumber = random.Next(0, quote.Count() + 1);
//                    string discordToken = File.ReadLines("tock.txt").First();
//                    DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
//                    DiscordClient client = builder.Build();
//                    string error = "";
//                    string username = "";
//                    DiscordUser data = null;

//                    try
//                    {
//                        data = client.GetUserAsync(quote[quoteNumber].userId).Result;
//                    }
//                    catch (Exception e)
//                    {
//                        if (e is BadRequestException) { error = "\n-# Error: The user ID provided was not valid. Is it stored correctly? Falling back to placeholder name."; }
//                        if (e is ServerErrorException) { error = "\n-# Error: Discord has encountered a server error. Falling back to placeholder name."; }
//                    }

//                    if (error != "") { username = "Someone"; }
//                    else { username = data.GlobalName; }

//                    string[] bodySplit = new string[quote[quoteNumber].body.Count(x => x == '\n')];
//                    bodySplit = quote[quoteNumber].body.Split('\n');

//                    string answer = $"`#{quoteNumber}` **{username}** [said](https://discord.com/channels/{quote[quoteNumber].link}):\n";
//                    char separator = '\n';
//                    for (int i = 0; i < bodySplit.Count(); i++)
//                    {
//                        if (i == bodySplit.Count() - 1) { separator = ' '; }
//                        answer += $"> {bodySplit[i]}{separator}";
//                    }
//                    answer += error;

//                    await context.RespondAsync(answer);
//                }
//            }
//        }

//        public class SpecificQuoteCommand
//        {
//            [Command("sq"), Description("Prints a specified quote from the collection.")]
//            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of the quote you wish to recall.")] string quoteId)
//            {
//                using (StreamReader reader = new StreamReader("quotes.json"))
//                {
//                    string json = reader.ReadToEnd();
//                    List<Quotes> quote = JsonSerializer.Deserialize<List<Quotes>>(json);
//                    int quoteNumber = 0;
//                    bool alreadyAnswered = false;

//                    if (quoteId == "latest")
//                    {
//                        quoteNumber = quote.Count - 1;
//                    }
//                    else if (!Int32.TryParse(quoteId, out quoteNumber))
//                    {
//                        await context.RespondAsync($"Error: Invalid format. For help, see: `/help sq`.");
//                        alreadyAnswered = true;
//                    }
//                    else if (quoteNumber < 0 || quoteNumber > quote.Count() - 1)
//                    {
//                        await context.RespondAsync($"Error: The specified number falls outside the accepted range. For help, see: `/help sq`.");
//                        alreadyAnswered = true;
//                    }

//                    if(!alreadyAnswered)
//                    {
//                        string discordToken = File.ReadLines("tock.txt").First();
//                        DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
//                        DiscordClient client = builder.Build();
//                        string error = "";
//                        string username = "";
//                        DiscordUser data = null;

//                        try
//                        {
//                            data = client.GetUserAsync(quote[quoteNumber].userId).Result;
//                        }
//                        catch (Exception e)
//                        {
//                            if (e is BadRequestException) { error = "\n-# Error: The user ID provided was not valid. Is it stored correctly? Falling back to placeholder name."; }
//                            if (e is ServerErrorException) { error = "\n-# Error: Discord has encountered a server error. Falling back to placeholder name."; }
//                        }

//                        if (error != "") { username = "Someone"; }
//                        else { username = data.GlobalName; }

//                        string[] bodySplit = new string[quote[quoteNumber].body.Count(x => x == '\n')];
//                        bodySplit = quote[quoteNumber].body.Split('\n');

//                        string answer = $"`#{quoteNumber}` **{username}** [said](https://discord.com/channels/{quote[quoteNumber].link}):\n";
//                        char separator = '\n';
//                        for (int i = 0; i < bodySplit.Count(); i++)
//                        {
//                            if (i == bodySplit.Count() - 1) { separator = ' '; }
//                            answer += $"> {bodySplit[i]}{separator}";
//                        }
//                        answer += error;

//                        await context.RespondAsync(answer);
//                    }
//                }
//            }
//        }

        

//        public class PingCommand
//        {
//            [Command("ping"), Description("Answers you.")]
//            public static async ValueTask ExecuteAsync(CommandContext context) =>
//                await context.RespondAsync($"A Winner is You!");
//        }

//        public class RollDiceCommand
//        {
//            [Command("r"), Description("Rolls n x-sided dice. Example: ‘/r 2d4’. Details: ‘/help r’.")]
//            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of dice to roll and the number of their sides. Example: ‘2d4’.")] string dice)
//            {
//                string RollDice(int diceN, int sidesN, bool hasReroll, char modifierType, int modifier)
//                {
//                    int[] rolls = new int[diceN];
//                    string[] numberName = new string[10];
//                    numberName = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"];
//                    bool rerollProtectionTriggered = false;
//                    string answer = "";
//                    string separator = ", ";
//                    Random random = new Random();

//                    if (diceN == 1) { answer += $"You roll a {sidesN}-sided die…\n\n"; }
//                    else { answer += $"You roll {numberName[diceN - 1]} {sidesN}-sided dice…\n\n"; }

//                    for (int i = 0; i < diceN; i++)
//                    {
//                        rolls[i] = random.Next(1, sidesN + 1);

//                        if (i == diceN - 1) { separator = ""; }

//                        if (rolls[i] == sidesN) //Critical success
//                        {
//                            answer += $"**{rolls[i]}**{separator}";
//                        }
//                        else if (hasReroll && rolls[i] == 1) // Critical fail with reroll
//                        {
//                            for (int j = 0; j < 10; j++)
//                            {
//                                rolls[i] = random.Next(1, sidesN + 1);
//                                if (rolls[i] != 1) { break; }
//                                if (j == 9) { 
//                                    rerollProtectionTriggered = true;
//                                    if (sidesN > 2) { rolls[i] = random.Next(2, sidesN - 1); }
//                                    else { rolls[i] = 2; }
//                                    break;
//                                }
//                            }
//                            if (rerollProtectionTriggered) { answer += $"__*{rolls[i]}*__{separator}"; }
//                            else { answer += $"__{rolls[i]}__{separator}"; }
//                        }
//                        else if (!hasReroll && rolls[i] == 1) //Critical fail
//                        {
//                            answer += $"__{rolls[i]}__{separator}";
//                        }
//                        else
//                        {
//                            answer += $"{rolls[i]}{separator}";
//                        }
//                    }

//                    if (modifierType == '.')
//                    {
//                        answer += $" → {rolls.Sum()}";
//                    }
//                    else if (modifierType == '+')
//                    {
//                        answer += $" {modifierType} {modifier} → {rolls.Sum() + modifier}";
//                    }
//                    else
//                    {
//                        answer += $" {modifierType} {modifier} → {rolls.Sum() - modifier}";
//                    }

//                    if (rerollProtectionTriggered) { answer += "\n-# Note: The rerolling of one or more of these values exceeded 10 tries. For more information, see: `/help r`."; }

//                    return answer;
//                }

//                string[] diceSplit = new string[2];
//                int diceN = 0;
//                int sidesN = 0;
//                bool hasReroll = false;
//                char modifierType = '.';
//                int modifier = 0;
//                bool alreadyResponded = false;

//                for (int i = 0; i < dice.Length; i++)
//                {
//                    if (dice[i] == 'd')
//                    {
//                        break;
//                    }

//                    if (i == dice.Length - 1)
//                    {
//                        await context.RespondAsync($"Error: Your input has no `d`. For help, see: `/help r`.");
//                        alreadyResponded = true;
//                    }
//                }

//                for (int i = 0; i < dice.Length; i++)
//                {
//                    if (dice[i] == '+' || dice[i] == '-')
//                    {
//                        modifierType = dice[i];
//                        if (Int32.TryParse(dice.Substring(i + 1), out modifier) == false)
//                        {
//                            await context.RespondAsync($"Error: Invalid format. For help, see: `/help r`.");
//                            alreadyResponded = true;
//                        }

//                        dice = dice.Remove(i);
//                    }
//                }

//                if (!alreadyResponded && dice[0] == 'd')
//                {
//                    dice = dice.Substring(1);
//                    if (Int32.TryParse(dice, out sidesN) == false)
//                    {
//                        await context.RespondAsync($"Error: Invalid format. For help, see: `/help r`.");
//                        alreadyResponded = true;
//                    }

//                    if (!alreadyResponded && sidesN > 100)
//                    {
//                        await context.RespondAsync($"Error: The number of dice or number of sides specified is too high. For help, see: `/help r`.");
//                    }
//                    else if (!alreadyResponded && sidesN < 1)
//                    {
//                        await context.RespondAsync($"Error: You must roll at least one d1. For help, see: `/help r`.");
//                    }
//                    else if (!alreadyResponded)
//                    {
//                        await context.RespondAsync(RollDice(1, sidesN, hasReroll, modifierType, modifier));
//                    }
//                }
//                else if (!alreadyResponded)
//                {
//                    diceSplit = dice.Split('d');
                    
//                    if (diceSplit[1].Length > 2 && diceSplit[1].Substring(diceSplit[1].Length - 2) == "r1")
//                    {
//                        hasReroll = true;
//                        diceSplit[1] = diceSplit[1].Remove(diceSplit[1].Length - 2, 2);
//                    }

//                    if (Int32.TryParse(diceSplit[0], out diceN) == false ||
//                        Int32.TryParse(diceSplit[1], out sidesN) == false)
//                    {
//                        await context.RespondAsync($"Error: Invalid format. For help, see: `/help r`.");
//                    }
//                    else if (diceN > 10 || sidesN > 100)
//                    {
//                        await context.RespondAsync($"Error: The number of dice or number of sides specified is too high. For help, see: `/help r`.");
//                    }
//                    else if (diceN < 1 || sidesN < 1)
//                    {
//                        await context.RespondAsync($"Error: You must roll at least one d1. For help, see: `/help r`.");
//                    }
//                    else
//                    {
//                        await context.RespondAsync(RollDice(diceN, sidesN, hasReroll, modifierType, modifier));
//                    }
//                }
//            }
//        }
//    }
//}
