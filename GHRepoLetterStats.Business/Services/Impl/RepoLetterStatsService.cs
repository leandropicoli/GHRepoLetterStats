using GHRepoLetterStats.Business.Services.Interfaces;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;

namespace GHRepoLetterStats.Business.Services.Impl;
public class RepoLetterStatsService : IRepoLetterStatsService
{
    private readonly IGitHubApiClient _gitHubApiClient;

    public RepoLetterStatsService(IGitHubApiClient gitHubApiClient)
    {
        _gitHubApiClient = gitHubApiClient;
    }

    public async Task<Dictionary<char, int>> GetLetterFrequenciesAsync()
    {
        var extensions = new string[2] { "js", "ts" };

        var result = (await _gitHubApiClient.GetRepoFilePathByExtensionAsync(extensions)).ToList();

        var response = new Dictionary<char, int>();

        if (result.Count == 0) 
            return response;

        foreach (var item in result)
        {
            foreach (var letter in item)
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

        return response.OrderByDescending(x => x.Value).ToDictionary();
    }
}
