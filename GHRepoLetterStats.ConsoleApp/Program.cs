using GHRepoLetterStats.Business.Services.Impl;
using GHRepoLetterStats.Business.Services.Interfaces;
using GHRepoLetterStats.Common.Configuration;
using GHRepoLetterStats.DataAccess.Clients.Impl;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var serviceCollection = new ServiceCollection();
var configurationManager = new ConfigurationManager();
configurationManager.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
configurationManager.AddEnvironmentVariables();
ConfigureServices(serviceCollection, configurationManager);
var serviceProvider = serviceCollection.BuildServiceProvider();

var gitHubInfoSettings = serviceProvider.GetRequiredService<IOptions<GitHubOptions>>().Value;

Console.WriteLine($"Start fetching data from repo {gitHubInfoSettings.RepoOwner}/{gitHubInfoSettings.RepoName} on branch {gitHubInfoSettings.DefaultBranch}");
Console.WriteLine($"On file types {string.Join(", ", gitHubInfoSettings.FileTypes)}");
Console.WriteLine("Please wait...");
Console.WriteLine("");

var result = await FetchItemsAsync();
PrintItems(result);

Console.ReadLine();

#region Helper Methods
async Task<Dictionary<char, int>> FetchItemsAsync()
{
    var service = serviceProvider.GetRequiredService<IRepoLetterStatsService>();

    return await service.GetLetterFrequenciesAsync(gitHubInfoSettings.RepoOwner, gitHubInfoSettings.RepoName, gitHubInfoSettings.DefaultBranch, gitHubInfoSettings.FileTypes);
}

void PrintItems(Dictionary<char, int> result)
{
    if (result.Count == 0)
    {
        Console.WriteLine("No files are found for the given repo");
        return;
    }

    foreach (var item in result)
    {
        Console.WriteLine($"{item.Key} - {item.Value}");
    }
}

void ConfigureServices(ServiceCollection serviceCollection, ConfigurationManager configurationManager)
{
    serviceCollection.AddHttpClient();
    serviceCollection.AddScoped<IGitHubApiClient, GitHubApiClient>();
    serviceCollection.AddScoped<IRepoLetterStatsService, RepoLetterStatsService>();
    serviceCollection.Configure<GitHubOptions>(configurationManager.GetSection("GitHubOptions"));
}

#endregion
