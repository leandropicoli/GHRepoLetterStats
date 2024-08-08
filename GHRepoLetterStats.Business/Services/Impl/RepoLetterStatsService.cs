using GHRepoLetterStats.Business.Services.Interfaces;

namespace GHRepoLetterStats.Business.Services.Impl;
public class RepoLetterStatsService : IRepoLetterStatsService
{
    public Task<Dictionary<char, int>> GetLetterFrequenciesAsync()
    {
        throw new NotImplementedException();
    }
}
