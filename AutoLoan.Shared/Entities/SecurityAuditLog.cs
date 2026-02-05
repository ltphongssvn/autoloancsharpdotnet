namespace AutoLoan.Shared.Entities;

public class SecurityAuditLog
{
    public long Id { get; set; }
    public int? UserId { get; set; }
    public string? EventType { get; set; }
    public string? ResourceType { get; set; }
    public int? ResourceId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool Success { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
