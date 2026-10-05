using Acorn.Records;
using DSharpPlus;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace Acorn.AutoCompleteProviders
{
    public class PortalToAutoCompleteProvider : IAutoCompleteProvider
    {
        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            List<DiscordChannel> channels =
            [
                .. context.Guild?.Channels.Values.Where(ch => ch.Type.Equals(DiscordChannelType.Text)) ?? []
            ];

            channels.Remove(channels.First(ch => ch.Id == context.Channel.Id));
            channels = [.. channels.OrderBy(ch => ch.Position)];

            List<DiscordAutoCompleteChoice> choices =
            [
                .. channels.Select(channel => new DiscordAutoCompleteChoice($"#{channel.Name}", channel.Id.ToString()))
            ];

            IEnumerable<DiscordAutoCompleteChoice> typesTask = context.UserInput.Length < 2
                ? choices.Take(25)
                : choices.Where(choice =>
                    choice.Name.Contains(context.UserInput, StringComparison.CurrentCultureIgnoreCase)).Take(25);
            return typesTask;
        }
    }
}