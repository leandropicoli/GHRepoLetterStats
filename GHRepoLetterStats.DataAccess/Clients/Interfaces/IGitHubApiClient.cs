namespace GHRepoLetterStats.DataAccess.Clients.Interfaces;
public interface IGitHubApiClient
{
    Task<IEnumerable<string>> GetRepoFileNamesAsync();
    Task<IEnumerable<string>> GetRepoJavascriptAndTypescriptFileNamesAsync();
    Task<IEnumerable<string>> GetRepoFileNamesByExtensionAsync(string[] extensions);
}
