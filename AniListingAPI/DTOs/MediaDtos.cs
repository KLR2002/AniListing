using System.Text.Json.Serialization;

namespace AniListingAPI.DTOs;

public record MediaItemDto(
    int Id,
    string MediaType, // "anime" or "manga"
    string Title,
    string? JapaneseTitle,
    string? EnglishTitle,
    string? PosterUrl,
    double? MeanScore,
    string? Status,
    int? EpisodeCount,
    int? ChapterCount,
    int? VolumeCount,
    string? Synopsis,
    List<string> Genres
);

public record MediaDetailsDto(
    int Id,
    string MediaType,
    string Title,
    string? JapaneseTitle,
    string? EnglishTitle,
    string? PosterUrl,
    double? MeanScore,
    int? Rank,
    int? Popularity,
    string? Status,
    string? Synopsis,
    int? EpisodeCount,
    int? ChapterCount,
    int? VolumeCount,
    string? StartDate,
    string? EndDate,
    List<string> Genres,
    string? StudioOrAuthor
);

// MyAnimeList API v2 Raw JSON response mappings
public class MalSearchResponse
{
    [JsonPropertyName("data")]
    public List<MalDataNode> Data { get; set; } = new();
}

public class MalDataNode
{
    [JsonPropertyName("node")]
    public MalMediaNode? Node { get; set; }
}

public class MalMediaNode
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("main_picture")]
    public MalPicture? MainPicture { get; set; }

    [JsonPropertyName("alternative_titles")]
    public MalAlternativeTitles? AlternativeTitles { get; set; }

    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; set; }

    [JsonPropertyName("mean")]
    public double? Mean { get; set; }

    [JsonPropertyName("rank")]
    public int? Rank { get; set; }

    [JsonPropertyName("popularity")]
    public int? Popularity { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("genres")]
    public List<MalGenre>? Genres { get; set; }

    [JsonPropertyName("num_episodes")]
    public int? NumEpisodes { get; set; }

    [JsonPropertyName("num_chapters")]
    public int? NumChapters { get; set; }

    [JsonPropertyName("num_volumes")]
    public int? NumVolumes { get; set; }

    [JsonPropertyName("studios")]
    public List<MalStudio>? Studios { get; set; }

    [JsonPropertyName("authors")]
    public List<MalAuthorNode>? Authors { get; set; }
}

public class MalPicture
{
    [JsonPropertyName("medium")]
    public string? Medium { get; set; }

    [JsonPropertyName("large")]
    public string? Large { get; set; }
}

public class MalAlternativeTitles
{
    [JsonPropertyName("synonyms")]
    public List<string>? Synonyms { get; set; }

    [JsonPropertyName("en")]
    public string? En { get; set; }

    [JsonPropertyName("ja")]
    public string? Ja { get; set; }
}

public class MalGenre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class MalStudio
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class MalAuthorNode
{
    [JsonPropertyName("node")]
    public MalAuthorDetail? Node { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }
}

public class MalAuthorDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
}
