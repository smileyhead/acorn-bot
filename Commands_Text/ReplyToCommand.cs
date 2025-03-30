using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;

namespace Acorn.Commands_Text
{
    public static class ReplyToCommand
    {
        [Command("replyto"), AllowedProcessors<TextCommandProcessor>()]
        public static async ValueTask ExecuteAsync(CommandContext context, [RemainingText] string input)
        {
            if (context.User.Id == 164119349836120074)
            {
                ulong channelId = ulong.Parse(input.Substring(0, input.IndexOf(' ')));
                input = input.Remove(0, input.IndexOf(" ") + 1);

                ulong messageId = ulong.Parse(input.Substring(0, input.IndexOf(' ')));
                input = input.Remove(0, input.IndexOf(" ") + 1);

                DiscordChannel channel = await Program.debugClient.GetChannelAsync(channelId);

                var reply = await new DiscordMessageBuilder().WithContent(input).WithReply(messageId).SendAsync(channel);
            }
        }
    }
}
