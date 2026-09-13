using AniListingAPI.DTOs;
using AniListingAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AniListingAPI.Tests;

public class MediaServiceTests
{
    private IMyAnimeListService CreateMediaService()
    {
        // ClientId is empty -> triggers Mock fallback
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "MyAnimeList:ClientId", "" }
            })
            .Build();

        var httpClient = new HttpClient();
        return new MyAnimeListService(httpClient, config, NullLogger<MyAnimeListService>.Instance);
    }

    [Fact]
    public async Task SearchMedia_EnglishQuery_ReturnsMatchingAnime()
    {
        var service = CreateMediaService();

        var results = await service.SearchMediaAsync("anime", "Frieren");

        Assert.NotEmpty(results);
        var item = results.First();
        Assert.Equal(52991, item.Id);
        Assert.Contains("Frieren", item.Title);
    }

    [Fact]
    public async Task SearchMedia_JapaneseQuery_ReturnsMatchingAnime()
    {
        var service = CreateMediaService();

        var results = await service.SearchMediaAsync("anime", "葬送のフリーレン");

        Assert.NotEmpty(results);
        var item = results.First();
        Assert.Equal(52991, item.Id);
        Assert.Equal("葬送のフリーレン", item.JapaneseTitle);
    }

    [Fact]
    public async Task SearchMedia_MangaQuery_ReturnsMatchingManga()
    {
        var service = CreateMediaService();

        var results = await service.SearchMediaAsync("manga", "Berserk");

        Assert.NotEmpty(results);
        var item = results.First();
        Assert.Equal(2, item.Id);
        Assert.Equal("Berserk", item.Title);
        Assert.Equal("ベルセルク", item.JapaneseTitle);
    }

    [Fact]
    public async Task GetMediaDetails_ValidId_ReturnsFullDetails()
    {
        var service = CreateMediaService();

        var item = await service.GetMediaDetailsAsync("anime", 52991);

        Assert.NotNull(item);
        Assert.Equal(52991, item.Id);
        Assert.False(string.IsNullOrWhiteSpace(item.Synopsis));
        Assert.NotNull(item.MeanScore);
        Assert.True(item.MeanScore > 9.0);
        Assert.NotNull(item.PosterUrl);
        Assert.NotEmpty(item.Genres);
    }

    [Fact]
    public async Task SearchMedia_EmptyQuery_InMockMode_ReturnsPopularList()
    {
        var service = CreateMediaService();

        var results = await service.SearchMediaAsync("anime", "");

        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.Title.Contains("Frieren"));
    }

    [Fact]
    public async Task SearchMedia_EmptyQuery_WithClientId_CallsRankingEndpoint()
    {
        var testHandler = new TestRankingHttpMessageHandler();
        var httpClient = new HttpClient(testHandler);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "MyAnimeList:ClientId", "valid_client_id_123" }
            })
            .Build();

        var service = new MyAnimeListService(httpClient, config, NullLogger<MyAnimeListService>.Instance);

        var results = await service.SearchMediaAsync("anime", "");

        Assert.NotNull(testHandler.LastRequestedUri);
        Assert.Contains("/anime/ranking?ranking_type=bypopularity", testHandler.LastRequestedUri.PathAndQuery);
        Assert.NotEmpty(results);
        Assert.Equal("Ranking Title from Live API", results[0].Title);
    }

    private class TestRankingHttpMessageHandler : HttpMessageHandler
    {
        public Uri? LastRequestedUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestedUri = request.RequestUri;

            var jsonResponse = """
            {
                "data": [
                    {
                        "node": {
                            "id": 1001,
                            "title": "Ranking Title from Live API",
                            "main_picture": {
                                "large": "https://example.com/poster.jpg"
                            },
                            "mean": 8.95,
                            "synopsis": "Live API popular ranking synopsis."
                        },
                        "ranking": {
                            "rank": 1
                        }
                    }
                ]
            }
            """;

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}
