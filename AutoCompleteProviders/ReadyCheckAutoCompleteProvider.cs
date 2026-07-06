using Acorn.Records;
using DSharpPlus;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace Acorn.AutoCompleteProviders
{
    public class ReadyCheckAutoCompleteProvider : IAutoCompleteProvider
    {
        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
        {
            List<DiscordAutoCompleteChoice> types = new List<DiscordAutoCompleteChoice>();
            List<DiscordVoiceState> voiceStates = new List<DiscordVoiceState>(context.Guild?.VoiceStates.Values!) ?? [];
            List<DiscordVoiceState> distinctVoiceStates = new List<DiscordVoiceState>(voiceStates.DistinctBy(v => v.ChannelId)) ?? [];

            foreach (DiscordVoiceState voiceState in distinctVoiceStates)
            {
                int count = voiceStates.Count(v => v.ChannelId == voiceState.ChannelId);

                if (count == 1)
                    types.Add(new DiscordAutoCompleteChoice(
                        $"Ready check {context.Guild.Channels.First(c => c.Value.Id == voiceState.ChannelId).Value.Name} (1 user connected)",
                        voiceState.ChannelId.ToString()));
                else types.Add(new DiscordAutoCompleteChoice(
                    $"Ready check {context.Guild.Channels.First(c => c.Value.Id == voiceState.ChannelId).Value.Name} ({count} users connected)",
                    voiceState.ChannelId.ToString()));
            }
            
            types.Sort();
            types.Add(new DiscordAutoCompleteChoice("Generic ready check", "generic"));

            IEnumerable<DiscordAutoCompleteChoice> typesTask = types;
            return typesTask;
        }
    }
}