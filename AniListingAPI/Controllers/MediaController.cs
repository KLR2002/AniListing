using AniListingAPI.DTOs;
using AniListingAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AniListingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMyAnimeListService _mediaService;

    public MediaController(IMyAnimeListService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<MediaItemDto>>> Search(
        [FromQuery] string type = "anime",
        [FromQuery] string q = "")
    {
        var results = await _mediaService.SearchMediaAsync(type, q);
        return Ok(results);
    }

    [HttpGet("{type}/{id:int}")]
    public async Task<ActionResult<MediaDetailsDto>> GetDetails(string type, int id)
    {
        var details = await _mediaService.GetMediaDetailsAsync(type, id);
        if (details == null)
        {
            return NotFound(new { message = $"Media item {id} of type '{type}' not found." });
        }

        return Ok(details);
    }
}
