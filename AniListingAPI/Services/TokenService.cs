using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AniListingAPI.Data.Entities;
using Microsoft.IdentityModel.Tokens;

namespace AniListingAPI.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(User user)
    {
        var secretKey = _config["Jwt:Key"] ?? "SuperSecretSakuraBlossomKeyForJwtValidation2026!#AniListingDefaultSecretKeyLengthCheck123";
        var issuer = _config["Jwt:Issuer"] ?? "AniListingAPI";
        var audience = _config["Jwt:Audience"] ?? "AniListingFront";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
