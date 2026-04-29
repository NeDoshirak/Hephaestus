using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Hephaestus.Domain.Enums;
using Hephaestus.Features.VacancyAiParsing;
using Hephaestus.Features.VacancyAiParsing.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Features.SkillManagement;

public class SkillImportService(
    ISkillNormalizationService normalizationService,
    ISkillExtractionService extractionService,
    ISkillClassificationService classificationService,
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

            logger.LogInformation("Found {Count} unprocessed vacancies", vacancies.Count);

            if (vacancies.Count == 0)
            {
                logger.LogInformation("No unprocessed vacancies found");
                return result;
            }

            logger.LogInformation("Starting two-stage skill extraction and classification");

            try
            {
                var professions = await dbContext.Professions.ToListAsync();

                logger.LogInformation("Stage 1: Extracting skill names from {Count} vacancies", vacancies.Count);
                var extractedSkillsWithCounts = await extractionService.ExtractSkillsAsync(vacancies);
                logger.LogInformation("Extracted {SkillCount} unique skill names", extractedSkillsWithCounts.Count);

                if (extractedSkillsWithCounts.Count == 0)
                {
                    foreach (var vacancy in vacancies)
                    {
                        vacancy.IsProcessed = true;
                    }
                    await dbContext.SaveChangesAsync();
                    logger.LogInformation("No skills extracted, marking vacancies as processed");
                    return result;
                }

                logger.LogInformation("Stage 2: Classifying {SkillCount} skills", extractedSkillsWithCounts.Count);
                var classifiedSkills = await classificationService.ClassifySkillsAsync(extractedSkillsWithCounts, professions);
                logger.LogInformation("Classified {SkillCount} skills with metadata", classifiedSkills.Count);

                var batchSkillsAdded = 0;
                foreach (var skill in classifiedSkills)
                {
                    var normalizedName = normalizationService.Normalize(skill.Name);

                    if (string.IsNullOrWhiteSpace(normalizedName))
                        continue;

                    var existingClean = await dbContext.CleanSkills
                        .FirstOrDefaultAsync(c => c.NormalizedName == normalizedName);

                    if (existingClean != null)
                    {
                        existingClean.Counter += skill.Count;
                        if (skill.ProfessionIds.Count > 0)
                        {
                            existingClean.ProfessionId = skill.ProfessionIds.FirstOrDefault();
                        }
                        result.SkillsMatchedExisting++;
                    }
                    else
                    {
                        var existingReview = await dbContext.SkillsOnReview
                            .FirstOrDefaultAsync(s => s.NormalizedName == normalizedName);

                        if (existingReview != null)
                        {
                            existingReview.Counter += skill.Count;
                            if (skill.ProfessionIds.Count > 0)
                            {
                                existingReview.ProfessionId = skill.ProfessionIds.FirstOrDefault();
                            }
                        }
                        else
                        {
                            var skillOnReview = new SkillOnReview
                            {
                                OriginalName = skill.Name,
                                NormalizedName = normalizedName,
                                SuggestedDisplayName = skill.Name,
                                SkillType = skill.Type,
                                Direction = skill.Direction,
                                Level = skill.Level,
                                ProfessionId = skill.ProfessionIds.Count > 0 ? skill.ProfessionIds.FirstOrDefault() : null,
                                Counter = skill.Count,
                                Status = "pending"
                            };

                            await dbContext.SkillsOnReview.AddAsync(skillOnReview);
                            result.SkillsAddedToReview++;
                            batchSkillsAdded++;
                        }
                    }
                }

                foreach (var vacancy in vacancies)
                {
                    vacancy.IsProcessed = true;
                }

                logger.LogInformation("Saving results: {SkillsAdded} new skills, {VacanciesCount} vacancies marked", batchSkillsAdded, vacancies.Count);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("All skills classified and saved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при импорте навыков: {Message}", ex.Message);
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при импорте навыков: {Message}", ex.Message);
        }

        return result;
    }

    private static T? ParseEnum<T>(string? value) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (Enum.TryParse<T>(value, true, out var result))
            return result;

        return null;
    }
}
