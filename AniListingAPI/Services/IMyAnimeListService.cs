using AniListingAPI.DTOs;

namespace AniListingAPI.Services;

public interface IMyAnimeListService
{
    Task<List<MediaItemDto>> SearchMediaAsync(string mediaType, string query);
    Task<MediaDetailsDto?> GetMediaDetailsAsync(string mediaType, int id);
}
