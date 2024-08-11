using System.Text.RegularExpressions;

namespace GHRepoLetterStats.Common.ExtensionMethods;
public static class StringExtensionMethods
{
    public static bool EndsWith(this string value, string[] suffixes)
    {
        foreach (var suffix in suffixes)
        {
            if (value.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
                return true;
        }

        return false;
    }

    public static string RemoveSpecialCharacters(this string value)
    {
        return Regex.Replace(value, "[^a-zA-Z]", "");
    }

    public static string RemoveSpecialCharacters(this string value, string[] stringsToRemove)
    {
        var pattern = $"[^a-zA-Z{string.Join("", stringsToRemove)}]";
        return Regex.Replace(value, pattern, "");
    }
}
