using GHRepoLetterStats.Business.Services.Impl;
using GHRepoLetterStats.Business.Services.Interfaces;
using GHRepoLetterStats.Common.Configuration;
using GHRepoLetterStats.DataAccess.Clients.Impl;
using GHRepoLetterStats.DataAccess.Clients.Interfaces;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection("GitHubOptions"));
builder.Services.AddScoped<IGitHubApiClient, GitHubApiClient>();
builder.Services.AddScoped<IRepoLetterStatsService, RepoLetterStatsService>();

var app = builder.Build();

var options = app.Services.GetRequiredService<IOptions<GitHubOptions>>().Value;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/LetterStats", async (IRepoLetterStatsService service) =>
{
    var repoOwner = options.RepoOwner;
    var repoName = options.RepoName;
    var defaultBranch = options.DefaultBranch;
    var fileTypes = options.FileTypes;

    var result = await service.GetLetterFrequenciesAsync(repoOwner, repoName, defaultBranch, fileTypes);
    return new LetterStatsResponse(repoOwner, repoName, defaultBranch, fileTypes, result);
})
.WithName("LetterStats")
.WithDescription("Letter Stats to search for the Repository and the FileTypes configured on project settings.")
.WithOpenApi();

app.MapGet("/CustomLetterStats", async (IRepoLetterStatsService service, string repoOwner, string repoName, string defaultBranch, string[] fileTypes) =>
{
    var result = await service.GetLetterFrequenciesAsync(repoOwner, repoName, defaultBranch, fileTypes);
    return new LetterStatsResponse(repoOwner, repoName, defaultBranch, fileTypes, result);
})
.WithName("CustomLetterStats")
.WithDescription("Custom Letter Stats to search for a specific Repository and the FileTypes.")
.WithOpenApi();

app.Run();

record LetterStatsResponse(string RepoOwner, string RepoName, string Branch, string[] FileTypes, Dictionary<char, int> Results);
