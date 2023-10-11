using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuBot.Modules;

namespace YuzuBot.Interactions;
internal sealed class GachaMenuModule : InteractionModuleBase
{
    private const string ID_GachaRetryButton = "gacha-retry";
    private const string ID_GachaDeleteButton = "gacha-delete";
    private const string ID_GachaSelectMenu = "gacha-selectmenu";
    private const string ID_PickupGacha = "pickup";
    private const string ID_FestivalGacha = "festival";
    private const string ID_RegularGacha = "regular";

    private readonly static MessageComponent s_GachaMenuComps;
    private readonly static MessageComponent s_GachaRetryComps;

    static GachaMenuModule()
    {
        var selectMenu = new SelectMenuBuilder()
            .WithCustomId(ID_GachaSelectMenu)
            .WithPlaceholder("원하시는 가챠 종류를 선택해주세요")
            .AddOption("픽업 가챠", ID_PickupGacha, "픽업 캐릭터가 0.7%의 확률로 등장합니다.", Emotes.GachaPaper_2Star)
            .AddOption("페스 가챠", ID_FestivalGacha, "통상 3성 출현 확률이 2배가 되며 0.7% 확률로 픽업 캐릭터가 등장합니다.", Emotes.GachaPaper_3Star)
            .AddOption("상시 가챠", ID_RegularGacha, "픽업 캐릭터를 생각하지 않습니다.", Emotes.GachaPaper_1Star);

        var comps = new ComponentBuilder()
            .WithSelectMenu(selectMenu, row: 0);

        s_GachaMenuComps = comps.Build();

        comps = new ComponentBuilder()
            .WithButton("다시 돌리기", ID_GachaRetryButton, ButtonStyle.Primary)
            .WithButton("결과 삭제", ID_GachaDeleteButton, ButtonStyle.Danger);

        s_GachaRetryComps = comps.Build();
    }

    [SlashCommand("gacha-menu", "가챠 메뉴를 표시합니다.", runMode: RunMode.Async)]
    public async Task GachaCommand()
    {
        var embed = YuzuChatBox.Create("메뉴에요 선생님...", "원하시는 가챠를 골라주세요...");
        await RespondAsync(embed: embed.Build(), components: s_GachaMenuComps, ephemeral: true);
    }

    [ComponentInteraction("gacha-selectmenu")]
    public async Task GachaMenuSelected(string[] selected)
    {
        GachaResult gachaResult;
        EmbedBuilder embed;

        switch (selected[0])
        {
            case ID_PickupGacha:
                Gacha.CreatePickupResult(out gachaResult);
                Gacha.GetYuzuReactions(in gachaResult, out var message, out var expression, out var color);
                embed = YuzuChatBox.Create("픽업 10연차의 결과에요...!", message, expression, color);
                break;

            case ID_FestivalGacha:
                Gacha.CreateFestivalResult(out gachaResult);
                Gacha.GetYuzuReactions(in gachaResult, out message, out expression, out color);
                embed = YuzuChatBox.Create("페스 10연차의 결과에요...!", message, expression, color);
                break;

            case ID_RegularGacha:
                Gacha.CreateRegularResult(out gachaResult);
                Gacha.GetYuzuReactions(in gachaResult, out message, out expression, out color);
                embed = YuzuChatBox.Create("상시 10연차의 결과에요...!", message, expression, color);
                break;

            default:
                await RespondAsync("가챠 명령어를 처리 하는 중 문제가 발생했어요...", ephemeral: true);
                return;
        }
        embed.AddField("주인님:", Context.User.Mention, inline: true);
        embed.AddField("모집 포인트:", "10", inline: true);
        embed.AddField($"최대 누적 엘리그마:", $"{Resources.Emote_Eligma}x{gachaResult.EligmaCount}", inline: true);
        embed.WithFooter("/gacha-menu");
        
        await DeferAsync();
        await Context.Interaction.DeleteOriginalResponseAsync();

        await Context.Channel.SendMessageAsync(
            text: Gacha.BuildEmoteTextFromResult(in gachaResult),
            embed: embed.Build(),
            components: s_GachaRetryComps);
    }

