using Discord;

namespace YuzuBot.Modules;
internal static class Emotes
{
    public static readonly IEmote GachaPaper_1Star = Emote.Parse(Resources.Emote_Gacha1Star);
    public static readonly IEmote GachaPaper_2Star = Emote.Parse(Resources.Emote_Gacha2Star);
    public static readonly IEmote GachaPaper_3Star = Emote.Parse(Resources.Emote_Gacha3Star);

    public static readonly Emoji RepeatButton = Emoji.Parse("🔁");
    public static readonly Emoji RedCross = Emoji.Parse("❌");
    public static readonly Emoji RightArrow = Emoji.Parse("▶️");
    public static readonly Emoji LeftArrow = Emoji.Parse("◀️");
    public static readonly Emoji RightEndArrow = Emoji.Parse("⏭️");
    public static readonly Emoji LeftEndArrow = Emoji.Parse("⏮️");
}
