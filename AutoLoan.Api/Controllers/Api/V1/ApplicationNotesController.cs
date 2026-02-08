using AutoLoan.Api.Data;
using AutoLoan.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoLoan.Api.Controllers.Api.V1;

[ApiController]
[Route("api/v1/applications/{applicationId}/notes")]
[Authorize(Roles = "LoanOfficer,Underwriter")]
public class ApplicationNotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ApplicationNotesController(ApplicationDbContext context)
    {
        _context = context;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index(long applicationId)
    {
        var notes = await _context.ApplicationNotes
            .Include(n => n.User)
            .Where(n => n.ApplicationId == applicationId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> Create(long applicationId, [FromBody] CreateNoteRequest request)
    {
        var application = await _context.Applications.FindAsync(applicationId);
        if (application == null) return NotFound();

        var note = new ApplicationNote
        {
            ApplicationId = applicationId,
            UserId = GetUserId(),
            Note = request.Note,
            Internal = request.Internal ?? true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ApplicationNotes.Add(note);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Index), new { applicationId }, note);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long applicationId, long id)
    {
        var note = await _context.ApplicationNotes
            .FirstOrDefaultAsync(n => n.Id == id && n.ApplicationId == applicationId && n.UserId == GetUserId());
        
        if (note == null) return NotFound();

        _context.ApplicationNotes.Remove(note);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateNoteRequest(string Note, bool? Internal);
