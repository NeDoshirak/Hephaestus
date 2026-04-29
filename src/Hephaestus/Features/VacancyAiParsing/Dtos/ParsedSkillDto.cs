using Hephaestus.Domain.Enums;

namespace Hephaestus.Features.VacancyAiParsing.Dtos;

public class ParsedSkillDto
{
    public string Name { get; set; } = string.Empty;
    public SkillType? Type { get; set; }
    public Direction? Direction { get; set; }
    public SkillLevel? Level { get; set; }
    public List<Guid> ProfessionIds { get; set; } = new();
    public int Count { get; set; } = 1;
}
