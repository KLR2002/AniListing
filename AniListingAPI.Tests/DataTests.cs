using AniListingAPI.Data;
using AniListingAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AniListingAPI.Tests;

public class DataTests
{
    private AniListingDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AniListingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AniListingDbContext(options);
    }

    [Fact]
    public async Task CanAddUserAndMediaItem()
    {
        using var context = CreateDbContext();

        var user = new User
        {
            Username = "otakutest",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var mediaItem = new UserMediaItem
        {
            UserId = user.Id,
            MalId = 52991, // Frieren
            MediaType = "anime",
            Title = "Sousou no Frieren",
            JapaneseTitle = "葬送のフリーレン",
            PosterUrl = "https://cdn.myanimelist.net/images/anime/1015/138006.jpg",
            Status = "Watching",
            Score = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.UserMediaItems.Add(mediaItem);
        await context.SaveChangesAsync();

        var savedUser = await context.Users
            .Include(u => u.MediaItems)
            .FirstOrDefaultAsync(u => u.Username == "otakutest");

        Assert.NotNull(savedUser);
        Assert.Single(savedUser.MediaItems);
        var item = savedUser.MediaItems.First();
        Assert.Equal(52991, item.MalId);
        Assert.Equal("Sousou no Frieren", item.Title);
        Assert.Equal("Watching", item.Status);
        Assert.Equal(10, item.Score);
    }
}
