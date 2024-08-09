using Microsoft.Extensions.Configuration;

namespace GHRepoLetterStats.Common.Configuration;
public static class ConfigurationManager
{
    private static IConfigurationRoot configuration;

    static ConfigurationManager()
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public static string GetSetting(string key)
    {
        return configuration[key];
    }

    public static T GetSection<T>(string section) where T : new()
    {
        var sectionData = new T();
        configuration.GetSection(section).Bind(sectionData);
        return sectionData;
    }

    public static IConfigurationSection GetSection(string section)
    {
        return configuration.GetSection(section);
    }
}
