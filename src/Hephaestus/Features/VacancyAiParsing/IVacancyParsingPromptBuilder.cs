using Hephaestus.Domain.Entities;

namespace Hephaestus.Features.VacancyAiParsing;

public interface IVacancyParsingPromptBuilder
{
    string BuildSystemPrompt(IEnumerable<Profession> professions);
    string BuildUserPrompt(IEnumerable<RawVacancy> vacancies);
}
