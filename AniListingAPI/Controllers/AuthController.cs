using System.Security.Claims;
using AniListingAPI.Data;
using AniListingAPI.Data.Entities;
using AniListingAPI.DTOs;
using AniListingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AniListingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AniListingDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthController(
        AniListingDbContext context,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var normalizedUsername = request.Username.Trim();

        var existingUser = await _context.Users
            .AnyAsync(u => u.Username.ToLower() == normalizedUsername.ToLower());

        if (existingUser)
        {
            return BadRequest(new { message = "Username is already taken." });
        }

        var user = new User
        {
            Username = normalizedUsername,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, user.Username, user.Id));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var normalizedUsername = request.Username.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedUsername.ToLower());

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, user.Username, user.Id));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserDto(user.Id, user.Username, user.CreatedAt));
    }
}
