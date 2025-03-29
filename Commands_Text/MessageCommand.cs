using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;

namespace Acorn.Commands_Text
{
    public static class MessageCommand
    {
        [Command("message")]
        public static async ValueTask TextOnlyAsync(TextCommandContext context, [RemainingText] string input)
        {
            ulong channelId = ulong.Parse(input.Substring(0, input.IndexOf(' ')));
            input = input.Remove(0, input.IndexOf(" ") + 1);

            DiscordChannel channel = await Program.debugClient.GetChannelAsync(channelId);

            var message = await new DiscordMessageBuilder().WithContent(input).SendAsync(channel);
        }
    }
}
