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

namespace Acorn
{
    class Program
    {
        static string discordTokenPath = "tock.txt";
        static string quotesPath = "quotes.json";
        static string discordToken = File.ReadLines(discordTokenPath).First();
        static DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
        static DiscordClient debugClient = builder.Build();
        static DiscordChannel debugChannel;
        //TODO: initialise quotes- and help articles lists

        static async Task Main(string[] args)
        {
            var initTime = System.Diagnostics.Stopwatch.StartNew();

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

            // Now we connect and log in.
            await client.ConnectAsync(status, DiscordUserStatus.Online);

            initTime.Stop();
            PrintDebugMessage($"Initialising finished in {initTime.ElapsedMilliseconds} ms.");

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
                //TODO: timer start

                //TODO: call quotelist.add routine
                //TODO: answer with the newest quote

                //TODO: timer stop
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
                //TODO: timer start

                //TODO: return with the relevant help article

                //TODO: timer stop
            }
        }

        public class QuoteCommand
        {
            [Command("quote"), Description("Prints a random quote from the collection.")]
            public static async ValueTask ExecuteAsync(CommandContext context)
            {
                //TODO: timer start

                //TODO: return random quote
                //make sure the above has a subroutine to reshuffle if needed

                //TODO: timer stop
            }
        }

        public class SpecificQuoteCommand
        {
            [Command("specificquote"), Description("Prints a specified quote from the collection.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of the quote you wish to recall.")] string quoteId)
            {
                //TODO: timer start

                //TODO: return specific quote

                //TOD: timer stop
            }
        }

        public class SearchQuoteCommand
        {
            [Command("searchquote"), Description("Searches for quotes that match the given query.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The search query. At least 3 characters long.")] string query)
            {
                //TODO: timer start

                //TODO: return search results

                //TODO: timer stop
            }
        }

        public class CharacterCommand
        {
            [Command("character"), Description("Prints a randomly-rolled Dungeons and Dragons character block.")]
            public static async ValueTask ExecuteAsync(CommandContext context)
            {
                //TODO: timer start

                //TODO: roll character

                //TODO: timer stop
            }
        }

        public class RollDiceCommand
        {
            [Command("roll"), Description("Rolls n x-sided dice. Example: ‘/r 2d4’. Details: ‘/help r’.")]
            public static async ValueTask ExecuteAsync(CommandContext context, [Description("The number of dice to roll and the number of their sides. Example: ‘2d4’.")] string dice)
            {
                //TODO: timer start

                //TODO: roll dice

                //TODO: timer stop
            }
        }

        public class FlipCommand
        {
            [Command("flip"), Description("Flips a coin.")]
            public static async ValueTask ExecuteAsync(CommandContext context)
            {
                //TODO: timer start

                //TODO: flip coin

                //TODO: timer stop
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
                //TODO: timer start

                //TODO: convert

                //TODO: timer stop
            }
        }
    }
}