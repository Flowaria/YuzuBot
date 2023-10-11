using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using System.Text;
using System.Web;
using YuzuBot.Modules;

namespace YuzuBot;
internal partial class YuzuBot
{
    private async Task ProcessURL(IMessage context, SocketGuildUser author, string url)
    {
        if (url.ContainsIgnoreCase("dcinside.com/"))
        {
            await ProcessURL_DCInside(context, author, url);
        }
    }

    private async Task ProcessURL_DCInside(IMessage context, SocketGuildUser _, string url)
    {
        using var ms = new MemoryStream();
        if (!(await TryGetContentFromURL(url, outputStream: ms)).Success)
            return;

        using var sr = new StreamReader(ms, Encoding.UTF8);
        var content = await sr.ReadToEndAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(content);

        var root = doc.DocumentNode;
        var title = HttpUtility.HtmlDecode(root.SelectSingleNode("//head/title").GetDirectInnerText()); //타이틀
        var titleHead = root.SelectSingleNode("//span[@class='title_headtext']")?.InnerText ?? null;
        var contentNode = root.SelectSingleNode("//div[@class='view_content_wrap']");
        var writter = contentNode.SelectSingleNode("//div[@class='gall_writer ub-writer']");
        var writterNick = writter.Attributes["data-nick"].Value;
        var writterUID = string.Empty;
        if (writter.Attributes.Contains("data-ip"))
        {
            writterUID += writter.Attributes["data-ip"].Value;
        }
        if (writter.Attributes.Contains("data-uid"))
        {
            writterUID += writter.Attributes["data-uid"].Value;
        }
        writterNick += $" ({writterUID})";

        var uploadDate = contentNode.SelectSingleNode("//span[@class='gall_date']").Attributes["title"].Value; //추천수
        var like = contentNode.SelectSingleNode("//p[contains(@id, 'recommend_view_up_')]").InnerText; //추천수
        var dislike = contentNode.SelectSingleNode("//p[@class='down_num']").InnerText; //비추수

        var viewNode = contentNode.SelectSingleNode("//div[@class='writing_view_box']");

        //Remove Items from DC Series (Except Title)
        var seriesBoxes = viewNode.SelectNodes("//div[@class='dc_series']");
        if (seriesBoxes != null)
        {
            foreach (var child in seriesBoxes.SelectMany(x => x.ChildNodes).ToArray())
            {
                if (child.Name == "a")
                    child.Remove();
            }
        }

        //Remove External Video Area
        var externalVids = viewNode.SelectNodes("//div[@class='og-div']");
        if (externalVids != null)
        {
            foreach (var child in externalVids.ToArray())
            {
                child.Remove();
            }
        }

        //Remove DC App Footer
        var dcappfooter = viewNode.SelectSingleNode("//span[@id='dcappfooter']");
        dcappfooter?.Remove();

        var texts = HttpUtility.HtmlDecode(viewNode.InnerText).Trim().ReplaceLineEndings(" ");
        var embedInfo = new DCInsidePost()
        {
            URL = url,
            Title = title,
            TitleHead = titleHead,
            Description = texts,
            LikeCount = like,
            DislikeCount = dislike,
            UploadDate = uploadDate,
            Writter = writterNick,
            ThumbnailURL = Resources.DCThumnail
        };

        var images = contentNode.SelectNodes("//img");
        if (images != null)
        {
            Stream? embedFileStream = null;
            foreach (var img in images)
            {
                var src = img.Attributes["src"].Value;
                if (!src.ContainsIgnoreCase("dcinside.co.kr/viewimage.php?"))
                    continue;

                var match = RX.DC_IMAGE.Match(src);
                if (match == null)
                    continue;

                var imgUrl = $"https://images.dcinside.com/{match.Value}";
                using var imgMs = new MemoryStream();

                var (success, fileName) = await TryGetContentFromURL(imgUrl, imgMs);
                if (success && fileName != null)
                {
                    embedFileStream = imgMs;
                    embedInfo.ThumbnailURL = $"attachment://{fileName}";
                    await context.Channel.SendFileAsync(embedFileStream, fileName, embed: Embeds.BuildDCInside(embedInfo),
                        messageReference: new MessageReference(context.Id),
                        flags: MessageFlags.SuppressNotification,
                        allowedMentions: AllowedMentions.None);
                    return;
                }
            }
        }

        await context.Channel.SendMessageAsync(embed: Embeds.BuildDCInside(embedInfo),
            messageReference: new MessageReference(context.Id),
            flags: MessageFlags.SuppressNotification,
            allowedMentions: AllowedMentions.None);
    }

    private static async Task<(bool Success, string? FileName)> TryGetContentFromURL(string url, Stream outputStream)
    {
        using var wc = new HttpClient();
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        var headers = req.Headers;
        headers.Add("Connection", "keep-alive");
        headers.Add("Cache-Control", "no-cache");
        headers.Add("sec-ch-ua-mobile", "?0");
        headers.Add("DNT", "1");
        headers.Add("Upgrade-Insecure-Requests", "1");
        headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");
        headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
        headers.Add("Sec-Fetch-Site", "none");
        headers.Add("Sec-Fetch-Mode", "navigate");
        headers.Add("Sec-Fetch-User", "?1");
        headers.Add("Sec-Fetch-Dest", "document");
        headers.Add("Accept-Encoding", "gzip, deflate, br");
        headers.Add("Accept-Language", "ko-KR,ko;q=0.9");

        using var res = await wc.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        if (res.IsSuccessStatusCode)
        {
            var prevPos = outputStream.Position;
            await res.Content.CopyToAsync(outputStream);
            outputStream.Position = prevPos;
            return (true, res.Content.Headers.ContentDisposition?.FileName ?? null);
        }
        else
        {
            LogError($"ERROR: Response code was {res.StatusCode} ({(int)res.StatusCode})! [url: {url}]");
            return (false, null);
        }
    }
}
