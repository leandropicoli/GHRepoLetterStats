using GHRepoLetterStats.Business.Services.Interfaces;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using GHRepoLetterStats.Common.ExtensionMethods;
using Microsoft.Extensions.Options;
using GHRepoLetterStats.Common.Configuration;

namespace GHRepoLetterStats.Business.Services.Impl;
public class RepoLetterStatsService : IRepoLetterStatsService
{
    private readonly IGitHubApiClient _gitHubApiClient;
    private readonly GitHubOptions _options;

    public RepoLetterStatsService(IGitHubApiClient gitHubApiClient, IOptions<GitHubOptions> config)
    {
        _gitHubApiClient = gitHubApiClient;
        _options = config.Value;
    }

    public async Task<Dictionary<char, int>> GetLetterFrequenciesAsync(string repoOwner, string repoName, string defaultBranch, string[] fileTypes)
    {
        var result = (await _gitHubApiClient.GetRepoFilesAsync(fileTypes, repoOwner, repoName, defaultBranch)).ToList();

        var response = new Dictionary<char, int>();

        if (result.Count == 0)
            return response;

        foreach (var item in result)
        {
            var fileName = Path.GetFileNameWithoutExtension(item).ToLower();

            foreach (var subtype in _options.SubExtensionsToIgnore)
            {
                fileName = fileName.Replace(subtype, "");
            }

            fileName = fileName.RemoveSpecialCharacters();

            foreach (var letter in fileName)
            {
                if (response.ContainsKey(letter))
                {
                    response[letter] += 1;
                }
                else
                {
                    response[letter] = 1;
                }
            }
        }

        return response.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }
}
