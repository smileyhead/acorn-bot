using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace Acorn.AutoCompleteProviders
{
    public class HelpCommandAutoCompleteProvider : IAutoCompleteProvider
    {
        public ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            IEnumerable<DiscordAutoCompleteChoice> commands =
            [
                new DiscordAutoCompleteChoice("/quote", "quote"),
                new DiscordAutoCompleteChoice("/specificquote", "specificquote"),
                new DiscordAutoCompleteChoice("/searchquote", "searchquote"),
                new DiscordAutoCompleteChoice("/character", "character"),
                new DiscordAutoCompleteChoice("/roll", "roll"),
                new DiscordAutoCompleteChoice("/flip", "flip"),
                new DiscordAutoCompleteChoice("/convert", "convert")
            ];

            return ValueTask.FromResult(commands);
        }
    }
}
