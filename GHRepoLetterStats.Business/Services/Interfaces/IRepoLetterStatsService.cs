namespace GHRepoLetterStats.Business.Services.Interfaces;
public interface IRepoLetterStatsService
{
    Task<Dictionary<char, int>> GetLetterFrequenciesAsync();

}
