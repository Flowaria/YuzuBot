using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuBot;
internal static class Embeds
{
    private static readonly Random s_RNG = new();
    private static readonly Color s_YuzuColor = new(255, 100, 105);

    public static Embed BuildYuzuStatus(string thumbnailURL, float memoryUsageInMB, string? alternativeTitle = null)
    {
        var embed = new EmbedBuilder()
            .WithTitle(alternativeTitle ?? "지금 저의 상태에요 선생님...")
            .WithColor(s_YuzuColor)
            .WithThumbnailUrl(thumbnailURL);
        embed.AddField("메모리 사용량", $"{memoryUsageInMB:0.000}MB");
        return embed.Build();
    }

    public static Embed BuildYuzuStatusJoke(string thumbnailURL)
    {
        var embed = new EmbedBuilder()
            .WithTitle("지금 저의 상태에요 선생님...!")
            .WithColor(s_YuzuColor)
            .WithThumbnailUrl(thumbnailURL);

        embed.AddField("음란도", $"{s_RNG.NextDouble():P}");
        return embed.Build();
    }

    public static Embed BuildDCInside(DCInsidePost post)
    {
        var embed = new EmbedBuilder()
            .WithAuthor(post.Writter)
            .WithColor(Color.Blue)
            .WithUrl(post.URL)
            .WithFooter($"👍{post.LikeCount} 👎{post.DislikeCount}")
            .WithDescription(post.Description.ToShorten(140, "⋯⋯."));

        embed.Title = post.TitleHead != null ? $"{post.TitleHead} {post.Title}" : post.Title;
        if (TryParseDCDate(post.UploadDate, out var uploadTime))
        {
            embed.Timestamp = uploadTime;
        }

        if (!string.IsNullOrEmpty(post.ThumbnailURL))
        {
            embed.ThumbnailUrl = post.ThumbnailURL;
        }

        return embed.Build();
    }

    public static Embed BuildLargeEmote(SocketGuildUser sender, Emote emote)
    {
        var embed = new EmbedBuilder()
            .WithAuthor(sender.GlobalName, sender.GetDisplayAvatarUrl())
            .WithImageUrl(emote.Url)
            .WithColor(sender.GetDisplayColor())
            .WithFooter(emote.Name);

        return embed.Build();
    }

    private static bool TryParseDCDate(string? uploadDate, out DateTime time)
    {
        if (string.IsNullOrEmpty(uploadDate))
        {
            time = DateTime.MinValue;
            return false;
        }

        return DateTime.TryParseExact(
            uploadDate,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out time);
    }
}

internal struct DCInsidePost
{
    public string URL;
    public string Title;
    public string? TitleHead;
    public string Description;
    public string Writter;
    public string LikeCount;
    public string DislikeCount;
    public string? UploadDate;
    public string? ThumbnailURL;
}
