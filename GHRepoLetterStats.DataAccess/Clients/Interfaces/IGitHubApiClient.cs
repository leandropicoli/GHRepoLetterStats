namespace GHRepoLetterStats.DataAccess.Clients.Interfaces;
public interface IGitHubApiClient
{
    Task<IEnumerable<string>> GetRepoFilePathAsync();
    Task<IEnumerable<string>> GetRepoJavascriptAndTypescriptFilePathAsync();
    Task<IEnumerable<string>> GetRepoFilePathByExtensionAsync(string[] extensions);
}
