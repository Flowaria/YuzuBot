namespace YuzuBot;
internal static class StringExtension
{
    public static string ToShorten(this string str, int maxLength, string? shortendEnding = null)
    {
        var length = str.Length;
        if (length > maxLength)
        {
            if (!string.IsNullOrEmpty(shortendEnding))
            {
                return string.Concat(str.AsSpan(0, maxLength), shortendEnding);
            }
            else
            {
                return str[..maxLength];
            }
        }
        return str;
    }

    public static bool ContainsIgnoreCase(this string source, string substring)
    {
        return source.Contains(substring, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool EqualsIgnoreCase(this string source, string str)
    {
        return source.Equals(str, StringComparison.InvariantCultureIgnoreCase);
    }
}
