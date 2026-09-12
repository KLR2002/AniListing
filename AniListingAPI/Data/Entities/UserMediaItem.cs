using System.ComponentModel.DataAnnotations;

namespace AniListingAPI.Data.Entities;

public class UserMediaItem
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int MalId { get; set; }

    [Required]
    [MaxLength(20)]
    public string MediaType { get; set; } = "anime"; // "anime" or "manga"

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? JapaneseTitle { get; set; }

    [MaxLength(500)]
    public string? PosterUrl { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Watching"; // Watching/Reading, Completed, PlanToWatch/PlanToRead, Dropped

    [Range(1, 10)]
    public int? Score { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
