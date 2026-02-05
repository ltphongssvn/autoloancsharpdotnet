using AutoLoan.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoLoan.Api.Controllers.Api.V1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(
            request.Email, request.Password, request.FirstName, request.LastName, request.Phone);

        if (user == null)
            return BadRequest(new { error = "Email already exists" });

        var token = _jwtService.GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Email, user.FirstName, user.LastName, user.Role } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.LoginAsync(request.Email, request.Password);

        if (user == null)
            return Unauthorized(new { error = "Invalid email or password" });

        var token = _jwtService.GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Email, user.FirstName, user.LastName, user.Role } });
    }
}

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);
public record LoginRequest(string Email, string Password);
