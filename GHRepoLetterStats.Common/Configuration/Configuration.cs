namespace GHRepoLetterStats.Common.Configuration;
public class Configuration
{
    public GitHubOptions GitHubOptions { get; set; } = new();
}

public class GitHubOptions
{
    public string GitHubApiUrl { get; set; } = "https://api.github.com";
    public string RepoOwner { get; set; } = "lodash";
    public string RepoName { get; set; } = "lodash";
    public string DefaultBranch { get; set; } = "main";
    public string? AccessToken { get; set; }
    public string[] SubExtensionsToIgnore { get; set; } = [];
}