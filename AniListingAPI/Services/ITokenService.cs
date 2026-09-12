using AniListingAPI.Data.Entities;

namespace AniListingAPI.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
