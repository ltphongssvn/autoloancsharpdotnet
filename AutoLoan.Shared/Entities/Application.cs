namespace AutoLoan.Shared.Entities;

public class Application
{
    public long Id { get; set; }
    public long UserId { get; set; }
    
    // Application Info
    public string? ApplicationNumber { get; set; }
    public int CurrentStep { get; set; } = 1;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
    
    // Personal
    public DateTime? Dob { get; set; }
    public string? SsnEncrypted { get; set; }
    
    // Loan Details
    public decimal? LoanAmount { get; set; }
    public decimal? DownPayment { get; set; }
    public int? LoanTerm { get; set; }
    public decimal? InterestRate { get; set; }
    public decimal? MonthlyPayment { get; set; }
    
    // Status Tracking
    public DateTime? SubmittedAt { get; set; }
    public DateTime? DecidedAt { get; set; }
    public string? RejectionReason { get; set; }
    
    // Agreement
    public bool AgreementAccepted { get; set; }
    public string? SignatureData { get; set; }
    public DateTime? SignedAt { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties
    public User User { get; set; } = null!;
    public Vehicle? Vehicle { get; set; }
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<FinancialInfo> FinancialInfos { get; set; } = new List<FinancialInfo>();
    public ICollection<ApplicationNote> ApplicationNotes { get; set; } = new List<ApplicationNote>();
    public ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
    
    // Computed
    public bool IsSigned => SignedAt.HasValue && AgreementAccepted;
}

public enum ApplicationStatus
{
    Draft = 0,
    Submitted = 1,
    UnderReview = 2,
    PendingDocuments = 3,
    Approved = 4,
    Rejected = 5,
    Pending = 6
}
