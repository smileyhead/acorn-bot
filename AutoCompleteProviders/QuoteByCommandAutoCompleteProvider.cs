using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace Acorn.AutoCompleteProviders
{
    public class QuoteByCommandAutoCompleteProvider : IAutoCompleteProvider
    {
        public ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            List<DiscordAutoCompleteChoice> authors = new List<DiscordAutoCompleteChoice>();
            List<User> users = Program.quotesList.Users.OrderBy(n => n.Name).ToList();

            for (int i = 0; i < users.Count; i++) { authors.Add(new DiscordAutoCompleteChoice($"{users[i].Name} ({users[i].QuoteCount} quotes)", $"{users[i].Id}")); }

            IEnumerable<DiscordAutoCompleteChoice> authorsTask = authors;

            if (context.UserInput.Length < 3)
            {
                return ValueTask.FromResult(authorsTask);
            }
            else
            {
                return ValueTask.FromResult(authorsTask.Where(unit => unit.Name.ToLower().Contains(context.UserInput.ToLower())).Take(25));
            }
        }
}
}
