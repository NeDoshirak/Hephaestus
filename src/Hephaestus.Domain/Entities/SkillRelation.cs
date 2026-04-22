namespace Hephaestus.Domain.Entities;

public class SkillRelation
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid ParentSkillId { get; set; }
    public Guid ChildSkillId { get; set; }
    public string RelationType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CleanSkill? ParentSkill { get; set; }
    public CleanSkill? ChildSkill { get; set; }
}
