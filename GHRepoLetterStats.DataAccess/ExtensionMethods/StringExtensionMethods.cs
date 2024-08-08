namespace GHRepoLetterStats.DataAccess.ExtensionMethods;
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
}
