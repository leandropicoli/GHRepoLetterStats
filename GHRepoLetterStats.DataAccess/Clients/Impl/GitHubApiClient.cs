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
    private readonly Configuration _configuration;

    public GitHubApiClient(HttpClient httpClient, IOptions<Configuration> options)
    {
        _httpClient = httpClient;
        _configuration = options.Value;
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

        if (!string.IsNullOrWhiteSpace(_configuration.GitHubOptions.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _configuration.GitHubOptions.AccessToken);
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
        var githubOptions = _configuration.GitHubOptions;

        return $"{githubOptions.GitHubApiUrl}/repos/{githubOptions.RepoOwner}/{githubOptions.RepoName}/git/trees/{githubOptions.DefaultBranch}?recursive=1";
    }
}
