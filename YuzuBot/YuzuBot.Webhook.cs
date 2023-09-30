using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuBot;
internal partial class YuzuBot
{
    private async Task<ulong> SendWebhook(string msg, IChannel channel, SocketGuildUser copyUser, Stream? fileStream = null, string? filename = null, Embed? embed = null)
    {
        if (channel is not IIntegrationChannel ch)
        {
            return 0;
        }

        var webhooks = await ch.GetWebhooksAsync();
        IWebhook targetWebhook = null!;
        foreach (var wh in webhooks)
        {
            if (wh.Creator.Id == BotID)
            {
                targetWebhook = wh;
                break;
            }
        }

        targetWebhook ??= await ch.CreateWebhookAsync("YuzuProxy");
        using DiscordWebhookClient wbc = new(targetWebhook);

        IEnumerable<Embed>? embeds = embed != null ? new[] { embed } : null; 
        if (fileStream != null && filename != null)
        {
            return await wbc.SendFileAsync(fileStream, filename, msg, avatarUrl: copyUser.GetDisplayAvatarUrl(), username: copyUser.DisplayName, embeds: embeds);
        }
        else
        {
            return await wbc.SendMessageAsync(msg, avatarUrl: copyUser.GetDisplayAvatarUrl(), username: copyUser.DisplayName, embeds: embeds);
        }
    }
}
