namespace GHRepoLetterStats.Business.Services.Interfaces;
public interface IRepoLetterStatsService
{
    Task<Dictionary<char, int>> GetLetterFrequenciesAsync(string repoOwner, string repoName, string defaultBranch);
}
