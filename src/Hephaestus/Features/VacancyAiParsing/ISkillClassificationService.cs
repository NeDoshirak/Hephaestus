using Hephaestus.Features.VacancyAiParsing.Dtos;
using Hephaestus.Domain.Entities;

namespace Hephaestus.Features.VacancyAiParsing;

public interface ISkillClassificationService
{
    Task<List<ParsedSkillDto>> ClassifySkillsAsync(
        Dictionary<string, int> skillsWithCounts,
        IEnumerable<Profession> professions,
        CancellationToken ct = default);
}
