using System.Security.Claims;
using AniListingAPI.Data;
using AniListingAPI.Data.Entities;
using AniListingAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AniListingAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserListController : ControllerBase
{
    private readonly AniListingDbContext _context;

    public UserListController(AniListingDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    User.FindFirstValue("sub");

        if (int.TryParse(claim, out var id))
        {
            return id;
        }

        throw new UnauthorizedAccessException("User identity claim is invalid.");
    }

    [HttpGet]
    public async Task<ActionResult<List<UserMediaItemDto>>> GetUserList(
        [FromQuery] string mediaType = "anime",
        [FromQuery] string? status = null,
        [FromQuery] string sortBy = "date",
        [FromQuery] string sortOrder = "desc")
    {
        var userId = GetCurrentUserId();
        var type = mediaType.ToLower() == "manga" ? "manga" : "anime";

        var query = _context.UserMediaItems
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.MediaType == type);

        if (!string.IsNullOrWhiteSpace(status) && !status.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(m => m.Status.ToLower() == status.Trim().ToLower());
        }

        var isDesc = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLower() switch
        {
            "name" or "title" => isDesc ? query.OrderByDescending(m => m.Title) : query.OrderBy(m => m.Title),
            "score" => isDesc ? query.OrderByDescending(m => m.Score ?? 0).ThenBy(m => m.Title)
                              : query.OrderBy(m => m.Score ?? 0).ThenBy(m => m.Title),
            "date" or "updatedat" or _ => isDesc ? query.OrderByDescending(m => m.UpdatedAt) : query.OrderBy(m => m.UpdatedAt)
        };

        var items = await query.ToListAsync();

        var result = items.Select(m => new UserMediaItemDto(
            Id: m.Id,
            UserId: m.UserId,
            MalId: m.MalId,
            MediaType: m.MediaType,
            Title: m.Title,
            JapaneseTitle: m.JapaneseTitle,
            PosterUrl: m.PosterUrl,
            Status: m.Status,
            Score: m.Score,
            CreatedAt: m.CreatedAt,
            UpdatedAt: m.UpdatedAt
        )).ToList();

        return Ok(result);
    }

    [HttpGet("check/{mediaType}/{malId:int}")]
    public async Task<ActionResult<UserMediaItemDto>> CheckItem(string mediaType, int malId)
    {
        var userId = GetCurrentUserId();
        var type = mediaType.ToLower() == "manga" ? "manga" : "anime";

        var item = await _context.UserMediaItems
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == userId && m.MediaType == type && m.MalId == malId);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(new UserMediaItemDto(
            Id: item.Id,
            UserId: item.UserId,
            MalId: item.MalId,
            MediaType: item.MediaType,
            Title: item.Title,
            JapaneseTitle: item.JapaneseTitle,
            PosterUrl: item.PosterUrl,
            Status: item.Status,
            Score: item.Score,
            CreatedAt: item.CreatedAt,
            UpdatedAt: item.UpdatedAt
        ));
    }

    [HttpPost]
    public async Task<ActionResult<UserMediaItemDto>> SaveItem([FromBody] SaveUserMediaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetCurrentUserId();
        var type = request.MediaType.ToLower() == "manga" ? "manga" : "anime";

        var existingItem = await _context.UserMediaItems
            .FirstOrDefaultAsync(m => m.UserId == userId && m.MediaType == type && m.MalId == request.MalId);

        if (existingItem != null)
        {
            existingItem.Title = request.Title;
            existingItem.JapaneseTitle = request.JapaneseTitle;
            existingItem.PosterUrl = request.PosterUrl;
            existingItem.Status = request.Status;
            existingItem.Score = request.Score;
            existingItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new UserMediaItemDto(
                existingItem.Id,
                existingItem.UserId,
                existingItem.MalId,
                existingItem.MediaType,
                existingItem.Title,
                existingItem.JapaneseTitle,
                existingItem.PosterUrl,
                existingItem.Status,
                existingItem.Score,
                existingItem.CreatedAt,
                existingItem.UpdatedAt
            ));
        }

        var newItem = new UserMediaItem
        {
            UserId = userId,
            MalId = request.MalId,
            MediaType = type,
            Title = request.Title,
            JapaneseTitle = request.JapaneseTitle,
            PosterUrl = request.PosterUrl,
            Status = request.Status,
            Score = request.Score,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.UserMediaItems.Add(newItem);
        await _context.SaveChangesAsync();

        return Ok(new UserMediaItemDto(
            newItem.Id,
            newItem.UserId,
            newItem.MalId,
            newItem.MediaType,
            newItem.Title,
            newItem.JapaneseTitle,
            newItem.PosterUrl,
            newItem.Status,
            newItem.Score,
            newItem.CreatedAt,
            newItem.UpdatedAt
        ));
    }

    [HttpDelete("{mediaType}/{malId:int}")]
    public async Task<IActionResult> DeleteItem(string mediaType, int malId)
    {
        var userId = GetCurrentUserId();
        var type = mediaType.ToLower() == "manga" ? "manga" : "anime";

        var item = await _context.UserMediaItems
            .FirstOrDefaultAsync(m => m.UserId == userId && m.MediaType == type && m.MalId == malId);

        if (item == null)
        {
            return NotFound();
        }

        _context.UserMediaItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
