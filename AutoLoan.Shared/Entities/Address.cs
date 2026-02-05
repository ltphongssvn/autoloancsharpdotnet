namespace AutoLoan.Shared.Entities;

public class Address
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    
    public string AddressType { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int? YearsAtAddress { get; set; }
    public int? MonthsAtAddress { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Application Application { get; set; } = null!;
}
