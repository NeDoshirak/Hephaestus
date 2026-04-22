namespace Hephaestus.Features.SkillManagement;

public class CleanSkillDto
{
    public Guid Id { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Counter { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SkillSynonymDto> Synonyms { get; set; } = new();
    public List<SkillDependencyDto> DependentSkills { get; set; } = new();
    public List<SkillDependencyDto> ParentSkills { get; set; } = new();
}

public class SkillSynonymDto
{
    public Guid Id { get; set; }
    public string SynonymName { get; set; } = string.Empty;
    public bool IsFromNormalization { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SkillOnReviewDto
{
    public Guid Id { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public int Counter { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SuggestedDisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SkillDependencyDto
{
    public Guid Id { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
}

public class AddCleanSkillRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? NormalizedName { get; set; }
}
