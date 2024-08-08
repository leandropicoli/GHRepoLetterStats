using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using GHRepoLetterStats.DataAccess.ExternalModels;
using GHRepoLetterStats.Common.ExtensionMethods;
using System.Text.Json;

namespace GHRepoLetterStats.DataAccess.Clients.Impl;
public class GitHubApiClient : IGitHubApiClient
{
    private readonly HttpClient _httpClient;

    public GitHubApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<string>> GetRepoFilePathAsync()
    {
        var response = await GetResponseFromEndpointAsync();

        return response.Tree.Select(x => x.Path);
    }

    public async Task<IEnumerable<string>> GetRepoJavascriptAndTypescriptFilePathAsync()
    {
        var response = await GetResponseFromEndpointAsync();

        return response.Tree.Where(x => x.Path.EndsWith("js") || x.Path.EndsWith("ts")).Select(x => x.Path);
    }

    public async Task<IEnumerable<string>> GetRepoFilePathByExtensionAsync(string[] extensions)
    {
        extensions = extensions.Select(x => x.Replace(".", "")).ToArray();

        var response = await GetResponseFromEndpointAsync();

        return response.Tree.Where(x => x.Path.EndsWith(extensions)).Select(x => x.Path);
    }

    private async Task<GitRepoResponse> GetResponseFromEndpointAsync()
    {
        var endpoint = "https://api.github.com/repos/lodash/lodash/git/trees/main?recursive=1";
        var httpResponse = await _httpClient.GetAsync(endpoint);
        httpResponse.EnsureSuccessStatusCode();

        var httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(httpResponseBody))
            throw new Exception("The response body was null or empty.");

        var result = JsonSerializer.Deserialize<GitRepoResponse>(httpResponseBody);

        if (result == null)
            throw new Exception("Failed to deserialize the response.");

        return result;
    }
}
