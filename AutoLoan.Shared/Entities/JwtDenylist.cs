namespace AutoLoan.Shared.Entities;

public class JwtDenylist
{
    public long Id { get; set; }
    public string? Jti { get; set; }
    public DateTime? Exp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
