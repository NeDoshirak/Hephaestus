using Hephaestus.Domain.Enums;

namespace Hephaestus.Domain.Entities;

public class Profession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ProfessionDirection Direction { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CleanSkill> CleanSkills { get; } = [];
    public ICollection<SkillOnReview> SkillsOnReview { get; } = [];
}
