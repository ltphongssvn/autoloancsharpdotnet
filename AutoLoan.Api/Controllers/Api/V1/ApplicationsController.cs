using AutoLoan.Api.Data;
using AutoLoan.Api.Services;
using AutoLoan.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoLoan.Api.Controllers.Api.V1;

[ApiController]
[Route("api/v1/applications")]
[Authorize]
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
    public async Task<IActionResult> Index()
    {
        var applications = await _context.Applications
            .Where(a => a.UserId == GetUserId())
            .Include(a => a.Vehicle)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return Ok(applications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Show(long id)
    {
        var application = await _context.Applications
            .Include(a => a.Vehicle)
            .Include(a => a.Addresses)
            .Include(a => a.FinancialInfos)
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == GetUserId());

        if (application == null) return NotFound();
        return Ok(application);
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var application = new Application
        {
            UserId = GetUserId(),
            Status = ApplicationStatus.Draft,
            CurrentStep = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Show), new { id = application.Id }, application);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateApplicationRequest request)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == GetUserId());

        if (application == null) return NotFound();

        if (request.CurrentStep.HasValue) application.CurrentStep = request.CurrentStep.Value;
        if (request.Dob.HasValue) application.Dob = request.Dob.Value;
        if (request.LoanAmount.HasValue) application.LoanAmount = request.LoanAmount.Value;
        if (request.DownPayment.HasValue) application.DownPayment = request.DownPayment.Value;
        if (request.LoanTerm.HasValue) application.LoanTerm = request.LoanTerm.Value;
        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(application);
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(long id)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == GetUserId());

        if (application == null) return NotFound();

        try
        {
            await _workflowService.SubmitAsync(application, GetUserId());
            return Ok(application);
        }
        catch (TransitionException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/sign")]
    public async Task<IActionResult> Sign(long id, [FromBody] SignRequest request)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == GetUserId());

        if (application == null) return NotFound();

        try
        {
            await _workflowService.SignAsync(application, request.SignatureData);
            return Ok(application);
        }
        catch (TransitionException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == GetUserId());

        if (application == null) return NotFound();
        if (application.Status != ApplicationStatus.Draft)
            return BadRequest(new { error = "Cannot delete submitted application" });

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public record SignRequest(string SignatureData);

public record UpdateApplicationRequest(
    int? CurrentStep, DateTime? Dob, decimal? LoanAmount, 
    decimal? DownPayment, int? LoanTerm);
