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


app.MapGet("/LetterStats", async (IRepoLetterStatsService service, string? repoOwner, string? repoName, string? defaultBranch) =>
{
    if (string.IsNullOrEmpty(repoOwner))
        repoOwner = options.RepoOwner;

    if (string.IsNullOrEmpty(repoName))
        repoName = options.RepoName;

    if (string.IsNullOrEmpty(defaultBranch))
        defaultBranch = options.DefaultBranch;

    var result = await service.GetLetterFrequenciesAsync(repoOwner, repoName, defaultBranch);
    return new LetterStatsResponse(repoOwner, repoName, defaultBranch, result);
})
.WithName("LetterStats")
.WithOpenApi();

app.Run();

record LetterStatsResponse(string RepoOwner, string RepoName, string Branch, Dictionary<char, int> Results);
