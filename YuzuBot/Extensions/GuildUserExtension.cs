using Discord;
using Discord.WebSocket;

namespace YuzuBot;
internal static class GuildUserExtension
{
    public static Color GetDisplayColor(this SocketGuildUser user)
    {
        return user.GetDisplayColor(Color.Default);
    }

    public static Color GetDisplayColor(this SocketGuildUser user, Color defaultColor)
    {
        SocketRole? toprole = null;
        foreach (var role in user.Roles.OrderBy(x=>x.Position))
        {
            if (role.Color != Color.Default)
                toprole = role;
        }

        if (toprole == null)
        {
            return defaultColor;
        }
        else
        {
            return toprole.Color;
        }
    }
}
