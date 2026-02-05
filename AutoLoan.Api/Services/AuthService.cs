using AutoLoan.Api.Data;
using AutoLoan.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoLoan.Api.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(string email, string password, string firstName, string lastName, string phone);
    Task<User?> LoginAsync(string email, string password);
    Task<bool> EmailExistsAsync(string email);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> RegisterAsync(string email, string password, string firstName, string lastName, string phone)
    {
        if (await EmailExistsAsync(email))
            return null;

        var user = new User
        {
            Email = email.ToLower(),
            EncryptedPassword = BCrypt.Net.BCrypt.HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            Role = UserRole.Customer,
            Jti = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.EncryptedPassword))
            return null;

        user.SignInCount++;
        user.LastSignInAt = user.CurrentSignInAt;
        user.CurrentSignInAt = DateTime.UtcNow;
        user.Jti = Guid.NewGuid().ToString();
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email.ToLower());
    }
}
