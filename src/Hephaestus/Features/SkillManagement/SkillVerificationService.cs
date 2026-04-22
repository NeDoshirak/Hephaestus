using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Features.SkillManagement;

public class SkillVerificationService(
    AppDbContext dbContext,
    ILogger<SkillVerificationService> logger) : ISkillVerificationService
{
    public async Task<ApproveSkillResult> ApproveSkillAsync(Guid skillOnReviewId, ApproveSkillRequest request)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var skillOnReview = await dbContext.SkillsOnReview
                .FirstOrDefaultAsync(s => s.Id == skillOnReviewId);

            if (skillOnReview == null)
                throw new InvalidOperationException($"Skill on review with id {skillOnReviewId} not found");

            CleanSkill cleanSkill;

            if (request.ExistingCleanSkillId.HasValue)
            {
                cleanSkill = await dbContext.CleanSkills
                    .FirstOrDefaultAsync(c => c.Id == request.ExistingCleanSkillId.Value);

                if (cleanSkill == null)
                    throw new InvalidOperationException($"Clean skill with id {request.ExistingCleanSkillId} not found");

                cleanSkill.Counter += skillOnReview.Counter;

                var skillSynonym = new SkillSynonym
                {
                    CleanSkillId = cleanSkill.Id,
                    SynonymName = skillOnReview.NormalizedName,
                    IsFromNormalization = false
                };
                await dbContext.SkillSynonyms.AddAsync(skillSynonym);
            }
            else
            {
                cleanSkill = new CleanSkill
                {
                    NormalizedName = skillOnReview.NormalizedName,
                    DisplayName = request.DisplayName,
                    Description = request.Description,
                    Counter = skillOnReview.Counter
                };

                await dbContext.CleanSkills.AddAsync(cleanSkill);
                await dbContext.SaveChangesAsync();

                foreach (var synonym in request.Synonyms)
                {
                    var skillSynonym = new SkillSynonym
                    {
                        CleanSkillId = cleanSkill.Id,
                        SynonymName = synonym,
                        IsFromNormalization = false
                    };
                    await dbContext.SkillSynonyms.AddAsync(skillSynonym);
                }

                foreach (var childRelation in request.Children)
                {
                    var parentSkill = await dbContext.CleanSkills
                        .FirstOrDefaultAsync(c => c.NormalizedName == childRelation.ParentNormalizedName);

                    if (parentSkill != null && parentSkill.Id != cleanSkill.Id)
                    {
                        var relation = new SkillRelation
                        {
                            ParentSkillId = parentSkill.Id,
                            ChildSkillId = cleanSkill.Id,
                            RelationType = childRelation.RelationType
                        };
                        await dbContext.SkillRelations.AddAsync(relation);
                    }
                }
            }

            dbContext.SkillsOnReview.Remove(skillOnReview);

            var duplicates = await dbContext.SkillsOnReview
                .Where(s => s.NormalizedName == skillOnReview.NormalizedName && s.Id != skillOnReviewId)
                .ToListAsync();

            int duplicatesProcessed = 0;
            foreach (var duplicate in duplicates)
            {
                cleanSkill.Counter += duplicate.Counter;
                dbContext.SkillsOnReview.Remove(duplicate);
                duplicatesProcessed++;
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ApproveSkillResult
            {
                CleanSkillId = cleanSkill.Id,
                NormalizedName = cleanSkill.NormalizedName,
                DisplayName = cleanSkill.DisplayName,
                DuplicatesProcessed = duplicatesProcessed
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Ошибка при аппруве навыка: {EMessage}", e.Message);
            throw;
        }
    }
}
