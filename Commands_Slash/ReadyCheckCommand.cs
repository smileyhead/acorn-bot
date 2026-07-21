using Acorn.AutoCompleteProviders;
using Acorn.Classes;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System.ComponentModel;
using System.Diagnostics;
using DSharpPlus.Entities;
using Polly.Simmy.Fault;

namespace Acorn.Commands_Slash
{
    public class ReadyCheckCommand
    {
        [Command("readycheck"), Description("Initiate ready check.")]
        public static async ValueTask ExecuteAsync(CommandContext context,
            [Description("The type of ready check you wish to execute."),
             SlashAutoCompleteProvider<ReadyCheckAutoCompleteProvider>]
            string type)
        {
            await context.DeferResponseAsync();

            Console.WriteLine("Commencing ready check.");

            ulong channelId = 0;
            bool isGeneric = type == "generic";
            if (!isGeneric && !ulong.TryParse(type, out channelId))
            {
                await context.RespondAsync(
                    "I'm sorry, but the channel ID provided is not a valid unsigned long. Please choose an option from the auto-complete list.");
                return;
            }

            DiscordChannel channel;
            if (!isGeneric)
            {
                channel = await context.Guild.GetChannelAsync(channelId);
                if (channel.Users.Count == 0)
                {
                    await context.RespondAsync(
                        "I'm sorry, but you cannot start a ready check for a voice channel with no people connected. Perhaps, you could try a generic ready check.");
                    return;
                }
            }

            long deadline = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 60;
            List<DiscordUser> responseYes = [];
            List<DiscordUser> responseNo = [];
            List<DiscordUser>? responseUnknown = null;
            if (!isGeneric)
            {
                channel = await context.Guild.GetChannelAsync(channelId);
                responseUnknown = channel.Users.Where(u => !u.IsBot).ToList<DiscordUser>();
            }

            await context.RespondAsync(
                new DiscordMessageBuilder().AddEmbed(BuildReadyTableMessage(deadline, responseYes,
                    responseNo, responseUnknown)));
            var message = await context.GetResponseAsync();
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("✔️"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("✖️"));


            Task.Delay(4000).Wait(); //API usage optimisation by accounting for message arrival and human reaction time
            responseYes =
                message.GetReactionsAsync(DiscordEmoji.FromUnicode("✔️")).ToListAsync().Result;
            responseNo =
                message.GetReactionsAsync(DiscordEmoji.FromUnicode("✖️")).ToListAsync().Result;

            while (DateTimeOffset.UtcNow.ToUnixTimeSeconds() < deadline)
            {
                List<DiscordUser> responseYesNew =
                    message.GetReactionsAsync(DiscordEmoji.FromUnicode("✔️")).ToListAsync().Result.Where(u => !u.IsBot)
                        .ToList();
                List<DiscordUser> responseNoNew =
                    message.GetReactionsAsync(DiscordEmoji.FromUnicode("✖️")).ToListAsync().Result.Where(u => !u.IsBot)
                        .ToList();

                if (!new HashSet<DiscordUser>(responseYes).SetEquals(responseYesNew) ||
                    !new HashSet<DiscordUser>(responseNo).SetEquals(responseNoNew))
                {
                    responseYes = responseYesNew;
                    responseNo = responseNoNew;

                    await context.EditResponseAsync(new DiscordMessageBuilder().AddEmbed(BuildReadyTableMessage(
                        deadline, responseYes, responseNo, responseUnknown)));

                    if (responseUnknown != null)
                    {
                        List<DiscordUser> respondents = [];
                        respondents.AddRange(responseYes);
                        respondents.AddRange(responseNo);
                        if (respondents.Distinct().ToList().Count == responseUnknown.Count)
                        {
                            deadline = DateTimeOffset.UtcNow
                                .ToUnixTimeSeconds(); //Workaround so that the embed can change colour on the final draw
                            break;
                        }
                    }
                }

                Task.Delay(2000).Wait();
            }

            if (responseUnknown != null) //Convert non-respondents to negative responses
            {
                foreach (var user in responseUnknown.Where(user =>
                             responseYes.All(u => u.Id != user.Id) && responseNo.All(u => u.Id != user.Id)))
                    responseNo.Add(user);
            }

            await context.EditResponseAsync(
                new DiscordMessageBuilder().AddEmbed(BuildReadyTableMessage(deadline, responseYes, responseNo,
                    responseUnknown)));
        }

        private static DiscordEmbedBuilder BuildReadyTableMessage(long deadline,
            List<DiscordUser> responseYes, List<DiscordUser> responseNo,
            List<DiscordUser>? responseUnknown)

        {
            DiscordEmbedBuilder embed = new()
            {
                Title = "Ready Check"
            };

            responseYes.Sort((a, b) => a.Username.CompareTo(b.Username));
            responseNo.Sort((a, b) => a.Username.CompareTo(b.Username));
            responseUnknown?.Sort((a, b) => a.Username.CompareTo(b.Username));

            if (responseUnknown != null)
            {
                embed.Description = responseYes.Count == 1
                    ? $"1/{responseUnknown.Count} respondent\n\n"
                    : $"{responseYes.Count}/{responseUnknown.Count} respondents\n\n";
                embed.Description += BuildList(responseUnknown);
            }
            else
            {
                List<DiscordUser> respondents = [];
                respondents.AddRange(responseYes);
                if (responseNo.Count > 0) respondents.AddRange(responseNo);
                embed.Description = respondents.Distinct().ToList().Count switch
                {
                    0 => "No respondents yet\n\n",
                    1 => "1 respondent\n\n",
                    _ => $"{respondents.Count} respondents\n\n"
                };
                embed.Description += BuildList(respondents.Distinct().ToList());
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() < deadline)
            {
                embed.Description +=
                    $"\nA **ready check** has been commenced.\nInform your comrades that you are ready?\n\\*This notice expires <t:{deadline}:R>.";
                embed.Color = DiscordColor.Yellow;
            }
            else if (responseUnknown != null)
            {
                embed.Description +=
                    $"\nReady check complete.\nReady: {responseYes.Count}/{responseUnknown.Count}\nNot Ready: {responseNo?.Except(responseYes).ToList().Count ?? 0}/{responseUnknown.Count}";
                embed.Color = responseNo?.Except(responseYes).ToList() is { Count: > 0 }
                    ? DiscordColor.Red
                    : DiscordColor.Green;
            }
            else
            {
                embed.Description +=
                    $"\nReady check complete.\nReady: {responseYes.Count}\nNot Ready: {responseNo?.Except(responseYes).ToList().Count ?? 0}";
                embed.Color = responseNo?.Except(responseYes).ToList() is { Count: > 0 }
                    ? DiscordColor.Red
                    : DiscordColor.Green;
            }

            return embed;

            string BuildList(List<DiscordUser> users)
            {
                string answer = "";
                foreach (DiscordUser user in users)
                {
                    string usedName = user.GlobalName == "" ? user.Username : user.GlobalName;
                    if (responseYes.Contains(user))
                    {
                        answer += $"✔️ {usedName}\n";
                    }
                    else if (responseNo.Contains(user))
                    {
                        answer += $"️✖️ {usedName}\n";
                    }
                    else if (responseUnknown != null && responseUnknown.Contains(user))
                    {
                        answer += $"️<:blank:1522191751635275896> {usedName}\n";
                    }
                }

                return answer;
            }
        }
    }
}