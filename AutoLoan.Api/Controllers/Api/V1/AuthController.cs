using AutoLoan.Api.Data;
using AutoLoan.Api.Services;
using AutoLoan.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoLoan.Api.Controllers.Api.V1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly ISecurityAuditService _auditService;
    private readonly ApplicationDbContext _context;

    public AuthController(IAuthService authService, IJwtService jwtService, ISecurityAuditService auditService, ApplicationDbContext context)
    {
        _authService = authService;
        _jwtService = jwtService;
        _auditService = auditService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(
            request.Email, request.Password, request.FirstName, request.LastName, request.Phone);
        if (user == null)
        {
            await _auditService.LogAsync("registration_failed", null, "User", null, false, GetIpAddress(), GetUserAgent(), new { email = request.Email });
            return BadRequest(new { error = "Email already exists" });
        }

        await _auditService.LogAsync("registration", user.Id, "User", user.Id, true, GetIpAddress(), GetUserAgent());
        var token = _jwtService.GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Email, user.FirstName, user.LastName, user.Role } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.LoginAsync(request.Email, request.Password);
        if (user == null)
        {
            await _auditService.LogAsync("login_failed", null, "User", null, false, GetIpAddress(), GetUserAgent(), new { email = request.Email });
            return Unauthorized(new { error = "Invalid email or password" });
        }

        await _auditService.LogAsync("login", user.Id, "User", user.Id, true, GetIpAddress(), GetUserAgent());
        var token = _jwtService.GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Email, user.FirstName, user.LastName, user.Role } });
    }

    [HttpDelete("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var jti = User.FindFirstValue("jti");
        var exp = User.FindFirstValue("exp");
        
        if (jti != null && exp != null)
        {
            var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
            _context.JwtDenylists.Add(new JwtDenylist { Jti = jti, Exp = expDate });
            await _context.SaveChangesAsync();
        }

        await _auditService.LogAsync("logout", userId, "User", userId, true, GetIpAddress(), GetUserAgent());
        return NoContent();
    }

    private string? GetIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString();
    private string? GetUserAgent() => Request.Headers.UserAgent.ToString();
}

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);
public record LoginRequest(string Email, string Password);
