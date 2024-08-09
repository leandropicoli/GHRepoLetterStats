using GHRepoLetterStats.Common.Configuration;
using GHRepoLetterStats.DataAccess.Clients.Impl;
using GHRepoLetterStats.DataAccess.ExternalModels;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Text.Json;

namespace GHRepoLetterStats.Tests.DataAccess.Clients;
public class GitHubApiClientTests
{
    private readonly GitHubApiClient _sut;
    private readonly HttpClient _httpClient;
    private const string _repoOwner = "foo";
    private const string _repoName = "bar";
    private const string _defaultBranch = "main";
    private string[] _extensions;
    private Mock<IOptions<GitHubOptions>> _optionsMock;
    private GitHubOptions _options;
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

        _options = new GitHubOptions();
        _optionsMock = new Mock<IOptions<GitHubOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(_options);

        _extensions = ["js", "ts"];

        _sut = new GitHubApiClient(_httpClient, _optionsMock.Object);
    }

    [Fact]
    public async Task GetRepoFilesAsync_AccessTokenIsGiven_ShouldAddAsDefaultHeader()
    {
        //Arrange
        _options.AccessToken = "my-access-token";

        HttpRequestMessage requestMessage = null;

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
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => { requestMessage = request; })
            .ReturnsAsync(mockHttpResponseMessage);

        //Act
        _ = await _sut.GetRepoFilesAsync(_extensions, _repoOwner, _repoOwner, _defaultBranch);

        //Assert
        var headers = requestMessage!.Headers;
        Assert.Equal("my-access-token", headers.Authorization!.Parameter);
    }

    [Theory]
    [MemberData(nameof(GetRepoFilesAsyncTestData))]
    public async Task GetRepoFilesAsync_ListOfExtensionsIsGiven_ReturnsOnlyFilesWithThatExtension(string[] given, string[] expected)
    {
        //Arrange
        //Act
        var result = await _sut.GetRepoFilesAsync(given, _repoOwner, _repoOwner, _defaultBranch);

        //Assert
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> GetRepoFilesAsyncTestData
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
