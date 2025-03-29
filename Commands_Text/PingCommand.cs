using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;

namespace Acorn.Commands_Text
{
    public class PingCommand
    {
        [Command("ping")]
        public static async ValueTask TextOnlyAsync(TextCommandContext context)
        {
            
        }
    }
}
