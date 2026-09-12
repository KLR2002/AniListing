using System.Security.Claims;
using AniListingAPI.Controllers;
using AniListingAPI.Data;
using AniListingAPI.Data.Entities;
using AniListingAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AniListingAPI.Tests;

public class UserListTests
{
    private AniListingDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AniListingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AniListingDbContext(options);
    }

    private UserListController CreateController(AniListingDbContext context, int userId = 1)
    {
        var controller = new UserListController(context);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, "testuser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        return controller;
    }

    [Fact]
    public async Task SaveItem_AddsNewMediaItemAndChecksPresence()
    {
        using var context = CreateDbContext();
        var user = new User { Id = 1, Username = "testuser", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = CreateController(context, 1);

        var request = new SaveUserMediaRequest(
            MalId: 52991,
            MediaType: "anime",
            Title: "Frieren: Beyond Journey's End",
            JapaneseTitle: "葬送のフリーレン",
            PosterUrl: "https://cdn.myanimelist.net/images/anime/1015/138006.jpg",
            Status: "Watching",
            Score: 10
        );

        var saveResult = await controller.SaveItem(request);
        var okResult = Assert.IsType<OkObjectResult>(saveResult.Result);
        var itemDto = Assert.IsType<UserMediaItemDto>(okResult.Value);

        Assert.Equal(52991, itemDto.MalId);
        Assert.Equal("Watching", itemDto.Status);
        Assert.Equal(10, itemDto.Score);

        // Check presence
        var checkResult = await controller.CheckItem("anime", 52991);
        var checkOk = Assert.IsType<OkObjectResult>(checkResult.Result);
        var checkDto = Assert.IsType<UserMediaItemDto>(checkOk.Value);
        Assert.Equal(52991, checkDto.MalId);
    }

    [Fact]
    public async Task GetUserList_Sorting_WorksByNameScoreAndDate()
    {
        using var context = CreateDbContext();
        var user = new User { Id = 1, Username = "testuser", PasswordHash = "hash" };
        context.Users.Add(user);

        var item1 = new UserMediaItem
        {
            UserId = 1,
            MalId = 1,
            MediaType = "anime",
            Title = "Attack on Titan",
            Status = "Completed",
            Score = 8,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        };

        var item2 = new UserMediaItem
        {
            UserId = 1,
            MalId = 2,
            MediaType = "anime",
            Title = "Demon Slayer",
            Status = "Completed",
            Score = 10,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var item3 = new UserMediaItem
        {
            UserId = 1,
            MalId = 3,
            MediaType = "anime",
            Title = "Berserk Anime",
            Status = "Watching",
            Score = 9,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.UserMediaItems.AddRange(item1, item2, item3);
        await context.SaveChangesAsync();

        var controller = CreateController(context, 1);

        // Sort by Score Descending
        var scoreSorted = await controller.GetUserList("anime", sortBy: "score", sortOrder: "desc");
        var scoreOk = Assert.IsType<OkObjectResult>(scoreSorted.Result);
        var scoreList = Assert.IsType<List<UserMediaItemDto>>(scoreOk.Value);
        Assert.Equal(10, scoreList[0].Score);
        Assert.Equal(9, scoreList[1].Score);
        Assert.Equal(8, scoreList[2].Score);

        // Sort by Name Ascending
        var nameSorted = await controller.GetUserList("anime", sortBy: "name", sortOrder: "asc");
        var nameOk = Assert.IsType<OkObjectResult>(nameSorted.Result);
        var nameList = Assert.IsType<List<UserMediaItemDto>>(nameOk.Value);
        Assert.Equal("Attack on Titan", nameList[0].Title);
        Assert.Equal("Berserk Anime", nameList[1].Title);
        Assert.Equal("Demon Slayer", nameList[2].Title);

        // Filter by Status: Completed
        var filtered = await controller.GetUserList("anime", status: "Completed");
        var filteredOk = Assert.IsType<OkObjectResult>(filtered.Result);
        var filteredList = Assert.IsType<List<UserMediaItemDto>>(filteredOk.Value);
        Assert.Equal(2, filteredList.Count);
        Assert.All(filteredList, i => Assert.Equal("Completed", i.Status));
    }

    [Fact]
    public async Task DeleteItem_RemovesItemFromList()
    {
        using var context = CreateDbContext();
        var user = new User { Id = 1, Username = "testuser", PasswordHash = "hash" };
        context.Users.Add(user);
        context.UserMediaItems.Add(new UserMediaItem
        {
            UserId = 1,
            MalId = 52991,
            MediaType = "anime",
            Title = "Frieren",
            Status = "Watching"
        });
        await context.SaveChangesAsync();

        var controller = CreateController(context, 1);

        var deleteResult = await controller.DeleteItem("anime", 52991);
        Assert.IsType<NoContentResult>(deleteResult);

        var checkResult = await controller.CheckItem("anime", 52991);
        Assert.IsType<NotFoundResult>(checkResult.Result);
    }
}
