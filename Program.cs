using Acorn.AutoCompleteProviders;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using System.ComponentModel;
using System.Globalization;

namespace Acorn
{
    class Program
    {
        static string discordTokenPath = "tock.txt";
        static string quotesPath = "quotes.json";
        static string helpArticlesPath = "help.json";
        static string discordToken = File.ReadLines(discordTokenPath).First();
        static DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
        static DiscordClient debugClient = builder.Build();
        static DiscordChannel debugChannel;
        static HelpArticlesList helpArticlesList = new(helpArticlesPath);
        //TODO: initialise quotes lists

        async static void PrintDebugMessage(string message)
        {
            await debugChannel.SendMessageAsync(message);
        }

        static async Task Main(string[] args)
        {
            var initTime = System.Diagnostics.Stopwatch.StartNew();
            debugChannel = debugClient.GetChannelAsync(1337097452859428877).Result;

            if (string.IsNullOrEmpty(discordToken))
            {
                PrintDebugMessage("Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
                initTime.Stop();
                Environment.Exit(1);
            }

            builder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands(
                    [typeof(HelpCommand), typeof(RollDiceCommand), typeof(QuoteCommand), typeof(SpecificQuoteCommand), typeof(CharacterCommand),
                    typeof(FlipCommand), typeof(ConvertCommand), typeof(SearchQuoteCommand), typeof(AddQuoteMenu)]);
                TextCommandProcessor textCommandProcessor = new(new()
                {
                    PrefixResolver = new DefaultPrefixResolver(false, ".").ResolvePrefixAsync,
                });

                extension.AddProcessor(textCommandProcessor);
            }, new CommandsConfiguration() { RegisterDefaultCommandProcessors = true });

            DiscordClient client = builder.Build();

            DiscordActivity status = new("with fire", DiscordActivityType.Playing);

            await client.ConnectAsync(status, DiscordUserStatus.Online);

            initTime.Stop();
            PrintDebugMessage($"Initialising finished in {initTime.ElapsedMilliseconds} ms.");

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

                //TODO: call quotelist.add routine
                //TODO: answer with the newest quote

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

                await context.RespondAsync(helpArticlesList.GetHelp(command));

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

                //TODO: return random quote
                //make sure the above has a subroutine to reshuffle if needed

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

                //TODO: return specific quote

                //TOD: timer stopSpecificQuoteTime.Stop();
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

                //TODO: return search results

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

                //TODO: roll character

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

                //TODO: roll dice

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

                //TODO: flip coin

                //TODO: timer stopFlipTime.Stop();
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

                //TODO: convert

                ConvertTime.Stop();
                Console.WriteLine($"  Value-converting finished in {ConvertTime.ElapsedMilliseconds}ms.");
                if (ConvertTime.ElapsedMilliseconds > 3000) { PrintDebugMessage($"Converting a value took {ConvertTime.ElapsedMilliseconds}ms."); }
            }
        }
    }
}