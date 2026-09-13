using System.Net.Http.Json;
using AniListingAPI.DTOs;

namespace AniListingAPI.Services;

public class MyAnimeListService : IMyAnimeListService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<MyAnimeListService> _logger;
    private readonly string _clientId;
    private const string BaseUrl = "https://api.myanimelist.net/v2";

    public MyAnimeListService(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<MyAnimeListService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
        _clientId = (_config["MyAnimeList:ClientId"] ?? "").Trim();
    }

    private bool IsMockMode =>
        string.IsNullOrEmpty(_clientId) ||
        _clientId.Equals("YOUR_CLIENT_ID_HERE", StringComparison.OrdinalIgnoreCase);

    public async Task<List<MediaItemDto>> SearchMediaAsync(string mediaType, string query)
    {
        var type = mediaType.ToLower() == "manga" ? "manga" : "anime";
        var cleanQuery = query?.Trim() ?? string.Empty;

        if (IsMockMode)
        {
            _logger.LogInformation("Using smart mock media catalog for search: {Type}, {Query}", type, cleanQuery);
            return MockMediaCatalog.Search(type, cleanQuery);
        }

        try
        {
            var fields = type == "anime"
                ? "id,title,main_picture,alternative_titles,mean,synopsis,genres,status,num_episodes,start_date"
                : "id,title,main_picture,alternative_titles,mean,synopsis,genres,status,num_chapters,num_volumes,start_date";

            var url = string.IsNullOrEmpty(cleanQuery)
                ? $"{BaseUrl}/{type}/ranking?ranking_type=bypopularity&limit=25&fields={fields}"
                : $"{BaseUrl}/{type}?q={Uri.EscapeDataString(cleanQuery)}&limit=25&fields={fields}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-MAL-CLIENT-ID", _clientId);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("MyAnimeList API returned status {Status}. Falling back to mock catalog.", response.StatusCode);
                return MockMediaCatalog.Search(type, cleanQuery);
            }

            var malResponse = await response.Content.ReadFromJsonAsync<MalSearchResponse>();
            if (malResponse?.Data == null)
            {
                return new List<MediaItemDto>();
            }

            return malResponse.Data
                .Where(d => d.Node != null)
                .Select(d => MapToMediaItemDto(d.Node!, type))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching from MyAnimeList API. Falling back to mock catalog.");
            return MockMediaCatalog.Search(type, cleanQuery);
        }
    }

    public async Task<MediaDetailsDto?> GetMediaDetailsAsync(string mediaType, int id)
    {
        var type = mediaType.ToLower() == "manga" ? "manga" : "anime";

        if (IsMockMode)
        {
            _logger.LogInformation("Using smart mock catalog for details: {Type}, {Id}", type, id);
            return MockMediaCatalog.GetDetails(type, id);
        }

        try
        {
            var fields = type == "anime"
                ? "id,title,main_picture,alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,genres,num_episodes,studios"
                : "id,title,main_picture,alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,genres,num_chapters,num_volumes,authors{first_name,last_name}";

            var url = $"{BaseUrl}/{type}/{id}?fields={fields}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-MAL-CLIENT-ID", _clientId);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("MyAnimeList API details returned {Status}. Falling back to mock catalog.", response.StatusCode);
                return MockMediaCatalog.GetDetails(type, id);
            }

            var node = await response.Content.ReadFromJsonAsync<MalMediaNode>();
            if (node == null)
            {
                return null;
            }

            return MapToMediaDetailsDto(node, type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching details from MyAnimeList API. Falling back to mock catalog.");
            return MockMediaCatalog.GetDetails(type, id);
        }
    }

    private static MediaItemDto MapToMediaItemDto(MalMediaNode node, string mediaType)
    {
        return new MediaItemDto(
            Id: node.Id,
            MediaType: mediaType,
            Title: node.Title,
            JapaneseTitle: node.AlternativeTitles?.Ja,
            EnglishTitle: node.AlternativeTitles?.En,
            PosterUrl: node.MainPicture?.Large ?? node.MainPicture?.Medium,
            MeanScore: node.Mean,
            Status: node.Status,
            EpisodeCount: node.NumEpisodes,
            ChapterCount: node.NumChapters,
            VolumeCount: node.NumVolumes,
            Synopsis: node.Synopsis,
            Genres: node.Genres?.Select(g => g.Name).ToList() ?? new List<string>()
        );
    }

    private static MediaDetailsDto MapToMediaDetailsDto(MalMediaNode node, string mediaType)
    {
        string? studioOrAuthor = null;
        if (mediaType == "anime" && node.Studios != null && node.Studios.Count > 0)
        {
            studioOrAuthor = string.Join(", ", node.Studios.Select(s => s.Name));
        }
        else if (mediaType == "manga" && node.Authors != null && node.Authors.Count > 0)
        {
            studioOrAuthor = string.Join(", ", node.Authors
                .Where(a => a.Node != null)
                .Select(a => $"{a.Node!.LastName} {a.Node.FirstName}".Trim()));
        }

        return new MediaDetailsDto(
            Id: node.Id,
            MediaType: mediaType,
            Title: node.Title,
            JapaneseTitle: node.AlternativeTitles?.Ja,
            EnglishTitle: node.AlternativeTitles?.En,
            PosterUrl: node.MainPicture?.Large ?? node.MainPicture?.Medium,
            MeanScore: node.Mean,
            Rank: node.Rank,
            Popularity: node.Popularity,
            Status: node.Status,
            Synopsis: node.Synopsis,
            EpisodeCount: node.NumEpisodes,
            ChapterCount: node.NumChapters,
            VolumeCount: node.NumVolumes,
            StartDate: node.StartDate,
            EndDate: node.EndDate,
            Genres: node.Genres?.Select(g => g.Name).ToList() ?? new List<string>(),
            StudioOrAuthor: studioOrAuthor
        );
    }
}
