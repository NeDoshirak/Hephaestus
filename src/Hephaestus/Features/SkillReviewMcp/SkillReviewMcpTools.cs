using System.ComponentModel;
using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Hephaestus.Domain.Enums;
using Hephaestus.Features.SkillManagement;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace Hephaestus.Features.SkillReviewMcp;

[McpServerToolType]
public class SkillReviewMcpTools(
    AppDbContext dbContext,
    ISkillVerificationService verificationService,
    ISkillNormalizationService normalizationService,
    ILogger<SkillReviewMcpTools> logger)
{
    // ─────────────────────────────────────────────────────────────
    // Ревью новых навыков (SkillsOnReview)
    // ─────────────────────────────────────────────────────────────

    [McpServerTool]
    [Description("Возвращает список навыков, ожидающих проверки, отсортированных по частоте встречаемости. " +
                 "Вызывай в начале каждой сессии ревью.")]
    public async Task<IReadOnlyList<SkillOnReviewMcpDto>> GetSkillsOnReview(int limit = 50)
    {
        var skills = await dbContext.SkillsOnReview
            .OrderByDescending(s => s.Counter)
            .Take(limit)
            .ToListAsync();

        return skills.Select(s => new SkillOnReviewMcpDto(
            s.Id.ToString(),
            s.OriginalName,
            s.NormalizedName,
            s.Counter,
            s.SuggestedDisplayName,
            s.SkillType?.ToString(),
            s.Direction?.ToString(),
            s.Level?.ToString()
        )).ToList();
    }

    [McpServerTool]
    [Description("Ищет существующие чистые навыки по нормализованному имени, отображаемому имени или синониму. " +
                 "Используй перед принятием решения, чтобы найти кандидатов на слияние или родительскую категорию.")]
    public async Task<IReadOnlyList<CleanSkillMcpDto>> SearchCleanSkills(string query, int limit = 10)
    {
        var normalized = normalizationService.Normalize(query);
        var queryLower = query.ToLower();

        var skills = await dbContext.CleanSkills
            .Include(c => c.Synonyms)
            .Include(c => c.ParentRelations).ThenInclude(r => r.ChildSkill)
            .Include(c => c.ChildRelations).ThenInclude(r => r.ParentSkill)
            .Where(c =>
                c.NormalizedName.Contains(normalized) ||
                c.DisplayName.ToLower().Contains(queryLower) ||
                c.Synonyms.Any(s => s.SynonymName.Contains(normalized)))
            .OrderByDescending(c => c.Counter)
            .Take(limit)
            .ToListAsync();

        return skills.Select(ToDto).ToList();
    }

    [McpServerTool]
    [Description("Возвращает полную информацию об одном чистом навыке по его ID, " +
                 "включая синонимы и связанные родительские/дочерние навыки.")]
    public async Task<CleanSkillMcpDto?> GetSkillDetail(string cleanSkillId)
    {
        if (!Guid.TryParse(cleanSkillId, out var id))
            throw new ArgumentException($"Некорректный GUID: {cleanSkillId}");

        var skill = await dbContext.CleanSkills
            .Include(c => c.Synonyms)
            .Include(c => c.ParentRelations).ThenInclude(r => r.ChildSkill)
            .Include(c => c.ChildRelations).ThenInclude(r => r.ParentSkill)
            .FirstOrDefaultAsync(c => c.Id == id);

        return skill is null ? null : ToDto(skill);
    }

    [McpServerTool]
    [Description("Нормализует произвольное имя навыка по правилам платформы " +
                 "(транслитерация, удаление версий, спецсимволы). " +
                 "Используй чтобы получить normalizedName перед поиском.")]
    public string NormalizeSkillName(string rawName)
        => normalizationService.Normalize(rawName);

    [McpServerTool]
    [Description("Одобряет навык на проверке как синоним существующего чистого навыка. " +
                 "Используй когда навык — это другое написание уже существующего: 'JS' → 'JavaScript', 'Питон' → 'Python'.")]
    public async Task<ApproveSkillResult> ApproveAsSynonym(string skillOnReviewId, string targetCleanSkillId)
    {
        if (!Guid.TryParse(skillOnReviewId, out var reviewId))
            throw new ArgumentException($"Некорректный GUID skillOnReviewId: {skillOnReviewId}");
        if (!Guid.TryParse(targetCleanSkillId, out var targetId))
            throw new ArgumentException($"Некорректный GUID targetCleanSkillId: {targetCleanSkillId}");

        var skillOnReview = await dbContext.SkillsOnReview.FirstOrDefaultAsync(s => s.Id == reviewId)
            ?? throw new InvalidOperationException($"Навык на ревью {reviewId} не найден");

        var synonymExists = await dbContext.SkillSynonyms
            .AnyAsync(s => s.CleanSkillId == targetId && s.SynonymName == skillOnReview.NormalizedName);

        if (synonymExists)
            throw new InvalidOperationException(
                $"Синоним '{skillOnReview.NormalizedName}' уже существует для навыка {targetId}");

        return await verificationService.ApproveSkillAsync(reviewId, new ApproveSkillRequest
        {
            ExistingCleanSkillId = targetId
        });
    }

    [McpServerTool]
    [Description("Одобряет навык на проверке как новый самостоятельный чистый навык. " +
                 "Можно сразу указать родительский навык через parentNormalizedName. " +
                 "relationType — тип связи, например 'IsA' или 'BelongsTo'. " +
                 "skillType: Soft|Hard|Tool|Framework|Language|DomainKnowledge. " +
                 "direction: Programming|Analytics|Testing|Design|DevOps|DataScience|Management|General.")]
    public async Task<ApproveSkillResult> ApproveAsNewSkill(
        string skillOnReviewId,
        string displayName,
        string? description = null,
        string? skillType = null,
        string? direction = null,
        string? level = null,
        string? parentNormalizedName = null,
        string? relationType = null)
    {
        if (!Guid.TryParse(skillOnReviewId, out var reviewId))
            throw new ArgumentException($"Некорректный GUID: {skillOnReviewId}");

        var request = new ApproveSkillRequest
        {
            DisplayName = displayName,
            Description = description,
            SkillType = skillType,
            Direction = direction,
            Level = level
        };

        if (!string.IsNullOrEmpty(parentNormalizedName))
        {
            request.Children.Add(new ChildSkillRelation
            {
                ParentNormalizedName = parentNormalizedName,
                RelationType = relationType ?? "BelongsTo"
            });
        }

        return await verificationService.ApproveSkillAsync(reviewId, request);
    }

    [McpServerTool]
    [Description("Создаёт новую родительскую категорию и одобряет навык на проверке как её дочерний элемент. " +
                 "Используй когда навык принадлежит к категории, которой ещё нет в базе: " +
                 "например создать 'Системы контроля версий' и добавить 'Git' как дочерний. " +
                 "Если категория уже есть — используй ApproveAsNewSkill с parentNormalizedName.")]
    public async Task<object> ApproveWithNewParent(
        string skillOnReviewId,
        string childDisplayName,
        string parentDisplayName,
        string? childDescription = null,
        string? childSkillType = null,
        string? parentDescription = null,
        string? parentSkillType = null,
        string? direction = null,
        string? relationType = null)
    {
        if (!Guid.TryParse(skillOnReviewId, out var reviewId))
            throw new ArgumentException($"Некорректный GUID: {skillOnReviewId}");

        var parentNormalized = normalizationService.Normalize(parentDisplayName);

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var existingParent = await dbContext.CleanSkills
                .FirstOrDefaultAsync(c => c.NormalizedName == parentNormalized);

            CleanSkill parent;
            if (existingParent is null)
            {
                parent = new CleanSkill
                {
                    NormalizedName = parentNormalized,
                    DisplayName = parentDisplayName,
                    Description = parentDescription,
                    SkillType = ParseEnum<SkillType>(parentSkillType),
                    Direction = ParseEnum<Direction>(direction)
                };
                await dbContext.CleanSkills.AddAsync(parent);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Создана родительская категория '{Name}' ({Id})", parentDisplayName, parent.Id);
            }
            else
            {
                parent = existingParent;
                logger.LogInformation("Родительская категория '{Name}' уже существует, используем её", parentDisplayName);
            }

            var result = await verificationService.ApproveSkillAsync(reviewId, new ApproveSkillRequest
            {
                DisplayName = childDisplayName,
                Description = childDescription,
                SkillType = childSkillType,
                Direction = direction,
                Children =
                [
                    new ChildSkillRelation
                    {
                        ParentNormalizedName = parent.NormalizedName,
                        RelationType = relationType ?? "BelongsTo"
                    }
                ]
            });

            await transaction.CommitAsync();

            return new
            {
                ParentSkillId = parent.Id.ToString(),
                ParentNormalizedName = parent.NormalizedName,
                ChildSkillId = result.CleanSkillId.ToString(),
                ChildNormalizedName = result.NormalizedName
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [McpServerTool]
    [Description("Отклоняет навык на проверке (жёсткое удаление — навык ещё не был одобрен). " +
                 "Используй для спама, устаревших технологий, дублей с уже существующим синонимом. " +
                 "Укажи reason для журнала аудита.")]
    public async Task<object> RejectPendingSkill(string skillOnReviewId, string reason)
    {
        if (!Guid.TryParse(skillOnReviewId, out var id))
            throw new ArgumentException($"Некорректный GUID: {skillOnReviewId}");

        var skill = await dbContext.SkillsOnReview.FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new InvalidOperationException($"Навык на ревью {id} не найден");

        logger.LogInformation(
            "Отклонён навык на ревью '{Name}' ({Id}). Причина: {Reason}",
            skill.OriginalName, id, reason);

        dbContext.SkillsOnReview.Remove(skill);
        await dbContext.SaveChangesAsync();

        return new { Success = true, SkillOnReviewId = skillOnReviewId, Reason = reason };
    }

    // ─────────────────────────────────────────────────────────────
    // Ревью существующих навыков (CleanSkills)
    // ─────────────────────────────────────────────────────────────

    [McpServerTool]
    [Description("Возвращает список уже одобренных навыков для актуализации. " +
                 "withoutDescription=true — только навыки без описания. " +
                 "Можно фильтровать по skillType и direction.")]
    public async Task<IReadOnlyList<CleanSkillMcpDto>> GetCleanSkillsForReview(
        int limit = 50,
        bool withoutDescription = false,
        string? skillType = null,
        string? direction = null)
    {
        var query = dbContext.CleanSkills
            .Include(c => c.Synonyms)
            .Include(c => c.ParentRelations).ThenInclude(r => r.ChildSkill)
            .Include(c => c.ChildRelations).ThenInclude(r => r.ParentSkill)
            .AsQueryable();

        if (withoutDescription)
            query = query.Where(c => c.Description == null || c.Description == string.Empty);

        if (!string.IsNullOrEmpty(skillType) && Enum.TryParse<SkillType>(skillType, true, out var st))
            query = query.Where(c => c.SkillType == st);

        if (!string.IsNullOrEmpty(direction) && Enum.TryParse<Direction>(direction, true, out var dir))
            query = query.Where(c => c.Direction == dir);

        var skills = await query
            .OrderByDescending(c => c.Counter)
            .Take(limit)
            .ToListAsync();

        return skills.Select(ToDto).ToList();
    }

    [McpServerTool]
    [Description("Обновляет метаданные существующего одобренного навыка. " +
                 "Передавай только те поля, которые нужно изменить (остальные останутся прежними). " +
                 "skillType: Soft|Hard|Tool|Framework|Language|DomainKnowledge. " +
                 "direction: Programming|Analytics|Testing|Design|DevOps|DataScience|Management|General. " +
                 "level: Junior|Middle|Senior|Lead.")]
    public async Task<CleanSkillMcpDto> UpdateCleanSkill(
        string cleanSkillId,
        string? displayName = null,
        string? description = null,
        string? skillType = null,
        string? direction = null,
        string? level = null)
    {
        if (!Guid.TryParse(cleanSkillId, out var id))
            throw new ArgumentException($"Некорректный GUID: {cleanSkillId}");

        var skill = await dbContext.CleanSkills
            .Include(c => c.Synonyms)
            .Include(c => c.ParentRelations).ThenInclude(r => r.ChildSkill)
            .Include(c => c.ChildRelations).ThenInclude(r => r.ParentSkill)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new InvalidOperationException($"Навык {id} не найден");

        if (displayName is not null) skill.DisplayName = displayName;
        if (description is not null) skill.Description = description;
        if (skillType is not null) skill.SkillType = ParseEnum<SkillType>(skillType);
        if (direction is not null) skill.Direction = ParseEnum<Direction>(direction);
        if (level is not null) skill.Level = ParseEnum<SkillLevel>(level);
        skill.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return ToDto(skill);
    }

    [McpServerTool]
    [Description("Сливает два чистых навыка: source становится синонимом target, " +
                 "его Counter переносится на target, связи SkillRelation переносятся. " +
                 "Используй для устранения дублей среди уже одобренных навыков.")]
    public async Task<object> MergeCleanSkills(string sourceCleanSkillId, string targetCleanSkillId)
    {
        if (!Guid.TryParse(sourceCleanSkillId, out var sourceId))
            throw new ArgumentException($"Некорректный GUID sourceCleanSkillId: {sourceCleanSkillId}");
        if (!Guid.TryParse(targetCleanSkillId, out var targetId))
            throw new ArgumentException($"Некорректный GUID targetCleanSkillId: {targetCleanSkillId}");
        if (sourceId == targetId)
            throw new ArgumentException("source и target не могут совпадать");

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var source = await dbContext.CleanSkills
                .Include(c => c.Synonyms)
                .Include(c => c.ParentRelations)
                .Include(c => c.ChildRelations)
                .FirstOrDefaultAsync(c => c.Id == sourceId)
                ?? throw new InvalidOperationException($"Навык {sourceId} не найден");

            var target = await dbContext.CleanSkills
                .FirstOrDefaultAsync(c => c.Id == targetId)
                ?? throw new InvalidOperationException($"Навык {targetId} не найден");

            // Перенос синонимов source → target
            foreach (var syn in source.Synonyms)
            {
                syn.CleanSkillId = targetId;
            }

            // Добавить normalizedName source как синоним target
            var synExists = await dbContext.SkillSynonyms
                .AnyAsync(s => s.CleanSkillId == targetId && s.SynonymName == source.NormalizedName);
            if (!synExists)
            {
                await dbContext.SkillSynonyms.AddAsync(new SkillSynonym
                {
                    CleanSkillId = targetId,
                    SynonymName = source.NormalizedName,
                    IsFromNormalization = false
                });
            }

            // Перенос SkillRelation
            var existingTargetRelations = await dbContext.SkillRelations
                .Where(r => r.ParentSkillId == targetId || r.ChildSkillId == targetId)
                .ToListAsync();

            foreach (var rel in source.ParentRelations.ToList())
            {
                var alreadyExists = existingTargetRelations.Any(r =>
                    r.ParentSkillId == targetId && r.ChildSkillId == rel.ChildSkillId);
                if (!alreadyExists && rel.ChildSkillId != targetId)
                    rel.ParentSkillId = targetId;
                else
                    dbContext.SkillRelations.Remove(rel);
            }

            foreach (var rel in source.ChildRelations.ToList())
            {
                var alreadyExists = existingTargetRelations.Any(r =>
                    r.ChildSkillId == targetId && r.ParentSkillId == rel.ParentSkillId);
                if (!alreadyExists && rel.ParentSkillId != targetId)
                    rel.ChildSkillId = targetId;
                else
                    dbContext.SkillRelations.Remove(rel);
            }

            target.Counter += source.Counter;
            target.UpdatedAt = DateTime.UtcNow;

            // Soft-delete source
            source.IsDeleted = true;
            source.DeletedAt = DateTime.UtcNow;
            source.DeletedReason = $"Merged into {target.NormalizedName}";

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            logger.LogInformation(
                "Навык '{Source}' слит в '{Target}'",
                source.NormalizedName, target.NormalizedName);

            return new
            {
                SourceId = sourceCleanSkillId,
                SourceNormalizedName = source.NormalizedName,
                TargetId = targetCleanSkillId,
                TargetNormalizedName = target.NormalizedName,
                CounterTransferred = source.Counter
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [McpServerTool]
    [Description("Мягкое удаление одобренного навыка: помечает IsDeleted=true с причиной и датой. " +
                 "Навык перестаёт отображаться, но остаётся в БД для истории. " +
                 "Используй для устаревших, некорректных или ошибочно добавленных навыков.")]
    public async Task<object> SoftDeleteCleanSkill(string cleanSkillId, string reason)
    {
        if (!Guid.TryParse(cleanSkillId, out var id))
            throw new ArgumentException($"Некорректный GUID: {cleanSkillId}");

        var skill = await dbContext.CleanSkills
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new InvalidOperationException($"Навык {id} не найден");

        if (skill.IsDeleted)
            throw new InvalidOperationException($"Навык {id} уже удалён (причина: {skill.DeletedReason})");

        skill.IsDeleted = true;
        skill.DeletedAt = DateTime.UtcNow;
        skill.DeletedReason = reason;
        skill.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "Мягкое удаление навыка '{Name}' ({Id}). Причина: {Reason}",
            skill.NormalizedName, id, reason);

        return new { Success = true, CleanSkillId = cleanSkillId, Reason = reason };
    }

    [McpServerTool]
    [Description("Добавляет связь родитель-дочерний между двумя существующими чистыми навыками. " +
                 "relationType — тип связи: 'IsA', 'BelongsTo', 'Requires', 'Complements'. " +
                 "Используй для структурирования таксономии навыков.")]
    public async Task<object> AddSkillRelation(
        string parentCleanSkillId,
        string childCleanSkillId,
        string relationType = "BelongsTo")
    {
        if (!Guid.TryParse(parentCleanSkillId, out var parentId))
            throw new ArgumentException($"Некорректный GUID parentCleanSkillId: {parentCleanSkillId}");
        if (!Guid.TryParse(childCleanSkillId, out var childId))
            throw new ArgumentException($"Некорректный GUID childCleanSkillId: {childCleanSkillId}");
        if (parentId == childId)
            throw new ArgumentException("parent и child не могут совпадать");

        var parentExists = await dbContext.CleanSkills.AnyAsync(c => c.Id == parentId);
        var childExists = await dbContext.CleanSkills.AnyAsync(c => c.Id == childId);

        if (!parentExists) throw new InvalidOperationException($"Родительский навык {parentId} не найден");
        if (!childExists) throw new InvalidOperationException($"Дочерний навык {childId} не найден");

        var alreadyExists = await dbContext.SkillRelations
            .AnyAsync(r => r.ParentSkillId == parentId && r.ChildSkillId == childId);

        if (alreadyExists)
            throw new InvalidOperationException("Связь между этими навыками уже существует");

        var relation = new SkillRelation
        {
            ParentSkillId = parentId,
            ChildSkillId = childId,
            RelationType = relationType
        };

        await dbContext.SkillRelations.AddAsync(relation);
        await dbContext.SaveChangesAsync();

        return new
        {
            RelationId = relation.Id.ToString(),
            ParentCleanSkillId = parentCleanSkillId,
            ChildCleanSkillId = childCleanSkillId,
            RelationType = relationType
        };
    }

    // ─────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────

    private static CleanSkillMcpDto ToDto(CleanSkill s) => new(
        s.Id.ToString(),
        s.NormalizedName,
        s.DisplayName,
        s.Description,
        s.Counter,
        s.SkillType?.ToString(),
        s.Synonyms.Select(syn => syn.SynonymName).ToList(),
        s.ChildRelations.Select(r => r.ParentSkill?.NormalizedName ?? string.Empty).Where(n => n != string.Empty).ToList(),
        s.ParentRelations.Select(r => r.ChildSkill?.NormalizedName ?? string.Empty).Where(n => n != string.Empty).ToList()
    );

    private static T? ParseEnum<T>(string? value) where T : struct, Enum
        => !string.IsNullOrEmpty(value) && Enum.TryParse<T>(value, true, out var result) ? result : null;
}
