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
                new DiscordAutoCompleteChoice("Add Quote", "addquote"),
                new DiscordAutoCompleteChoice("Undo Last Quote", "undolastquote"),
                new DiscordAutoCompleteChoice("Text Commands", "textcommands"),
                new DiscordAutoCompleteChoice("/quote", "quote"),
                new DiscordAutoCompleteChoice("/specificquote", "specificquote"),
                new DiscordAutoCompleteChoice("/quoteby", "quoteby"),
                new DiscordAutoCompleteChoice("/searchquote", "searchquote"),
                new DiscordAutoCompleteChoice("/exchange", "exchange"),
                new DiscordAutoCompleteChoice("/character", "character"),
                new DiscordAutoCompleteChoice("/roll", "roll"),
                new DiscordAutoCompleteChoice("/flip", "flip"),
                new DiscordAutoCompleteChoice("/8ball", "8ball"),
                new DiscordAutoCompleteChoice("/convert", "convert")
            ];

            if (context.UserInput.Length < 3)
            {
                return ValueTask.FromResult(commands);
            }
            else
            {
                return ValueTask.FromResult(commands.Where(unit => unit.Name.ToLower().Contains(context.UserInput.ToLower())).Take(25));
            }
        }
    }
}
