using AutoLoan.Api.Data;
using AutoLoan.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoLoan.Api.Controllers.Api.V1.LoanOfficer;

[ApiController]
[Route("api/v1/loan_officer/applications")]
[Authorize(Roles = "LoanOfficer,Underwriter")]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ApplicationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string? status)
    {
        var query = _context.Applications
            .Include(a => a.User)
            .Include(a => a.Vehicle)
            .Where(a => a.Status != ApplicationStatus.Draft);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ApplicationStatus>(status, true, out var s))
            query = query.Where(a => a.Status == s);

        var applications = await query.OrderByDescending(a => a.SubmittedAt).ToListAsync();
        return Ok(applications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Show(long id)
    {
        var application = await _context.Applications
            .Include(a => a.User)
            .Include(a => a.Vehicle)
            .Include(a => a.Addresses)
            .Include(a => a.FinancialInfos)
            .Include(a => a.Documents)
            .Include(a => a.ApplicationNotes)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null) return NotFound();
        return Ok(application);
    }

    [HttpPost("{id}/review")]
    public async Task<IActionResult> Review(long id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null) return NotFound();

        if (application.Status != ApplicationStatus.Submitted)
            return BadRequest(new { error = "Application not in submitted status" });

        application.Status = ApplicationStatus.UnderReview;
        application.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(application);
    }

    [HttpPost("{id}/request_documents")]
    public async Task<IActionResult> RequestDocuments(long id, [FromBody] RequestDocumentsRequest request)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null) return NotFound();

        application.Status = ApplicationStatus.PendingDocuments;
        application.UpdatedAt = DateTime.UtcNow;

        foreach (var docType in request.DocumentTypes)
        {
            _context.Documents.Add(new Document
            {
                ApplicationId = id,
                FileName = $"Requested: {docType}",
                DocType = docType,
                Status = DocumentStatus.Requested,
                RequestNote = request.Note,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return Ok(application);
    }
}

public record RequestDocumentsRequest(DocumentType[] DocumentTypes, string? Note);
