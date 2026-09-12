namespace AniListingFront.Models;

public class MediaItemModel
{
    public int Id { get; set; }
    public string MediaType { get; set; } = "anime";
    public string Title { get; set; } = string.Empty;
    public string? JapaneseTitle { get; set; }
    public string? EnglishTitle { get; set; }
    public string? PosterUrl { get; set; }
    public double? MeanScore { get; set; }
    public string? Status { get; set; }
    public int? EpisodeCount { get; set; }
    public int? ChapterCount { get; set; }
    public int? VolumeCount { get; set; }
    public string? Synopsis { get; set; }
    public List<string> Genres { get; set; } = new();
}

public class MediaDetailsModel
{
    public int Id { get; set; }
    public string MediaType { get; set; } = "anime";
    public string Title { get; set; } = string.Empty;
    public string? JapaneseTitle { get; set; }
    public string? EnglishTitle { get; set; }
    public string? PosterUrl { get; set; }
    public double? MeanScore { get; set; }
    public int? Rank { get; set; }
    public int? Popularity { get; set; }
    public string? Status { get; set; }
    public string? Synopsis { get; set; }
    public int? EpisodeCount { get; set; }
    public int? ChapterCount { get; set; }
    public int? VolumeCount { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public List<string> Genres { get; set; } = new();
    public string? StudioOrAuthor { get; set; }
}

public class UserMediaItemModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MalId { get; set; }
    public string MediaType { get; set; } = "anime";
    public string Title { get; set; } = string.Empty;
    public string? JapaneseTitle { get; set; }
    public string? PosterUrl { get; set; }
    public string Status { get; set; } = "Watching";
    public int? Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SaveUserMediaModel
{
    public int MalId { get; set; }
    public string MediaType { get; set; } = "anime";
    public string Title { get; set; } = string.Empty;
    public string? JapaneseTitle { get; set; }
    public string? PosterUrl { get; set; }
    public string Status { get; set; } = "Watching";
    public int? Score { get; set; }
}
