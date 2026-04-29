using Hephaestus.Domain.Entities;
using Hephaestus.Features.VacancyAiParsing.Dtos;

namespace Hephaestus.Features.VacancyAiParsing;

public interface IVacancyAiParsingService
{
    Task<List<ParsedSkillDto>> ParseVacanciesAsync(
        IEnumerable<RawVacancy> vacancies,
        CancellationToken ct = default);
}
