using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Features.SkillManagement;

[ApiController]
[Route("api/[controller]")]
public class SkillsController(
    ISkillImportService importService,
    ISkillVerificationService verificationService,
    ISkillNormalizationService normalizationService,
    AppDbContext dbContext) : ControllerBase
{
    [HttpPost("import-from-vacancies")]
    public async Task<IActionResult> ImportFromVacancies(
        [FromQuery] string? vacancyNameFilter = null,
        [FromQuery] int? limit = null)
    {
        var result = await importService.ImportFromUnprocessedVacanciesAsync(vacancyNameFilter, limit);
        return Ok(result);
    }

    [HttpGet("on-review")]
    public async Task<IActionResult> GetSkillsOnReview()
    {
        var skills = await dbContext.SkillsOnReview
            .Where(s => s.Status == "pending")
            .OrderByDescending(s => s.Counter)
            .ToListAsync();

        var dtos = skills.Select(s => new SkillOnReviewDto
        {
            Id = s.Id,
            OriginalName = s.OriginalName,
            NormalizedName = s.NormalizedName,
            Counter = s.Counter,
            Status = s.Status,
            SuggestedDisplayName = s.SuggestedDisplayName,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).ToList();

        return Ok(new { items = dtos });
    }

    [HttpPost("on-review/{id}/approve")]
    public async Task<IActionResult> ApproveSkill(Guid id, [FromBody] ApproveSkillRequest request)
    {
        var result = await verificationService.ApproveSkillAsync(id, request);
        return Ok(result);
    }

    [HttpPost("on-review/{id}/reject")]
    public async Task<IActionResult> RejectSkill(Guid id)
    {
        var skill = await dbContext.SkillsOnReview.FirstOrDefaultAsync(s => s.Id == id);
        if (skill == null)
            return NotFound();

        dbContext.SkillsOnReview.Remove(skill);
        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("clean/add")]
    public async Task<IActionResult> AddCleanSkill([FromBody] AddCleanSkillRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName))
            return BadRequest("Display name is required");

        var normalizedName = !string.IsNullOrWhiteSpace(request.NormalizedName)
            ? request.NormalizedName
            : normalizationService.Normalize(request.DisplayName);

        var existingSkill = await dbContext.CleanSkills
            .FirstOrDefaultAsync(c => c.NormalizedName == normalizedName);

        if (existingSkill != null)
            return BadRequest("Skill with this normalized name already exists");

        var cleanSkill = new CleanSkill
        {
            NormalizedName = normalizedName,
            DisplayName = request.DisplayName,
            Description = request.Description,
            Counter = 1
        };

        await dbContext.CleanSkills.AddAsync(cleanSkill);
        await dbContext.SaveChangesAsync();

        return Ok(new CleanSkillDto
        {
            Id = cleanSkill.Id,
            NormalizedName = cleanSkill.NormalizedName,
            DisplayName = cleanSkill.DisplayName,
            Description = cleanSkill.Description,
            Counter = cleanSkill.Counter,
            CreatedAt = cleanSkill.CreatedAt,
            UpdatedAt = cleanSkill.UpdatedAt,
            Synonyms = new(),
            DependentSkills = new(),
            ParentSkills = new()
        });
    }

    [HttpGet("clean")]
    public async Task<IActionResult> GetCleanSkills()
    {
        var skills = await dbContext.CleanSkills
            .Include(c => c.Synonyms)
            .Include(c => c.ParentRelations)
            .ThenInclude(r => r.ChildSkill)
            .Include(c => c.ChildRelations)
            .ThenInclude(r => r.ParentSkill)
            .OrderByDescending(c => c.Counter)
            .ToListAsync();

        var dtos = skills.Select(s => new CleanSkillDto
        {
            Id = s.Id,
            NormalizedName = s.NormalizedName,
            DisplayName = s.DisplayName,
            Description = s.Description,
            Counter = s.Counter,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            Synonyms = s.Synonyms.Select(syn => new SkillSynonymDto
            {
                Id = syn.Id,
                SynonymName = syn.SynonymName,
                IsFromNormalization = syn.IsFromNormalization,
                CreatedAt = syn.CreatedAt
            }).ToList(),
            DependentSkills = s.ParentRelations
                .Where(rel => rel.ChildSkill != null)
                .Select(rel => new SkillDependencyDto
                {
                    Id = rel.ChildSkill!.Id,
                    NormalizedName = rel.ChildSkill!.NormalizedName,
                    DisplayName = rel.ChildSkill!.DisplayName,
                    RelationType = rel.RelationType
                }).ToList(),
            ParentSkills = s.ChildRelations
                .Where(rel => rel.ParentSkill != null)
                .Select(rel => new SkillDependencyDto
                {
                    Id = rel.ParentSkill!.Id,
                    NormalizedName = rel.ParentSkill!.NormalizedName,
                    DisplayName = rel.ParentSkill!.DisplayName,
                    RelationType = rel.RelationType
                }).ToList()
        }).ToList();

        return Ok(new { items = dtos });
    }
}
