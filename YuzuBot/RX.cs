﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YuzuBot;
internal static class RX
{
    public static readonly Regex HTTPURL = new("http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*(),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+", RegexOptions.IgnoreCase);
    public static readonly Regex DISCORD_SPOILER = new(@"\|\|(.*?)\|\|");
    public static readonly Regex DC_IMAGE = new(@"viewimage\.php\?.*");

    public static readonly Regex REPLACE_FIX_PIXIV = new(@"(?<=http[s]?:\/\/[a-zA-Z0-9]+\.|\/\/)pixiv\.net", RegexOptions.IgnoreCase);
    public static readonly Regex REPLACE_FIX_TWITTER = new(@"(?<=http[s]?:\/\/[a-zA-Z0-9]+\.|\/\/)twitter\.com", RegexOptions.IgnoreCase);
    public static readonly Regex REPLACE_FIX_X = new(@"(?<=http[s]?:\/\/[a-zA-Z0-9]+\.|\/\/)x\.com", RegexOptions.IgnoreCase);

    public static string ReplaceToFixEmbedURLS(string content)
    {
        var newStr = REPLACE_FIX_PIXIV.Replace(content, "ppxiv.net");
        newStr = REPLACE_FIX_TWITTER.Replace(newStr, "fxtwitter.com");
        return REPLACE_FIX_X.Replace(newStr, "fxtwitter.com");
    }

    public static bool TryFullMatch(this Regex regex, string input, out Match? match)
    {
        match = regex.Match(input);
        if (!match.Success)
        {
            match = null;
            return false;
        }

        if (match.Length != input.Length)
        {
            match = null;
            return false;
        }

        return true;
    }
}
