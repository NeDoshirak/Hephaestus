namespace Hephaestus.Domain.Entities;

public class SkillOnReview
{
    public Guid Id { get; } = Guid.NewGuid();
    public string OriginalName { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public int Counter { get; set; } = 1;
    public string Status { get; set; } = "pending";
    public string SuggestedDisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
