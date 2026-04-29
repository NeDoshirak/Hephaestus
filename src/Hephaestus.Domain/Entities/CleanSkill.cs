using Hephaestus.Domain.Enums;

namespace Hephaestus.Domain.Entities;

public class CleanSkill
{
    public Guid Id { get; } = Guid.NewGuid();
    public string NormalizedName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Counter { get; set; } = 1;
    public SkillType? SkillType { get; set; }
    public Direction? Direction { get; set; }
    public SkillLevel? Level { get; set; }
    public Guid? ProfessionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SkillSynonym> Synonyms { get; set; } = new List<SkillSynonym>();
    public ICollection<SkillRelation> ParentRelations { get; set; } = new List<SkillRelation>();
    public ICollection<SkillRelation> ChildRelations { get; set; } = new List<SkillRelation>();
    public Profession? Profession { get; set; }
}
