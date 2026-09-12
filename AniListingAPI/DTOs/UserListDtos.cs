using System.ComponentModel.DataAnnotations;

namespace AniListingAPI.DTOs;

public record SaveUserMediaRequest(
    [Required] int MalId,
    [Required] string MediaType, // "anime" or "manga"
    [Required] string Title,
    string? JapaneseTitle,
    string? PosterUrl,
    [Required] string Status, // Watching/Reading, Completed, PlanToWatch/PlanToRead, Dropped
    [Range(1, 10)] int? Score
);

public record UserMediaItemDto(
    int Id,
    int UserId,
    int MalId,
    string MediaType,
    string Title,
    string? JapaneseTitle,
    string? PosterUrl,
    string Status,
    int? Score,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
