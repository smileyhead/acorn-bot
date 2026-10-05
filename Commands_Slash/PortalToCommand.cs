using DSharpPlus.Commands;
using System.ComponentModel;
using Acorn.AutoCompleteProviders;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace Acorn.Commands_Slash
{
    public class PortalToCommand
    {
        [Command("portalto"), Description("Opens a bidirectional portal to a desired channel.")]
        public static async ValueTask ExecuteAsync(CommandContext context,
            [Description("The destination channel, or its link or ID."),
             SlashAutoCompleteProvider<PortalToAutoCompleteProvider>]
            string destination)
        {
            await context.DeferResponseAsync();

            Console.WriteLine("  Opening a portal.");

            destination = destination.Replace(" ", "");
            if (destination.EndsWith('/')) destination = destination[..^1];
            
            string answer;
            ulong channelId;
            DiscordChannel? destinationChannel;

            if (destination.Contains("discord") && destination.Count('/') == 6)
            {
                await context.RespondAsync($"<a:BlackHole:1556347101183737997> One-way portal to {destination}\n-# Sorry, but I can't create two-way portals with a message link. Please use a channel link next time.");
                return;
            }

            if (!ulong.TryParse(destination, out channelId))
            {
                string extractedUlongMaybe = destination[(destination.LastIndexOf('/') + 1)..];

                if (!ulong.TryParse(extractedUlongMaybe, out channelId) &&
                    !ulong.TryParse([.. destination.Where(char.IsDigit)], out channelId))
                {
                    await context.RespondAsync("Sorry, but the ID received doesn't seem valid.");
                    Console.WriteLine(
                        $"  Error: invalid channel ID.\n   destination: {destination}\n   extractedUlongMaybe: {extractedUlongMaybe}");
                    return;
                }
            }

            try
            {
                destinationChannel = await context.Guild!.GetChannelAsync(channelId);
            }
            catch (NotFoundException)
            {
                await context.RespondAsync(
                    $"<a:BlackHole:1556347101183737997> One-way portal to <#{channelId}>\n-# This portal leads to a channel I don't have access to, or is on a different server.");
                return;
            }


            // ReSharper disable once JoinDeclarationAndInitializer
            // ReSharper doesn't consider preprocessor directives, apparently
            DiscordMember me;
#if DEBUG
            me = await context.Guild!.GetMemberAsync(1355501247725178930);
#else
            me = await context.Guild!.GetMemberAsync(1335008063589191721);
#endif

            if (!destinationChannel.PermissionsFor(me).HasPermission(DiscordPermission.SendMessages))
            {
                await context.RespondAsync(
                    $"<a:BlackHole:1556347101183737997> One-way portal to <#{channelId}>\n-# This portal leads to a channel I don't have message-sending permissions in.");
                return;
            }

            if (!destinationChannel.PermissionsFor(context.Member!).HasPermission(DiscordPermission.SendMessages))
            {
                await context.RespondAsync(
                    $"<a:BlackHole:1556347101183737997> One-way portal to <#{channelId}>\n-# Sorry, but this portal leads to a channel you don't have message-sending permissions in, so I refrained from sending one as an anti-spam measure.");
                return;
            }

            await context.RespondAsync(
                $"Opening a portal to <#{channelId}>…");

            DiscordMessage? response = await context.GetResponseAsync();

            if (response == null) return;

            await destinationChannel.SendMessageAsync(
                $"<a:OrangePortal:1556344846141358240> Portal from {response.JumpLink}");

            DiscordMessage? destinationMessage =
                await destinationChannel.GetMessagesAsync(10).Where(a => a.Author!.Equals(me)).FirstAsync();

            await context.EditResponseAsync(
                $"<a:BluePortal:1556344789090177094> Portal to {destinationMessage.JumpLink}");
        }
    }
}