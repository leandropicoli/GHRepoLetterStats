using GHRepoLetterStats.DataAccess.Clients.Impl;
using GHRepoLetterStats.DataAccess.ExternalModels;
using Moq;
using Moq.Protected;
using System.Text.Json;

namespace GHRepoLetterStats.Tests.DataAccess.Clients;
public class GitHubApiClientTests
{
    private readonly GitHubApiClient _sut;
    private readonly HttpClient _httpClient;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public GitHubApiClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var mockResponse = new GitRepoResponse
        {
            Tree = new List<GitHubTree>
            {
                new GitHubTree { Path = "file.js" },
                new GitHubTree { Path = "file.ts" },
                new GitHubTree { Path = "file.config" },
                new GitHubTree { Path = "readme.md" },
            }
        };

        var mockHttpResponseMessage = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(mockResponse))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockHttpResponseMessage);

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _sut = new GitHubApiClient(_httpClient);
    }

    [Fact]
    public async Task GetRepoFileNamesAsync_ReturnAListOfFileNamesAsync()
    {
        //Arrange
        //Act
        var result = await _sut.GetRepoFileNamesAsync();

        //Assert
        Assert.Equal(4, result.Count());
        Assert.Contains("file.js", result);
        Assert.Contains("file.ts", result);
        Assert.Contains("file.config", result);
        Assert.Contains("readme.md", result);
    }

    [Theory]
    [MemberData(nameof(GetRepoFileNamesByExtensionAsyncTestData))]
    public async Task GetRepoFileNamesByExtensionAsync_ListOfExtensionsIsGiven_ReturnsOnlyFilesWithThatExtension(string[] given, string[] expected)
    {
        //Arrange
        //Act
        var result = await _sut.GetRepoFileNamesByExtensionAsync(given);

        //Assert
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> GetRepoFileNamesByExtensionAsyncTestData
    {
        get
        {
            yield return new object[] { new string[] { "js" }, new string[] { "file.js" } };
            yield return new object[] { new string[] { ".js" }, new string[] { "file.js" } };
            yield return new object[] { new string[] { ".js", "ts" }, new string[] { "file.js", "file.ts" } };
            yield return new object[] { new string[0], new string[0] };
        }
    }
}
