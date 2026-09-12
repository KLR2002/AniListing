using AniListingFront.Models;

namespace AniListingFront.Services;

public interface IApiClient
{
    Task<(bool Success, string? Error, AuthResponseModel? Data)> RegisterAsync(RegisterModel model);
    Task<(bool Success, string? Error, AuthResponseModel? Data)> LoginAsync(LoginModel model);
    Task LogoutAsync();
    Task<List<MediaItemModel>> SearchMediaAsync(string mediaType, string query);
    Task<MediaDetailsModel?> GetMediaDetailsAsync(string mediaType, int id);
    Task<List<UserMediaItemModel>> GetUserListAsync(string mediaType, string? status = null, string sortBy = "date", string sortOrder = "desc");
    Task<UserMediaItemModel?> CheckUserListItemAsync(string mediaType, int malId);
    Task<(bool Success, string? Error, UserMediaItemModel? Item)> SaveUserListItemAsync(SaveUserMediaModel model);
    Task<bool> DeleteUserListItemAsync(string mediaType, int malId);
}
