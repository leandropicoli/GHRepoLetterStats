using GHRepoLetterStats.DataAccess.Clients.Interfaces;

namespace GHRepoLetterStats.DataAccess.Clients.Impl;
public class GitHubApiClient : IGitHubApiClient
{
    private readonly HttpClient _httpClient;

    public GitHubApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<IEnumerable<string>> GetRepoFileNamesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetRepoFileNamesByExtensionAsync(string[] extensions)
    {
        throw new NotImplementedException();
    }
}
