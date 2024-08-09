using GHRepoLetterStats.Business.Services.Impl;
using GHRepoLetterStats.Common.Configuration;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace GHRepoLetterStats.Tests.Business.Services;
public class RepoLetterStatsServiceTests
{
    private readonly RepoLetterStatsService _sut;
    private const string _repoOwner = "foo";
    private const string _repoName = "bar";
    private const string _defaultBranch = "main";
    private Mock<IGitHubApiClient> _gitHubApiClientMock;
    private Mock<IOptions<GitHubOptions>> _optionsMock;
    private GitHubOptions _options;

    public RepoLetterStatsServiceTests()
    {
        _gitHubApiClientMock = new Mock<IGitHubApiClient>(MockBehavior.Strict);
        _optionsMock = new Mock<IOptions<GitHubOptions>>(MockBehavior.Strict);
        _options = new GitHubOptions();
        _optionsMock.Setup(x => x.Value).Returns(_options);

        _sut = new RepoLetterStatsService(_gitHubApiClientMock.Object, _optionsMock.Object);
    }

    [Fact]
    public async Task GetLetterFrequenciesAsync_ReturnsLetterStats()
    {
        //Arrange
        var clientResponse = new List<string>
        {
            "abccaa",
            "abbbccd",
            "abbccc"
        };

        MockApiClientResponse(clientResponse);

        //Act
        var result = await _sut.GetLetterFrequenciesAsync(_repoOwner, _repoName, _defaultBranch);

        //Assert
        Assert.Equal(5, result['a']);
        Assert.Equal(6, result['b']);
        Assert.Equal(7, result['c']);
        Assert.Equal(1, result['d']);
    }

    [Fact]
    public async Task GetLetterFrequenciesAsync_ReturnsUndescOrderedDictionary()
    {
        //Arrange
        var clientResponse = new List<string>
        {
            "abccaa",
            "abbbccd",
            "abbccc"
        };

        var expectedOrder = new Dictionary<char, int>
        {
            { 'c', 7 },
            { 'b', 6 },
            { 'a', 5 },
            { 'd', 1 }
        };

        MockApiClientResponse(clientResponse);

        //Act
        var result = await _sut.GetLetterFrequenciesAsync(_repoOwner, _repoName, _defaultBranch);

        //Assert
        Assert.Equal(expectedOrder, result);
        Assert.Equal('c', result.ElementAt(0).Key);
        Assert.Equal('b', result.ElementAt(1).Key);
        Assert.Equal('a', result.ElementAt(2).Key);
        Assert.Equal('d', result.ElementAt(3).Key);
    }

    [Fact]
    public async Task GetLetterFrequenciesAsync_FullPath_CountOnlyFileName()
    {
        //Arrange
        var clientResponse = new List<string>
        {
            "src/abccaa.ts",
            "test/abbbccd.js",
            "Services/abbccc.js"
        };

        MockApiClientResponse(clientResponse);

        //Act
        var result = await _sut.GetLetterFrequenciesAsync(_repoOwner, _repoName, _defaultBranch);

        //Assert
        Assert.Equal(5, result['a']);
        Assert.Equal(6, result['b']);
        Assert.Equal(7, result['c']);
        Assert.Equal(1, result['d']);
    }

    [Fact]
    public async Task GetLetterFrequenciesAsync_FullPathWithSpecFiles_CountOnlyFileName()
    {
        //Arrange
        var clientResponse = new List<string>
        {
            "src/abccaa.spec.ts",
            "test/abbbccd.js",
            "Services/abbccc.js"
        };

        _options.SubExtensionsToIgnore = [".spec"];

        MockApiClientResponse(clientResponse);

        //Act
        var result = await _sut.GetLetterFrequenciesAsync(_repoOwner, _repoName, _defaultBranch);

        //Assert
        Assert.Equal(5, result['a']);
        Assert.Equal(6, result['b']);
        Assert.Equal(7, result['c']);
        Assert.Equal(1, result['d']);
    }

    [Fact]
    public async Task GetLetterFrequenciesAsync_NamesWithSpecialCharacters_CountOnlyLetters()
    {
        //Arrange
        var clientResponse = new List<string>
        {
            "ab-cc-aa",
            "abbb-ccd",
            "ab_bccc"
        };

        MockApiClientResponse(clientResponse);

        //Act
        var result = await _sut.GetLetterFrequenciesAsync(_repoOwner, _repoName, _defaultBranch);

        //Assert
        Assert.Equal(5, result['a']);
        Assert.Equal(6, result['b']);
        Assert.Equal(7, result['c']);
        Assert.Equal(1, result['d']);
        Assert.False(result.ContainsKey('-'));
        Assert.False(result.ContainsKey('_'));
    }

    [Fact]
    public async Task GetLetterFrequenciesAsync_LowerCaseAndUpperCaseLetters_IgnoreCase()
    {
        //Arrange
        var clientResponse = new List<string>
        {
            "abccaA",
            "abbBccd",
            "abbCcc"
        };

        MockApiClientResponse(clientResponse);

        //Act
        var result = await _sut.GetLetterFrequenciesAsync(_repoOwner, _repoName, _defaultBranch);

        //Assert
        Assert.Equal(5, result['a']);
        Assert.Equal(6, result['b']);
        Assert.Equal(7, result['c']);
        Assert.Equal(1, result['d']);
    }

    #region Helpers
    private void MockApiClientResponse(List<string> response)
    {
        _gitHubApiClientMock
            .Setup(x => x.GetRepoFilesAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(response);
    }
    #endregion
}
