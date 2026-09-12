using System.ComponentModel.DataAnnotations;

namespace AniListingAPI.DTOs;

public record RegisterRequest(
    [Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, and hyphens.")]
    string Username,

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    string Password
);

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    string Username,
    int UserId
);

public record UserDto(
    int Id,
    string Username,
    DateTime CreatedAt
);