    [ComponentInteraction("gacha-retry")]
    public async Task GachaRetryButton()
    {
        if (Context.Interaction is not SocketMessageComponent arg)
        {
            await RespondAsync("처리 중 예상치 못한 문제가 발생했어요...", ephemeral: true);
            return;
        }

        if (arg.Message.Embeds == null || arg.Message.Embeds.Count < 1)
        {
            await RespondAsync("처리 중 예상치 못한 문제가 발생했어요...", ephemeral: true);
            return;
        }

        if ((arg.Message.Embeds?.FirstOrDefault()?.Fields.First().Value) != arg.User.Mention)
        {
            await RespondAsync("이 가챠를 돌린 사람만 다시 돌릴 수 있어요...", ephemeral: true);
            return;
        }

        var origEmbed = arg.Message.Embeds.ElementAt(0);
        await arg.Message.ModifyAsync(async msg =>
        {
            var title = origEmbed.Title;
            var requester = origEmbed.Fields[0].Value;
            var count = int.Parse(origEmbed.Fields[1].Value);
            var eligmaCount = int.Parse(RX.GACHA_STAT_NUMBER.Match(origEmbed.Fields[2].Value).Value);

            EmbedBuilder embed;
            GachaResult gachaResult;
            if (title.Contains("페스"))
            {
                Gacha.CreateFestivalResult(out gachaResult);
                Gacha.GetYuzuReactions(in gachaResult, out var message, out var expression, out var color);
                embed = YuzuChatBox.Create("페스 10연차의 결과에요...!", message, expression, color);
                msg.Content = Gacha.BuildEmoteTextFromResult(in gachaResult);
            }
            else if (title.StartsWith("픽업"))
            {
                Gacha.CreatePickupResult(out gachaResult);
                Gacha.GetYuzuReactions(in gachaResult, out var message, out var expression, out var color);
                embed = YuzuChatBox.Create("픽업 10연차의 결과에요...!", message, expression, color);
                msg.Content = Gacha.BuildEmoteTextFromResult(in gachaResult);
            }
            else if (title.StartsWith("상시"))
            {
                Gacha.CreateRegularResult(out gachaResult);
                Gacha.GetYuzuReactions(in gachaResult, out var message, out var expression, out var color);
                embed = YuzuChatBox.Create("상시 10연차의 결과에요...!", message, expression, color);
                msg.Content = Gacha.BuildEmoteTextFromResult(in gachaResult);
            }
            else
            {
                await RespondAsync("처리 중 예상치 못한 문제가 발생했어요...", ephemeral: true);
                return;
            }

            embed.AddField("주인님:", arg.User.Mention, inline: true);
            embed.AddField("모집 포인트:", count + 10, inline: true);
            embed.AddField($"최대 누적 엘리그마:", $"{Resources.Emote_Eligma}x{gachaResult.EligmaCount + eligmaCount} (+{gachaResult.EligmaCount})", inline: true);
            embed.WithFooter("/gacha-menu");
            msg.Embeds = new Embed[] { embed.Build() };
        });
        await DeferAsync();
    }

    [ComponentInteraction("gacha-delete")]
    public async Task GachaDeleteButton()
    {
        if (Context.Interaction is not SocketMessageComponent arg)
        {
            await RespondAsync("처리 중 예상치 못한 문제가 발생했어요...", ephemeral: true);
            return;
        }

        if ((arg.Message.Embeds?.FirstOrDefault()?.Fields.First().Value) != arg.User.Mention)
        {
            await RespondAsync("이 가챠를 돌린 사람만 삭제 할 수 있어요...", ephemeral: true);
            return;
        }

        await arg.Message.DeleteAsync();
    }
}
