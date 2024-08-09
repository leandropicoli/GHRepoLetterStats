using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using GHRepoLetterStats.DataAccess.ExternalModels;
using GHRepoLetterStats.Common.ExtensionMethods;
using System.Text.Json;
using System.Net.Http.Headers;
using GHRepoLetterStats.Common.Configuration;
using Microsoft.Extensions.Options;

namespace GHRepoLetterStats.DataAccess.Clients.Impl;
public class GitHubApiClient : IGitHubApiClient
{
    private readonly HttpClient _httpClient;
    private readonly GitHubOptions _options;

    public GitHubApiClient(HttpClient httpClient, IOptions<GitHubOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
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
        var endpoint = BuildEndpointUrl();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GHRepoLetterStats", "1.0"));

        if (!string.IsNullOrWhiteSpace(_options.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _options.AccessToken);
        }

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

    private string BuildEndpointUrl()
    {
        return $"{_options.GitHubApiUrl}/repos/{_options.RepoOwner}/{_options.RepoName}/git/trees/{_options.DefaultBranch}?recursive=1";
    }
}
