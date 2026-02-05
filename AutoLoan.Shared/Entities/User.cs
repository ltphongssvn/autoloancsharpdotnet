namespace AutoLoan.Shared.Entities;

public class User
{
    public long Id { get; set; }
    
    // Authentication
    public string Email { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public string? Jti { get; set; }
    
    // Profile
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    
    // Devise Trackable
    public int SignInCount { get; set; }
    public DateTime? CurrentSignInAt { get; set; }
    public DateTime? LastSignInAt { get; set; }
    public string? CurrentSignInIp { get; set; }
    public string? LastSignInIp { get; set; }
    
    // Devise Confirmable
    public string? ConfirmationToken { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ConfirmationSentAt { get; set; }
    public string? UnconfirmedEmail { get; set; }
    
    // Devise Recoverable
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordSentAt { get; set; }
    
    // Devise Rememberable
    public DateTime? RememberCreatedAt { get; set; }
    
    // Devise Lockable
    public int FailedAttempts { get; set; }
    public string? UnlockToken { get; set; }
    public DateTime? LockedAt { get; set; }
    
    // MFA (OTP)
    public string? OtpSecret { get; set; }
    public bool OtpRequiredForLogin { get; set; }
    public string? OtpBackupCodes { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Computed
    public string FullName => $"{FirstName} {LastName}";
}

public enum UserRole
{
    Customer = 0,
    LoanOfficer = 1,
    Underwriter = 2
}
