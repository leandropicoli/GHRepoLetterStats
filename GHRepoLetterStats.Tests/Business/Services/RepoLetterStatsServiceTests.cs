﻿using GHRepoLetterStats.Business.Services.Impl;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using Moq;
using Xunit;

namespace GHRepoLetterStats.Tests.Business.Services;
public class RepoLetterStatsServiceTests
{
    private readonly RepoLetterStatsService _sut;
    private Mock<IGitHubApiClient> _gitHubApiClientMock;

    public RepoLetterStatsServiceTests()
    {
        _gitHubApiClientMock = new Mock<IGitHubApiClient>(MockBehavior.Strict);
        _sut = new RepoLetterStatsService(_gitHubApiClientMock.Object);
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

        _gitHubApiClientMock
            .Setup(x => x.GetRepoFilePathByExtensionAsync(It.IsAny<string[]>()))
            .ReturnsAsync(clientResponse);

        //Act
        var result = await _sut.GetLetterFrequenciesAsync();

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

        _gitHubApiClientMock
            .Setup(x => x.GetRepoFilePathByExtensionAsync(It.IsAny<string[]>()))
            .ReturnsAsync(clientResponse);

        //Act
        var result = await _sut.GetLetterFrequenciesAsync();

        //Assert
        Assert.Equal(expectedOrder, result);
        Assert.Equal('c', result.ElementAt(0).Key);
        Assert.Equal('b', result.ElementAt(1).Key);
        Assert.Equal('a', result.ElementAt(2).Key);
        Assert.Equal('d', result.ElementAt(3).Key);
    }
}
