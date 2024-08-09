namespace GHRepoLetterStats.DataAccess.Clients.Interfaces;
public interface IGitHubApiClient
{
    Task<IEnumerable<string>> GetRepoFilesAsync(string[] extensions, string repoOwner, string repoName, string defaultBranch);
}
