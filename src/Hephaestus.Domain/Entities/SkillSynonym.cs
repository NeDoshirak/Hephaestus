namespace Hephaestus.Domain.Entities;

public class SkillSynonym
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid CleanSkillId { get; set; }
    public string SynonymName { get; set; } = string.Empty;
    public bool IsFromNormalization { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CleanSkill? CleanSkill { get; set; }
}
