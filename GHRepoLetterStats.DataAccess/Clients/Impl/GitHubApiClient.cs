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

    public async Task<IEnumerable<string>> GetRepoFilesAsync(string[] extensions, string repoOwner, string repoName, string defaultBranch)
    {
        extensions = extensions.Select(x => x.Replace(".", "")).ToArray();
        var endpoint = BuildEndpointUrl(repoOwner, repoName, defaultBranch);
        var response = await GetResponseFromEndpointAsync(endpoint);

        return response.Tree.Where(x => x.Path.EndsWith(extensions)).Select(x => x.Path);
    }

    private async Task<GitRepoResponse> GetResponseFromEndpointAsync(string endpoint)
    {
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

    private string BuildEndpointUrl(string repoOwner, string repoName, string defaultBranch)
    {
        return $"{_options.GitHubApiUrl}/repos/{repoOwner}/{repoName}/git/trees/{defaultBranch}?recursive=1";
    }
}
