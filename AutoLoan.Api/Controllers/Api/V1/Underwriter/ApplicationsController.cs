using AutoLoan.Api.Data;
using AutoLoan.Api.Services;
using AutoLoan.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoLoan.Api.Controllers.Api.V1.Underwriter;

[ApiController]
[Route("api/v1/underwriter/applications")]
[Authorize(Roles = "Underwriter")]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ApplicationWorkflowService _workflowService;

    public ApplicationsController(ApplicationDbContext context, ApplicationWorkflowService workflowService)
    {
        _context = context;
        _workflowService = workflowService;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string? status)
    {
        var query = _context.Applications
            .Include(a => a.User)
            .Include(a => a.Vehicle)
            .Include(a => a.FinancialInfos)
            .Where(a => a.Status == ApplicationStatus.UnderReview || 
                        a.Status == ApplicationStatus.Approved || 
                        a.Status == ApplicationStatus.Rejected);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ApplicationStatus>(status, true, out var s))
            query = query.Where(a => a.Status == s);

        var applications = await query.OrderByDescending(a => a.UpdatedAt).ToListAsync();
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
            .Include(a => a.StatusHistories)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null) return NotFound();
        return Ok(application);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(long id, [FromBody] ApproveRequest request)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null) return NotFound();

        try
        {
            await _workflowService.ApproveAsync(application, GetUserId(), request.InterestRate, request.MonthlyPayment);
            return Ok(application);
        }
        catch (TransitionException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(long id, [FromBody] RejectRequest request)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null) return NotFound();

        try
        {
            await _workflowService.RejectAsync(application, GetUserId(), request.Reason);
            return Ok(application);
        }
        catch (TransitionException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record ApproveRequest(decimal InterestRate, decimal MonthlyPayment);
public record RejectRequest(string Reason);
