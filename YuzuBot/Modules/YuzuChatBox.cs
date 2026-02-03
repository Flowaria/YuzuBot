using Discord;

namespace YuzuBot.Modules;

internal struct YuzuChatBox
{
    public static readonly Color DefaultColor = new(255, 100, 105);

    public YuzuExpression Expression;
    public string? Title;
    public string? Message;
    public Color Color;

    public static EmbedBuilder Create(string? title = null, string? message = null, YuzuExpression expression = YuzuExpression.Default, Color? color = null)
    {
        var chatBox = new YuzuChatBox(title, message, expression, color);
        return chatBox.ToEmbedBuilder();
    }

    public YuzuChatBox(string? title = null, string? message = null, YuzuExpression expression = YuzuExpression.Default, Color? color = null)
    {
        Title = title;
        Message = message;
        Expression = expression;
        Color = color ?? DefaultColor;
    }

    public readonly EmbedBuilder ToEmbedBuilder()
    {
        var embed = new EmbedBuilder()
            .WithTitle(Title)
            .WithDescription(Message)
            .WithColor(Color);

        embed.ThumbnailUrl = Expression switch
        {
            YuzuExpression.Default => Resources.ResourceBaseURL + Resources.YuzuImage_Default,
            YuzuExpression.A => Resources.ResourceBaseURL + Resources.YuzuImage_A,
            YuzuExpression.Smile => Resources.ResourceBaseURL + Resources.YuzuImage_Smile,
            YuzuExpression.SmallSmile => Resources.ResourceBaseURL + Resources.YuzuImage_SmileSmall,
            YuzuExpression.Cry => Resources.ResourceBaseURL + Resources.YuzuImage_Cry,
            YuzuExpression.Despair => Resources.ResourceBaseURL + Resources.YuzuImage_Despair,
            YuzuExpression.Determined => Resources.ResourceBaseURL + Resources.YuzuImage_Determined,
            YuzuExpression.Fear => Resources.ResourceBaseURL + Resources.YuzuImage_Fear,
            YuzuExpression.Kimo => Resources.ResourceBaseURL + Resources.YuzuImage_Kimo,
            YuzuExpression.Mataku => Resources.ResourceBaseURL + Resources.YuzuImage_Mataku,
            _ => Resources.ResourceBaseURL + Resources.YuzuImage_Default,
        };

        return embed;
    }
}

internal enum YuzuExpression
{
    Default,
    A,
    Smile,
    SmallSmile,
    Cry,
    Despair,
    Determined,
    Fear,
    Kimo,
    Mataku
}
