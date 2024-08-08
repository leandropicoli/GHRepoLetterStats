using System.Text.Json.Serialization;

namespace GHRepoLetterStats.DataAccess.ExternalModels;
public class GitRepoResponse
{
    [JsonPropertyName("tree")]
    public IEnumerable<GitHubTree> Tree { get; set; } = Enumerable.Empty<GitHubTree>();
}

public class GitHubTree
{
    [JsonPropertyName("path")]
    public string Path { get; set; }
}