using Acorn.Classes;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Acorn
{
    class Program
    {
        static string discordTokenPath = "tock.txt";
        public static string quotesPath = "quotes.json";
        public static string backupsPath = "";
        public static string currencyPath = "eur.json";
        static string helpArticlesPath = "help.json";
        static string discordToken = File.ReadLines(discordTokenPath).First();
        static DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, DiscordIntents.MessageContents | TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
        public static DiscordClient debugClient = builder.Build();
        static DiscordChannel debugChannel;
        public static HelpArticlesList helpArticlesList = new(helpArticlesPath);
        public static QuotesList quotesList = new(debugClient, quotesPath);
        public static Magic8Ball magic8Ball = new();
        public static Exchange exchange = new();

        public async static void PrintDebugMessage(string message)
        {
            await debugChannel.SendMessageAsync(message);
        }

        static async Task Main(string[] args)
        {
            builder = DiscordClientBuilder.CreateDefault(discordToken, DiscordIntents.MessageContents | TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents);
            var initTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Initialising...");
            debugChannel = debugClient.GetChannelAsync(1337097452859428877).Result;
            backupsPath = $"backups{Path.DirectorySeparatorChar}";

            if (string.IsNullOrEmpty(discordToken))
            {
                PrintDebugMessage("Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
                initTime.Stop();
                Environment.Exit(1);
            }

            builder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands(
                    [
                    //Slash Commands
                    typeof(Commands_Slash.HelpCommand), typeof(Commands_Slash.RollDiceCommand), typeof(Commands_Slash.QuoteCommand),
                    typeof(Commands_Slash.SpecificQuoteCommand), typeof(Commands_Slash.CharacterCommand), typeof(Commands_Slash.FlipCommand),
                    typeof(Commands_Slash.ConvertCommand), typeof(Commands_Slash.SearchQuoteCommand), typeof(Commands_Slash.QuoteByCommand),
                    typeof(Commands_Slash._8BallCommand), typeof(Commands_Slash.ExchangeCommand),

                    //Context Menu Commands
                    typeof(Commands_ContextMenu.AddQuoteMenu), typeof(Commands_ContextMenu.UndoAddQuoteMenu),

                    //Text Commands
                    typeof(Commands_Text.MessageCommand), typeof(Commands_Text.ReplyToCommand), typeof(Commands_Text.AlttextCommand), typeof(Commands_Text.DiskCommand),
                    typeof(Commands_Text.AlcoholShCommand), typeof(Commands_Text.BanShCommand), typeof(Commands_Text.CreatureShCommand), typeof(Commands_Text.CoffeeShCommand),
                    typeof(Commands_Text.HelpShCommand), typeof(Commands_Text.HorrorShCommand), typeof(Commands_Text.NoShCommand),
                    typeof(Commands_Text.ŐShCommand), typeof(Commands_Text.PirateShCommand), typeof(Commands_Text.QuoteShCommand), typeof(Commands_Text.SelfieShCommand),
                    typeof(Commands_Text.SteeveShCommand), typeof(Commands_Text.StopShCommand), typeof(Commands_Text.WakeupShCommand)]);
                TextCommandProcessor textCommandProcessor = new(new()
                {
                    PrefixResolver = new DefaultPrefixResolver(true, ".").ResolvePrefixAsync,
                });

                extension.AddProcessor(textCommandProcessor);
            }, new CommandsConfiguration() { RegisterDefaultCommandProcessors = true });

            DiscordClient client = builder.Build();

            DiscordActivity status = new("with you 🫵", DiscordActivityType.Playing);

            await client.ConnectAsync(status, DiscordUserStatus.Online);

            initTime.Stop();
            PrintDebugMessage($"Initialising finished in {initTime.ElapsedMilliseconds} ms.");

            await Task.Delay(-1);
        }
    }
}