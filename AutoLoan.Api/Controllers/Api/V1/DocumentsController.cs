using AutoLoan.Api.Data;
using AutoLoan.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoLoan.Api.Controllers.Api.V1;

[ApiController]
[Route("api/v1/applications/{applicationId}/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DocumentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index(long applicationId)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == GetUserId());
        if (application == null) return NotFound();

        var documents = await _context.Documents
            .Where(d => d.ApplicationId == applicationId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
        return Ok(documents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Show(long applicationId, long id)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id && d.ApplicationId == applicationId);
        if (document == null) return NotFound();
        return Ok(document);
    }

    [HttpPost]
    public async Task<IActionResult> Create(long applicationId, [FromBody] CreateDocumentRequest request)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == GetUserId());
        if (application == null) return NotFound();

        var document = new Document
        {
            ApplicationId = applicationId,
            FileName = request.FileName,
            FileUrl = request.FileUrl,
            ContentType = request.ContentType,
            FileSize = request.FileSize,
            DocType = request.DocType,
            Status = DocumentStatus.Pending,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Show), new { applicationId, id = document.Id }, document);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long applicationId, long id)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == GetUserId());
        if (application == null) return NotFound();

        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id && d.ApplicationId == applicationId);
        if (document == null) return NotFound();

        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateDocumentRequest(string FileName, string? FileUrl, string? ContentType, int? FileSize, DocumentType DocType);
