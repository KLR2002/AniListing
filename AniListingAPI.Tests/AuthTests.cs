using System.Security.Claims;
using AniListingAPI.Controllers;
using AniListingAPI.Data;
using AniListingAPI.Data.Entities;
using AniListingAPI.DTOs;
using AniListingAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AniListingAPI.Tests;

public class AuthTests
{
    private AniListingDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AniListingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AniListingDbContext(options);
    }

    private ITokenService CreateTokenService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "SuperSecretSakuraBlossomKeyForJwtValidation2026!#" },
                { "Jwt:Issuer", "AniListingAPI" },
                { "Jwt:Audience", "AniListingFront" }
            })
            .Build();

        return new TokenService(config);
    }

    [Fact]
    public async Task Register_CreatesUserWithHashedPassword()
    {
        using var context = CreateDbContext();
        var passwordHasher = new PasswordHasher<User>();
        var tokenService = CreateTokenService();
        var controller = new AuthController(context, passwordHasher, tokenService);

        var request = new RegisterRequest("sakura_fan", "SakuraPass123!");
        var actionResult = await controller.Register(request);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.Equal("sakura_fan", response.Username);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));

        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "sakura_fan");
        Assert.NotNull(savedUser);
        Assert.NotEqual("SakuraPass123!", savedUser.PasswordHash);

        var verificationResult = passwordHasher.VerifyHashedPassword(savedUser, savedUser.PasswordHash, "SakuraPass123!");
        Assert.Equal(PasswordVerificationResult.Success, verificationResult);
    }

    [Fact]
    public async Task Register_FailsWhenUsernameAlreadyExists()
    {
        using var context = CreateDbContext();
        var passwordHasher = new PasswordHasher<User>();
        var tokenService = CreateTokenService();
        var controller = new AuthController(context, passwordHasher, tokenService);

        context.Users.Add(new User
        {
            Username = "existing_user",
            PasswordHash = "somehash",
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var request = new RegisterRequest("existing_user", "AnotherPass123!");
        var actionResult = await controller.Register(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public async Task Login_SucceedsWithValidCredentials()
    {
        using var context = CreateDbContext();
        var passwordHasher = new PasswordHasher<User>();
        var tokenService = CreateTokenService();

        var user = new User
        {
            Username = "anime_lover",
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "MySecretPass123!");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new AuthController(context, passwordHasher, tokenService);
        var loginRequest = new LoginRequest("anime_lover", "MySecretPass123!");
        var actionResult = await controller.Login(loginRequest);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.Equal("anime_lover", response.Username);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
    }

    [Fact]
    public async Task Login_FailsWithInvalidPassword()
    {
        using var context = CreateDbContext();
        var passwordHasher = new PasswordHasher<User>();
        var tokenService = CreateTokenService();

        var user = new User
        {
            Username = "anime_lover",
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "CorrectPass123!");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new AuthController(context, passwordHasher, tokenService);
        var loginRequest = new LoginRequest("anime_lover", "WrongPass!");
        var actionResult = await controller.Login(loginRequest);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        Assert.NotNull(unauthorized.Value);
    }

    [Fact]
    public void CreateToken_ProducesSingleNameClaim_NotAnArray()
    {
        var tokenService = CreateTokenService();
        var user = new User { Id = 42, Username = "TestUser" };

        var token = tokenService.CreateToken(user);
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var nameClaims = jwtToken.Claims.Where(c => c.Type == "unique_name" || c.Type == ClaimTypes.Name).ToList();
        Assert.Single(nameClaims);
        Assert.Equal("TestUser", nameClaims[0].Value);

        var subClaims = jwtToken.Claims.Where(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier).ToList();
        Assert.Single(subClaims);
        Assert.Equal("42", subClaims[0].Value);
    }
}
