namespace AutoLoan.Shared.Entities;

public class FinancialInfo
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    
    public string? EmploymentStatus { get; set; }
    public string? EmployerName { get; set; }
    public string? JobTitle { get; set; }
    public int? YearsEmployed { get; set; }
    public int? MonthsEmployed { get; set; }
    public string? IncomeType { get; set; }
    public decimal? AnnualIncome { get; set; }
    public decimal? MonthlyIncome { get; set; }
    public decimal? OtherIncome { get; set; }
    public decimal? MonthlyExpenses { get; set; }
    public int? CreditScore { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Application Application { get; set; } = null!;
}
