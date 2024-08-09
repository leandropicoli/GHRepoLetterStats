using GHRepoLetterStats.Business.Services.Impl;
using GHRepoLetterStats.Business.Services.Interfaces;
using GHRepoLetterStats.Common.Configuration;
using GHRepoLetterStats.DataAccess.Clients.Impl;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var serviceProvider = serviceCollection.BuildServiceProvider();

var gitHubInfoSettings = serviceProvider.GetRequiredService<IOptions<GitHubOptions>>().Value;

Console.WriteLine($"Start fetching data from repo {gitHubInfoSettings.RepoOwner}/{gitHubInfoSettings.RepoName} on branch {gitHubInfoSettings.DefaultBranch}, please wait...");
Console.WriteLine("");

var result = await FetchItemsAsync();
PrintItems(result);

Console.ReadLine();

#region Helper Methods
async Task<Dictionary<char, int>> FetchItemsAsync()
{
    var service = serviceProvider.GetRequiredService<IRepoLetterStatsService>();

    return await service.GetLetterFrequenciesAsync(gitHubInfoSettings.RepoOwner, gitHubInfoSettings.RepoName, gitHubInfoSettings.DefaultBranch);
}

void PrintItems(Dictionary<char, int> result)
{
    foreach (var item in result)
    {
        Console.WriteLine($"{item.Key} - {item.Value}");
    }
}

void ConfigureServices(ServiceCollection serviceCollection)
{
    serviceCollection.AddHttpClient();
    serviceCollection.AddScoped<IGitHubApiClient, GitHubApiClient>();
    serviceCollection.AddScoped<IRepoLetterStatsService, RepoLetterStatsService>();
    serviceCollection.Configure<GitHubOptions>(ConfigurationManager.GetSection("GitHubOptions"));
}

#endregion
