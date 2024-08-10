# GHRepoLetterStats

## Overview

`GHRepoLetterStats` is a .NET-based project consisting of an API and a console application that connects to a specified GitHub repository. The application fetches files and calculates the frequency of each letter. The API provides endpoints to retrieve these statistics with both default and custom settings.

## Features

- Fetches letter frequencies from a given file type in a specified GitHub repository.
- Supports both a RESTful API and a console application.
- Customizable via configuration files.

## Project Structure

- **GHRepoLetterStats.Business**: Contains business logic services for calculating letter statistics.
- **GHRepoLetterStats.Common**: Contains common configuration classes.
- **GHRepoLetterStats.DataAccess**: Contains data access clients that interact with the GitHub API.
- **GHRepoLetterStats.ConsoleApp**: The console application for fetching and printing letter statistics.
- **GHRepoLetterStats.Api**: The RESTful API for fetching letter statistics via HTTP endpoints.

## Prerequisites

- .NET 8.0 SDK or later
- A GitHub repository to test against
- `appsettings.json` configured with GitHub repository details

## Configuration

### `appsettings.json` (for both API and Console App)

```json
{
  "GitHubOptions": {
    "RepoOwner": "lodash",
    "RepoName": "lodash",
    "DefaultBranch": "main",
    "FileTypes": [".js", ".ts"],
    "AccessToken": "",
    "SubExtensionsToIgnore": [ ".spec" ]
  }
}
```

- **RepoOwner**: The owner of the GitHub repository.
- **RepoName**: The name of the GitHub repository.
- **DefaultBranch**: The branch to fetch files from.
- **FileTypes**: An array of file extensions to include in the letter frequency analysis.
- **AccessToken (optional)**: To avoid GitHub api rate limiting. Add it to the env vars
  - **Windows**
  ```bash
  setx GitHubOptions__AccessToken "your_github_token"
  ```

  - **Linux/macOS**
  ```bash
  export GitHubOptions__AccessToken "your_github_token"
  ```
- **SubExtensionsToIgnore**: Array of *SubExtensions* to ignore on counting, like *spec* files.

## API Documentation

### Setup
1. **Clone the repository** and navigate to the **GHRepoLetterStats.Api** directory.
2. **Restore dependencies** by running:
```
dotnet restore
```

3. **Run the API** by executing:
```
dotnet run
```


### Endpoints
**/LetterStats**
- **Description**: Fetches letter statistics for the repository and file types configured in appsettings.json.
- **Method**: GET
- **Response**:
```json
{
  "repoOwner": "lodash",
  "repoName": "lodash",
  "branch": "main",
  "fileTypes": [
    "ts",
    "js"
  ],
  "results": {
    "e": 787,
    "t": 574
    // more letters and their counts
  }
}
```

**/CustomLetterStats**
- **Description**: Fetches letter statistics for a specified repository and file types.
- **Method**: GET
- **Parameters**:
  - **repoOwner** (string)
  - **repoName** (string)
  - **defaultBranch** (string)
  - **fileTypes** (string[])
- **Response**:
```json
{
  "repoOwner": "leandropicoli",
  "repoName": "RallyCalendar",
  "branch": "main",
  "fileTypes": [
    "md"
  ],
  "results": {
    "e": 2,
    "r": 1,
    "a": 1,
    "d": 1,
    "m": 1
  }
}
```

### Swagger UI
If running in development mode, you can access the Swagger UI at **`https://localhost:{port}/swagger`** to interact with the API endpoints.

## Console Application Documentation

### Setup
1. **Clone the repository** and navigate to the **GHRepoLetterStats.ConsoleApp** directory.
2. **Restore dependencies** by running:
```
dotnet restore
```

3. **Run the ConsoleApp** by executing:
```
dotnet run
```

### Usage
Upon running, the console application will:

- Fetch data from the GitHub repository specified in **appsettings.json**.
- Calculate the letter frequencies in the file types specified in **appsettings.json**.
- Print the results to the console.

### Example Output
```
Start fetching data from repo lodash/lodash on branch main
On file types .js, .ts
Please wait...

e - 787
t - 574
...
// more letters and their counts
```

## Testing
- The project uses xUnit for testing.
- To run tests, navigate to the solution directory and run:
```
dotnet test
```

## Benchmarks

There are another implementations of **RepoLetterStatsService** on branch [benchmarks](https://github.com/leandropicoli/GHRepoLetterStats/blob/benchmarks/GHRepoLetterStats.Business/Services/Impl/RepoLetterStatsService.cs)

Below are the results
![Benchmark](/benchmark.png)