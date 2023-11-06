using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuBot.Modules;

namespace YuzuBot.Interactions;

internal enum PageButtonAction
{
    Start,
    Previous,
    Next,
    End,
    PageMenu,
    None
}

internal class AnimatedEmoteModule : InteractionModuleBase
{
    private const string ID_AEmoteButton = "aemote-button;";
    private const string ID_AEmotePageButton = $"aemnote-pagebutton;";
    private const string ID_AEmotePageButton_Start = $"aemnote-pagebutton;{nameof(PageButtonAction.Start)}";
    private const string ID_AEmotePageButton_Prev = $"aemnote-pagebutton;{nameof(PageButtonAction.Previous)}";
    private const string ID_AEmotePageButton_Next = $"aemnote-pagebutton;{nameof(PageButtonAction.Next)}";
    private const string ID_AEmotePageButton_End = $"aemnote-pagebutton;{nameof(PageButtonAction.End)}";
    private const string ID_AEmotePageButton_PageMenu = $"aemnote-pagebutton;{nameof(PageButtonAction.PageMenu)}";
    private const string ID_AEmotePageButton_None = $"aemnote-pagebutton;{nameof(PageButtonAction.None)}";

    [SlashCommand("ae", "이 서버의 움짤콘 목록을 표시합니다.", runMode: RunMode.Async)]
    public async Task EmoteCommand()
    {
        if (Context.Guild == null)
        {
            await RespondAsync("이 커맨드는 서버 안에서만 사용 할 수 있어요...");
            return;
        }
        var comps = new ComponentBuilder();
        var emotes = Context.Guild.Emotes;
        var length = emotes.Count;
        var row = 0;
        var count = 0;
        for (int i = 0; i<length; i++)
        {
            var emote = emotes.ElementAt(i);
            if (!emote.Animated)
                continue;

            comps.WithButton(customId: ID_AEmoteButton+emote.ToString(), style: ButtonStyle.Secondary, emote: emote, row: row);
            count++;

            if (count >= 5)
            {
                count = 0;
                row++;
            }
        }

        //Controls
        comps.WithButton(customId: ID_AEmotePageButton_Start, style: ButtonStyle.Primary, emote: Emotes.LeftEndArrow, row: 4);
        comps.WithButton(customId: ID_AEmotePageButton_Prev, style: ButtonStyle.Primary, emote: Emotes.LeftArrow, row: 4);
        comps.WithButton(label: "99/99", customId: ID_AEmotePageButton_PageMenu, style: ButtonStyle.Primary, row: 4);
        comps.WithButton(customId: ID_AEmotePageButton_Next, style: ButtonStyle.Primary, emote: Emotes.RightArrow, row: 4);
        comps.WithButton(customId: ID_AEmotePageButton_End, style: ButtonStyle.Primary, emote: Emotes.RightEndArrow, row: 4);
        await RespondAsync(components: comps.Build(), ephemeral: true);
    }

    [ComponentInteraction(ID_AEmotePageButton + "*")]
    public async Task PageButton(string optStr)
    {
        if (!Enum.TryParse<PageButtonAction>(optStr, out var opt))
        {
            await RespondAsync("처리 중 예상치 못한 문제가 발생했어요...", ephemeral: true);
            return;
        }

        await RespondAsync($"선택 액션: {opt}", ephemeral: true);
        //await DeferAsync();
    }

    [ComponentInteraction(ID_AEmoteButton + "*")]
    public async Task SubmitEmoteButton(string emoteStr)
    {
        if (Context.User is not SocketGuildUser user)
        {
            await DeferAsync();
            return;
        }

        var embed = Embeds.BuildLargeEmote(user, Emote.Parse(emoteStr));
        await Context.Channel.SendMessageAsync(embed: embed);
        await DeferAsync();
    }
}
