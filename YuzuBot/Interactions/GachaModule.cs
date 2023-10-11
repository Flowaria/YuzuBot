using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuBot.Modules;

namespace YuzuBot.Interactions;
internal sealed class GachaModule : InteractionModuleBase
{
    [SlashCommand("gacha", "픽업 가챠를 기준으로 10연차를 돌립니다.", runMode: RunMode.Async)]
    public async Task GachaCommand()
    {
        Gacha.CreatePickupResult(out var result);
        Gacha.GetYuzuReactions(in result, out var message, out var expression, out var color);
        var embed = YuzuChatBox.Create("픽업 10연차의 결과에요...!", message, expression, color);
        embed.AddField($"엘리그마 환산:", $"{Resources.Emote_Eligma}x{result.EligmaCount}", inline: true);

        var paperEmotes = Gacha.BuildEmoteTextFromResult(in result);
        await RespondAsync(text: paperEmotes, embed: embed.Build());
    }
}
