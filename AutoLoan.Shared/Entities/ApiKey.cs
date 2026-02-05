namespace AutoLoan.Shared.Entities;

public class ApiKey
{
    public long Id { get; set; }
    public long UserId { get; set; }
    
    public string? Name { get; set; }
    public string? KeyDigest { get; set; }
    public bool Active { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public User User { get; set; } = null!;
}
