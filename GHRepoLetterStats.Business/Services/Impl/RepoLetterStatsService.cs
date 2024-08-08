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

    public Task<Dictionary<char, int>> GetLetterFrequenciesAsync()
    {
        throw new NotImplementedException();
    }
}
