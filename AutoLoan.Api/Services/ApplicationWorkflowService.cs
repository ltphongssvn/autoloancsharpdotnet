using AutoLoan.Api.Data;
using AutoLoan.Shared.Entities;

namespace AutoLoan.Api.Services;

public class ApplicationWorkflowService
{
    private readonly ApplicationDbContext _context;

    private static readonly Dictionary<ApplicationStatus, ApplicationStatus[]> ValidTransitions = new()
    {
        [ApplicationStatus.Draft] = [ApplicationStatus.Submitted],
        [ApplicationStatus.Submitted] = [ApplicationStatus.Pending, ApplicationStatus.UnderReview, ApplicationStatus.PendingDocuments],
        [ApplicationStatus.Pending] = [ApplicationStatus.UnderReview, ApplicationStatus.PendingDocuments],
        [ApplicationStatus.UnderReview] = [ApplicationStatus.PendingDocuments, ApplicationStatus.Approved, ApplicationStatus.Rejected],
        [ApplicationStatus.PendingDocuments] = [ApplicationStatus.UnderReview, ApplicationStatus.Approved, ApplicationStatus.Rejected]
    };

    public ApplicationWorkflowService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Application> SubmitAsync(Application application, long? userId = null)
    {
        var fromStatus = application.Status;
        ValidateTransition(application, ApplicationStatus.Submitted, ApplicationStatus.Draft);
        
        application.Status = ApplicationStatus.Submitted;
        application.SubmittedAt = DateTime.UtcNow;
        application.ApplicationNumber = $"AL-{DateTime.UtcNow:yyyyMMdd}-{application.Id:D6}";
        application.UpdatedAt = DateTime.UtcNow;
        
        TrackStatusChange(application, fromStatus, ApplicationStatus.Submitted, userId ?? application.UserId);
        
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<Application> MoveToReviewAsync(Application application, long userId, string? comment = null)
    {
        var fromStatus = application.Status;
        ValidateTransition(application, ApplicationStatus.UnderReview, ApplicationStatus.Submitted, ApplicationStatus.Pending);
        
        application.Status = ApplicationStatus.UnderReview;
        application.UpdatedAt = DateTime.UtcNow;
        
        TrackStatusChange(application, fromStatus, ApplicationStatus.UnderReview, userId, comment);
        
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<Application> RequestDocumentsAsync(Application application, long userId, string? comment = null)
    {
        var fromStatus = application.Status;
        ValidateTransition(application, ApplicationStatus.PendingDocuments, 
            ApplicationStatus.Submitted, ApplicationStatus.UnderReview, ApplicationStatus.Pending);
        
        application.Status = ApplicationStatus.PendingDocuments;
        application.UpdatedAt = DateTime.UtcNow;
        
        TrackStatusChange(application, fromStatus, ApplicationStatus.PendingDocuments, userId, comment);
        
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<Application> ApproveAsync(Application application, long userId, decimal? interestRate, decimal? monthlyPayment)
    {
        var fromStatus = application.Status;
        ValidateTransition(application, ApplicationStatus.Approved, 
            ApplicationStatus.UnderReview, ApplicationStatus.PendingDocuments);
        
        application.Status = ApplicationStatus.Approved;
        application.InterestRate = interestRate;
        application.MonthlyPayment = monthlyPayment;
        application.DecidedAt = DateTime.UtcNow;
        application.UpdatedAt = DateTime.UtcNow;
        
        TrackStatusChange(application, fromStatus, ApplicationStatus.Approved, userId);
        
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<Application> RejectAsync(Application application, long userId, string? reason)
    {
        var fromStatus = application.Status;
        ValidateTransition(application, ApplicationStatus.Rejected, 
            ApplicationStatus.UnderReview, ApplicationStatus.PendingDocuments);
        
        application.Status = ApplicationStatus.Rejected;
        application.RejectionReason = reason;
        application.DecidedAt = DateTime.UtcNow;
        application.UpdatedAt = DateTime.UtcNow;
        
        TrackStatusChange(application, fromStatus, ApplicationStatus.Rejected, userId, reason);
        
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<Application> SignAsync(Application application, string signatureData)
    {
        if (application.Status != ApplicationStatus.Approved)
            throw new TransitionException("Application must be approved");
        
        if (application.IsSigned)
            throw new TransitionException("Application already signed");
        
        application.SignatureData = signatureData;
        application.SignedAt = DateTime.UtcNow;
        application.AgreementAccepted = true;
        application.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return application;
    }

    public bool CanTransitionTo(Application application, ApplicationStatus target)
    {
        if (!ValidTransitions.TryGetValue(application.Status, out var allowed))
            return false;
        return allowed.Contains(target);
    }

    private void TrackStatusChange(Application application, ApplicationStatus from, ApplicationStatus to, long userId, string? comment = null)
    {
        _context.StatusHistories.Add(new StatusHistory
        {
            ApplicationId = application.Id,
            UserId = userId,
            FromStatus = from.ToString(),
            ToStatus = to.ToString(),
            Comment = comment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    private static void ValidateTransition(Application application, ApplicationStatus target, params ApplicationStatus[] allowedFrom)
    {
        if (!allowedFrom.Contains(application.Status))
        {
            throw new TransitionException(
                $"Cannot transition from {application.Status} to {target}. Allowed from: {string.Join(", ", allowedFrom)}");
        }
    }
}

public class TransitionException : Exception
{
    public TransitionException(string message) : base(message) { }
}
