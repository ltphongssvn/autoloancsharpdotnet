namespace AutoLoan.Shared.Entities;

public class Document
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public long? VerifiedById { get; set; }
    
    public string FileName { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? ContentType { get; set; }
    public int? FileSize { get; set; }
    public DocumentType DocType { get; set; } = DocumentType.Other;
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    
    public DateTime? UploadedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? RequestNote { get; set; }
    public string? RejectionNote { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Application Application { get; set; } = null!;
    public User? VerifiedBy { get; set; }
}

public enum DocumentType
{
    Other = 0,
    DriversLicense = 1,
    ProofOfIncome = 2,
    ProofOfInsurance = 3,
    ProofOfResidence = 4
}

public enum DocumentStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Requested = 3
}
