namespace Hephaestus.Features.SkillManagement;

public interface ISkillVerificationService
{
    Task<ApproveSkillResult> ApproveSkillAsync(Guid skillOnReviewId, ApproveSkillRequest request);
}

public class ApproveSkillRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Synonyms { get; set; } = new();
    public List<ChildSkillRelation> Children { get; set; } = new();
    public Guid? ExistingCleanSkillId { get; set; }
}

public class ChildSkillRelation
{
    public string ParentNormalizedName { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
}

public class ApproveSkillResult
{
    public Guid CleanSkillId { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int DuplicatesProcessed { get; set; }
}
