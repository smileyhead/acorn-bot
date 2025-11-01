using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Trees.Metadata;

namespace Acorn.Commands_Text
{
    public class ShitpostReply
    {
        public string GetReply(string name)
        {
            switch (name)
            {
                case "alcohol": return "https://media.discordapp.net/attachments/1354563761293492506/1354563821578092675/alcohol.png";
                case "ban": return "Night.";
                case "coffee": return "https://tenor.com/view/any-body-need-coffee-coffee-gif-15911994";
                case "creature": return "https://cdn.discordapp.com/attachments/1354563761293492506/1355650421427015790/image.png";
                case "helpme": return "https://cdn.discordapp.com/attachments/1337097452859428877/1338829503711150080/guy_screaming_for_help_while_holding_a_chick_fil_a_drink.mp4";
                case "horror": return "https://media.discordapp.net/attachments/1354563761293492506/1354563901668327484/horror_toby.png";
                case "no": return "https://media.discordapp.net/attachments/1354563761293492506/1354563944114684075/no.jpg";
                case "ő": return "https://media.discordapp.net/attachments/1354563761293492506/1355655288006639666/image0.png";
                case "pirate": return "https://media.discordapp.net/attachments/1354563761293492506/1354564017263476776/pirate.jpg";
                case "q": return "**Add a quote:** Right-click *(or tap and hold)* → Apps → Add Quote.\n**Undo the most recent quote:** Right-click *(or tap and hold)* → Apps → Undo Last Quote.";
                case "selfie": return "This is my router hat and I! (Yes, I share a home with Home Assistant. The label came before me.)\nhttps://cdn.discordapp.com/attachments/1354563761293492506/1355654632805765160/PXL_20250329_212641750.jpg";
                case "steeve": return "https://cdn.discordapp.com/attachments/1354563761293492506/1355510311070728333/Steeve.mov";
                case "stop": return "https://media.discordapp.net/attachments/1354563761293492506/1354564769641660486/stop_miku.gif";
                case "wakeup": return "https://tenor.com/view/i-aint-get-no-sleep-banging-pots-pans-gif-11664925";
                default: return "Invalid name.";
            }
        }
    }

    public class AlcoholShCommand { [Command("alcohol")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("alcohol")); }
    public class BanShCommand { [Command("ban")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("ban")); }
    public class CreatureShCommand { [Command("creature")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("creature")); }
    public class CoffeeShCommand { [Command("coffee")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("coffee")); }
    public class HelpShCommand { [Command("helpme")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("helpme")); }
    public class HorrorShCommand { [Command("horror")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("horror")); }
    public class NoShCommand { [Command("no")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("no")); }
    public class ŐShCommand { [Command("o"), TextAlias("ő")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("ő")); }
    public class PirateShCommand { [Command("pirate")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("pirate")); }
    public class QuoteShCommand { [Command("q")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("q")); }
    public class SelfieShCommand { [Command("selfie")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("selfie")); }
    public class SteeveShCommand { [Command("steeve")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("steeve")); }
    public class StopShCommand { [Command("stop")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("stop")); }
    public class WakeupShCommand { [Command("wakeup")] public static async ValueTask TextOnlyAsync(TextCommandContext context) => await context.RespondAsync(new ShitpostReply().GetReply("wakeup")); }
}
