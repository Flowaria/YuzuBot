using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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

        using var webhookClient = await OpenWebhookClient(ch);
        IEnumerable<Embed>? embeds = embed != null ? new[] { embed } : null; 
        if (fileStream != null && filename != null)
        {
            return await webhookClient.SendFileAsync(fileStream, filename, msg, avatarUrl: copyUser.GetDisplayAvatarUrl(), username: copyUser.DisplayName, embeds: embeds);
        }
        else
        {
            return await webhookClient.SendMessageAsync(msg, avatarUrl: copyUser.GetDisplayAvatarUrl(), username: copyUser.DisplayName, embeds: embeds);
        }
    }

    private async Task<DiscordWebhookClient> OpenWebhookClient(IIntegrationChannel channel)
    {
        IWebhook targetWebhook = null!;
        var webhooks = await channel.GetWebhooksAsync();
        foreach (var wh in webhooks)
        {
            if (wh.Creator.Id == BotID)
            {
                targetWebhook = wh;
                break;
            }
        }

        targetWebhook ??= await channel.CreateWebhookAsync("YuzuProxy");
        return new DiscordWebhookClient(targetWebhook);
    }
}
