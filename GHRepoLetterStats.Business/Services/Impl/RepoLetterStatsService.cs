using GHRepoLetterStats.Business.Services.Interfaces;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using GHRepoLetterStats.Common.ExtensionMethods;

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
            var fileName = Path.GetFileNameWithoutExtension(item);
            fileName = fileName.ToLower();
            fileName = fileName.Replace(".spec", "");
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

        return response.OrderByDescending(x => x.Value).ToDictionary();
    }
}
