using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Hephaestus.Features.VacancySaver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Features.VacancyController;

[ApiController]
[Route("api/[controller]")]
public class VacancyController(IVacancySaver vacancySaver, AppDbContext dbContext) : ControllerBase
{
    [HttpGet("save")]
    public async Task<IActionResult> SaveVacancies([FromQuery] string search)
    {
        await vacancySaver.SaveVacancyToDatabaseAsync(search);
        return Ok();
    }

    [HttpGet("skills")]
    public async Task<IActionResult> GetAllSkills()
    {
        var skills = await dbContext.KeySkills
            .Select(k => k.Name)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        return Ok(new { items = skills });
    }

    [HttpGet]
    public async Task<IActionResult> GetVacancies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? skillFilter = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = dbContext.RawVacancies.Include(r => r.KeySkills).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r =>
                r.VacancyName.Contains(search) ||
                r.VacancyDescription.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(skillFilter))
        {
            query = query.Where(r =>
                r.KeySkills.Any(ks => ks.Name.Contains(skillFilter)));
        }

        var total = await query.CountAsync();
        var vacancies = await query
            .OrderByDescending(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            items = vacancies
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVacancy(Guid id)
    {
        var vacancy = await dbContext.RawVacancies
            .Include(r => r.KeySkills)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (vacancy == null)
            return NotFound();

        return Ok(vacancy);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancyRequest request)
    {
        var vacancy = new RawVacancy
        {
            VacancyName = request.VacancyName,
            VacancyDescription = request.VacancyDescription,
            Url = request.Url ?? string.Empty,
            IsProcessed = false
        };

        dbContext.RawVacancies.Add(vacancy);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVacancy), new { id = vacancy.Id }, vacancy);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] UpdateVacancyRequest request)
    {
        var vacancy = await dbContext.RawVacancies.FirstOrDefaultAsync(r => r.Id == id);

        if (vacancy == null)
            return NotFound();

        vacancy.VacancyName = request.VacancyName ?? vacancy.VacancyName;
        vacancy.VacancyDescription = request.VacancyDescription ?? vacancy.VacancyDescription;
        vacancy.Url = request.Url ?? vacancy.Url;
        vacancy.IsProcessed = request.IsProcessed ?? vacancy.IsProcessed;

        dbContext.RawVacancies.Update(vacancy);
        await dbContext.SaveChangesAsync();

        return Ok(vacancy);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVacancy(Guid id)
    {
        var vacancy = await dbContext.RawVacancies.FirstOrDefaultAsync(r => r.Id == id);

        if (vacancy == null)
            return NotFound();

        dbContext.RawVacancies.Remove(vacancy);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}

public record CreateVacancyRequest(
    string VacancyName,
    string VacancyDescription,
    string? Url);

public record UpdateVacancyRequest(
    string? VacancyName = null,
    string? VacancyDescription = null,
    string? Url = null,
    bool? IsProcessed = null);