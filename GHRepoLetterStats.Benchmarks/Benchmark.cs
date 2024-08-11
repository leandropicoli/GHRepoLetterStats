using BenchmarkDotNet.Attributes;
using GHRepoLetterStats.Business.Services.Impl;
using GHRepoLetterStats.Common.Configuration;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace GHRepoLetterStats.Benchmarks;

[MemoryDiagnoser]
public class Benchmark
{
    private RepoLetterStatsService _repoLetterStatsService;

    [GlobalSetup]
    public void Setup()
    {
        var fileName = new string('a', 20);
        var response = new List<string>();
        for (int i = 0; i < 50; i++)
        {
            response.Add(fileName);
        }

        response.Add("file.spec");
        response.Add("file2.spec");
        response.Add("file3.spec");

        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
         .Setup(x => x.GetRepoFilesAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
         .ReturnsAsync(response);

        var mockOptions = new Mock<IOptions<GitHubOptions>>();
        mockOptions.Setup(x => x.Value).Returns(new GitHubOptions { SubExtensionsToIgnore = [".spec"] });

        _repoLetterStatsService = new RepoLetterStatsService(mockClient.Object, mockOptions.Object);
    }

    //[Benchmark]
    //public async Task<Dictionary<char, int>> GetLetterFrequenciesParallelAsync()
    //{
    //    return await _repoLetterStatsService.GetLetterFrequenciesParallelAsync("foo", "bar", "main", new string[] { "test" });
    //}

    //[Benchmark]
    //public async Task<Dictionary<char, int>> GetLetterFrequenciesParallelButLimitingThreadsAsync()
    //{
    //    return await _repoLetterStatsService.GetLetterFrequenciesParallelButLimitingThreadsAsync("foo", "bar", "main", new string[] { "test" });
    //}

    //[Benchmark]
    //public async Task<Dictionary<char, int>> GetLetterFrequenciesParallelWithStringBuilderAsync()
    //{
    //    return await _repoLetterStatsService.GetLetterFrequenciesParallelWithStringBuilderAsync("foo", "bar", "main", new string[] { "test" });
    //}

    //[Benchmark]
    //public async Task<Dictionary<char, int>> GetLetterFrequenciesWithStringBuilderAsync()
    //{
    //    return await _repoLetterStatsService.GetLetterFrequenciesWithStringBuilderAsync("foo", "bar", "main", new string[] { "test" });
    //}

    [Benchmark]
    public async Task<Dictionary<char, int>> GetLetterFrequenciesAsync()
    {
        return await _repoLetterStatsService.GetLetterFrequenciesAsync("foo", "bar", "main", new string[] { "test" });

        //| Mean     | Error    | StdDev   | Gen0   | Gen1   | Allocated |
        //| 32.94 us | 0.792 us | 2.272 us | 0.3662 | 0.1221 |   2.41 KB |
    }

    [Benchmark]
    public async Task<Dictionary<char, int>> GetLetterFrequenciesRegexRemovingAllAsync()
    {
        return await _repoLetterStatsService.GetLetterFrequenciesRegexRemovingAllAsync("foo", "bar", "main", new string[] { "test" });

        //| Mean     | Error    | StdDev   | Gen0   | Gen1   | Allocated |
        //| 31.87 us | 0.637 us | 1.148 us | 0.9460 | 0.1831 |   4.54 KB |
        //Also this method fails at unit tests
    }
}
