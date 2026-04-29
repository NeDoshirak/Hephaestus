using Hephaestus.Domain.Entities;

namespace Hephaestus.Features.VacancyAiParsing;

public interface ISkillExtractionService
{
    Task<Dictionary<string, int>> ExtractSkillsAsync(
        IEnumerable<RawVacancy> vacancies,
        CancellationToken ct = default);
}
