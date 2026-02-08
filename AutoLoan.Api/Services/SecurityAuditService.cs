using AutoLoan.Api.Data;
using AutoLoan.Shared.Entities;

namespace AutoLoan.Api.Services;

public interface ISecurityAuditService
{
    Task LogAsync(string eventType, long? userId, string? resourceType = null, long? resourceId = null, 
        bool success = true, string? ipAddress = null, string? userAgent = null, object? metadata = null);
}

public class SecurityAuditService : ISecurityAuditService
{
    private readonly ApplicationDbContext _context;

    public SecurityAuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string eventType, long? userId, string? resourceType = null, long? resourceId = null,
        bool success = true, string? ipAddress = null, string? userAgent = null, object? metadata = null)
    {
        _context.SecurityAuditLogs.Add(new SecurityAuditLog
        {
            EventType = eventType,
            UserId = (int?)userId,
            ResourceType = resourceType,
            ResourceId = (int?)resourceId,
            Success = success,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Metadata = metadata != null ? System.Text.Json.JsonSerializer.Serialize(metadata) : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }
}
