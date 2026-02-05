namespace AutoLoan.Shared.Entities;

public class Vehicle
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Trim { get; set; }
    public string? Vin { get; set; }
    public string? Condition { get; set; }
    public int? Mileage { get; set; }
    public decimal? EstimatedValue { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Application Application { get; set; } = null!;
}
