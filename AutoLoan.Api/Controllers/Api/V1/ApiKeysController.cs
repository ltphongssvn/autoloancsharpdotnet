using AutoLoan.Api.Data;
using AutoLoan.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AutoLoan.Api.Controllers.Api.V1;

[ApiController]
[Route("api/v1/api_keys")]
[Authorize]
public class ApiKeysController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ApiKeysController(ApplicationDbContext context)
    {
        _context = context;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var keys = await _context.ApiKeys
            .Where(k => k.UserId == GetUserId() && k.Active)
            .Select(k => new { k.Id, k.Name, k.LastUsedAt, k.ExpiresAt, k.CreatedAt })
            .ToListAsync();
        return Ok(keys);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest request)
    {
        var rawKey = GenerateApiKey();
        var hashedKey = HashKey(rawKey);

        var apiKey = new ApiKey
        {
            UserId = GetUserId(),
            Name = request.Name,
            KeyDigest = hashedKey,
            Active = true,
            ExpiresAt = request.ExpiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();

        return Ok(new { apiKey.Id, apiKey.Name, key = rawKey, apiKey.ExpiresAt });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var apiKey = await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Id == id && k.UserId == GetUserId());
        
        if (apiKey == null) return NotFound();

        apiKey.Active = false;
        apiKey.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static string GenerateApiKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private static string HashKey(string key)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(key);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}

public record CreateApiKeyRequest(string Name, DateTime? ExpiresAt);
