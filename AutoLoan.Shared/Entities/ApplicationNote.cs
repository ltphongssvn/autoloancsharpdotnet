namespace AutoLoan.Shared.Entities;

public class ApplicationNote
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public long UserId { get; set; }
    
    public string? Note { get; set; }
    public bool Internal { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Application Application { get; set; } = null!;
    public User User { get; set; } = null!;
}
