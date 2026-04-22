namespace Hephaestus.Features.SkillManagement;

public interface ISkillImportService
{
    Task<SkillImportResult> ImportFromUnprocessedVacanciesAsync(string? vacancyNameFilter = null, int? limit = null);
}

public class SkillImportResult
{
    public int VacanciesProcessed { get; set; }
    public int SkillsAddedToReview { get; set; }
    public int SkillsMatchedExisting { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
