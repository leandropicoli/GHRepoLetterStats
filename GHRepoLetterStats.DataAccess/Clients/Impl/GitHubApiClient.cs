﻿using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using GHRepoLetterStats.DataAccess.ExtensionMethods;
using GHRepoLetterStats.DataAccess.ExternalModels;
using System.Text.Json;

namespace GHRepoLetterStats.DataAccess.Clients.Impl;
public class GitHubApiClient : IGitHubApiClient
{
    private readonly HttpClient _httpClient;

    public GitHubApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<string>> GetRepoFileNamesAsync()
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

        return result.Tree.Select(x => x.Path);
    }

    public Task<IEnumerable<string>> GetRepoJavascriptAndTypescriptFileNamesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<string>> GetRepoFileNamesByExtensionAsync(string[] extensions)
    {
        extensions = extensions.Select(x => x.Replace(".", "")).ToArray();

        var endpoint = "https://api.github.com/repos/lodash/lodash/git/trees/main?recursive=1";
        var httpResponse = await _httpClient.GetAsync(endpoint);
        httpResponse.EnsureSuccessStatusCode();

        var httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(httpResponseBody))
            throw new Exception("The response body was null or empty.");

        var result = JsonSerializer.Deserialize<GitRepoResponse>(httpResponseBody);

        if (result == null)
            throw new Exception("Failed to deserialize the response.");

        return result.Tree.Where(x => x.Path.EndsWith(extensions)).Select(x => x.Path);
    }
}
