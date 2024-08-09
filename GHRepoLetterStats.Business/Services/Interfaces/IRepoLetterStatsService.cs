namespace GHRepoLetterStats.Business.Services.Interfaces;
public interface IRepoLetterStatsService
{
    Task<Dictionary<char, int>> GetLetterFrequenciesAsync(string repoOwner, string repoName, string defaultBranch, string[] fileTypes);
    Task<Dictionary<char, int>> GetLetterFrequenciesWithStringBuilderAsync(string repoOwner, string repoName, string defaultBranch, string[] fileTypes);
    Task<Dictionary<char, int>> GetLetterFrequenciesParallelAsync(string repoOwner, string repoName, string defaultBranch, string[] fileTypes);
    Task<Dictionary<char, int>> GetLetterFrequenciesParallelButLimitingThreadsAsync(string repoOwner, string repoName, string defaultBranch, string[] fileTypes);
    Task<Dictionary<char, int>> GetLetterFrequenciesParallelWithStringBuilderAsync(string repoOwner, string repoName, string defaultBranch, string[] fileTypes);
}
