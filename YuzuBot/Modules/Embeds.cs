using Discord;
using Discord.WebSocket;
using System.Globalization;

namespace YuzuBot.Modules;

internal static class Embeds
{
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
            embed.Timestamp = new DateTimeOffset(uploadTime, new TimeSpan(hours: 9, minutes: 0, seconds: 0));
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
            .WithAuthor(sender.DisplayName, sender.GetDisplayAvatarUrl())
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
