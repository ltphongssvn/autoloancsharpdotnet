namespace AutoLoan.Shared.Entities;

public class StatusHistory
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public long UserId { get; set; }
    
    public string? FromStatus { get; set; }
    public string? ToStatus { get; set; }
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Application Application { get; set; } = null!;
    public User User { get; set; } = null!;
}
