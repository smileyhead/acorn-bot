using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;

namespace Acorn.Commands_Text
{
    public static class MessageCommand
    {
        [Command("message"), AllowedProcessors<TextCommandProcessor>()]
        public static async ValueTask ExecuteAsync(CommandContext context, [RemainingText] string input)
        {
            string channelId = input.Substring(0, input.IndexOf(' '));
            input = input.Remove(0, input.IndexOf(" ") + 1);

            DiscordChannel channel = await Program.debugClient.GetChannelAsync(ulong.Parse(channelId));

            var message = await new DiscordMessageBuilder().WithContent(input).SendAsync(channel);
        }
    }
}
