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
    private static readonly Color s_YuzuColor = new(255, 100, 105);
    private static readonly Color s_GachaBlueColor = new(37, 190, 255);
    private static readonly Color s_GachaYellowColor = new(255, 244, 37);
    private static readonly Color s_GachaPinkColor = new(255, 90, 195);

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

        embed.AddField("음란도", $"{Random.Shared.NextDouble():P}");
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

    public static Embed BuildGachaResult(out string gachaResult)
    {
        gachaResult = "";

        var rng = Random.Shared;
        var result3StarCount = 0;
        var result2StarCount = 0;
        for (int i = 0; i<10; i++)
        {
            if (i == 5)
                gachaResult += '\n';

            var result = rng.NextDouble();
            if (result <= 0.03f)
            {
                gachaResult += Resources.Emote_Gacha3Star;
                result3StarCount++;
            }
            else if (result <= 0.185f || i == 9)
            {
                gachaResult += Resources.Emote_Gacha2Star;
                result2StarCount++;
            }
            else
            {
                gachaResult += Resources.Emote_Gacha1Star;
            }
        }


        var embed = new EmbedBuilder()
            .WithTitle("10연차 결과에요...!");


        if (result3StarCount >= 2)
        {
            embed.Description = $"3성이 {result3StarCount}개나..? 굉장해요!";
            embed.ThumbnailUrl = Resources.YuzuImage_Smile;
            embed.Color = s_GachaPinkColor;
        }
        else if(result3StarCount >= 1)
        {
            embed.Description = $"3성이 나왔어요...! 오늘 하루는 운이 좋을거... 같아요...";
            embed.ThumbnailUrl = Resources.YuzuImage_SmileSmall;
            embed.Color = s_GachaPinkColor;
        }
        else if (result2StarCount >= 2)
        {
            embed.Description = $"운이 안좋았네요...";
            embed.ThumbnailUrl = Resources.YuzuImage_A;
            embed.Color = s_GachaYellowColor;
        }
        else
        {
            embed.Description = $"올블루라니... 그런...";
            embed.ThumbnailUrl = Resources.YuzuImage_Cry;
            embed.Color = s_GachaBlueColor;
        }

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
