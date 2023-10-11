using Discord;
using Discord.WebSocket;
using System.Diagnostics;
using System.Text.RegularExpressions;
using YuzuBot.Modules;

namespace YuzuBot;
internal partial class YuzuBot
{
    private Task MessageReceived(SocketMessage arg)
    {
        _ = Task.Run(async () => { await HandleMessage(arg); });
        return Task.CompletedTask;
    }

    private async Task HandleMessage(SocketMessage arg)
    {
        try
        {
            if (arg.Author.IsBot)
                return;

            if (arg.Author.IsWebhook)
                return;

            if (arg.Author is not SocketGuildUser author)
                return;

            if (arg is not SocketUserMessage msg)
                return;

            var trimmedContent = msg.Content.Trim();

            //Stats
            if (trimmedContent.Equals(BotMentionString))
            {
                GC.Collect();
                await ShowStatus(arg);
                return;
            }

            //Scale-Up Custom Emote
            if (Emote.TryParse(trimmedContent, out var emote))
            {
                _ = arg.DeleteAsync();
                await arg.Channel.SendMessageAsync(embed: Embeds.BuildLargeEmote(author, emote),
                    messageReference: arg.Reference,
                    flags: MessageFlags.SuppressNotification,
                    allowedMentions: AllowedMentions.All);
                return;
            }

            //URL Embed
            var https = RX.HTTPURL.Matches(RX.DISCORD_SPOILER.Replace(trimmedContent, string.Empty));
            if (https.Count > 0)
            {
                //Fix-up URL Service (fxtwitter.com, fixupx.com, ppxiv.net)
                IMessage contextMessage = msg;
                var fixedMessage = RX.ReplaceToFixEmbedURLS(trimmedContent);
                if (!fixedMessage.EqualsIgnoreCase(trimmedContent))
                {
                    _ = msg.DeleteAsync();
                    var messageID = await SendWebhook(fixedMessage, msg.Channel, author);
                    contextMessage = await msg.Channel.GetMessageAsync(messageID);
                }

                //Manual Embed Services
                foreach (Match url in https.Cast<Match>())
                {
                    await ProcessURL(contextMessage, author, url.Value);
                }
            }
        }
        catch (Exception e)
        {
            LogError("Exception Was Thrown", e);
        }
    }

    private static async Task ShowStatus(SocketMessage arg)
    {
        using var process = Process.GetCurrentProcess();
        var rng = Random.Shared;
        if (rng.Next(69) == 0)
        {
            var embed = YuzuChatBox.Create("지금 저의 상태에요 선생님...!", expression: YuzuExpression.Smile);
            embed.AddField("음란도", $"{Random.Shared.NextDouble():P}");

            var jokeMessage = await arg.Channel.SendMessageAsync(embed: embed.Build(),
                messageReference: arg.Reference,
                flags: MessageFlags.SuppressNotification,
                allowedMentions: AllowedMentions.None);

            await Task.Delay(3000);

            embed = YuzuChatBox.Create("방금 그건 잊어주세요!!! 이게 제 상태에요!!", expression: YuzuExpression.Despair);
            embed.AddField("메모리 사용량", $"{process.WorkingSet64 / (1024.0f * 1024.0f):0.0}MB");

            await jokeMessage.ModifyAsync(p =>
            {
                p.Embed = embed.Build();
            });
        }
        else
        {
            var embed = YuzuChatBox.Create("지금 저의 상태에요 선생님...", expression: YuzuExpression.Default);
            embed.AddField("메모리 사용량", $"{process.WorkingSet64 / (1024.0f * 1024.0f):0.0}MB");

            await arg.Channel.SendMessageAsync(embed: embed.Build(),
                messageReference: arg.Reference,
                flags: MessageFlags.SuppressNotification,
                allowedMentions: AllowedMentions.None);
        }
    }
}
