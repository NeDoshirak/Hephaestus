using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Features.SkillManagement;

public class SkillImportService(
    ISkillNormalizationService normalizationService,
    AppDbContext dbContext,
    ILogger<SkillImportService> logger) : ISkillImportService
{
    public async Task<SkillImportResult> ImportFromUnprocessedVacanciesAsync(string? vacancyNameFilter = null, int? limit = null)
    {
        var result = new SkillImportResult();

        try
        {
            var query = dbContext.RawVacancies
                .Include(r => r.KeySkills)
                .Where(r => !r.IsProcessed);

            if (!string.IsNullOrWhiteSpace(vacancyNameFilter))
            {
                query = query.Where(r => r.VacancyName.Contains(vacancyNameFilter));
            }

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            var vacancies = await query.ToListAsync();
            result.VacanciesProcessed = vacancies.Count;

            var allSkillNames = vacancies
                .SelectMany(v => v.KeySkills)
                .Select(ks => ks.Name)
                .ToList();

            var skillsByNormalized = new Dictionary<string, (string normalizedName, List<string> originalNames, int count)>();

            foreach (var skillName in allSkillNames)
            {
                var normalizedName = normalizationService.Normalize(skillName);

                if (string.IsNullOrWhiteSpace(normalizedName))
                    continue;

                if (skillsByNormalized.ContainsKey(normalizedName))
                {
                    var (norm, origNames, cnt) = skillsByNormalized[normalizedName];
                    if (!origNames.Contains(skillName))
                        origNames.Add(skillName);
                    skillsByNormalized[normalizedName] = (norm, origNames, cnt + 1);
                }
                else
                {
                    skillsByNormalized[normalizedName] = (normalizedName, new List<string> { skillName }, 1);
                }
            }

            foreach (var kvp in skillsByNormalized)
            {
                var normalizedName = kvp.Key;
                var (_, originalNames, count) = kvp.Value;

                var existingClean = await dbContext.CleanSkills
                    .FirstOrDefaultAsync(c => c.NormalizedName == normalizedName);

                if (existingClean != null)
                {
                    existingClean.Counter += count;
                    result.SkillsMatchedExisting++;
                }
                else
                {
                    var existingReview = await dbContext.SkillsOnReview
                        .FirstOrDefaultAsync(s => s.NormalizedName == normalizedName);

                    if (existingReview != null)
                    {
                        existingReview.Counter += count;
                    }
                    else
                    {
                        var skillOnReview = new SkillOnReview
                        {
                            OriginalName = originalNames.First(),
                            NormalizedName = normalizedName,
                            SuggestedDisplayName = originalNames.First(),
                            Counter = count,
                            Status = "pending"
                        };

                        await dbContext.SkillsOnReview.AddAsync(skillOnReview);
                        result.SkillsAddedToReview++;
                    }
                }
            }

            foreach (var vacancy in vacancies)
            {
                vacancy.IsProcessed = true;
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError("Ошибка при импорте навыков: {EMessage}", e.Message);
            throw;
        }

        return result;
    }
}
