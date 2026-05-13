namespace Hephaestus.Features.SkillReviewMcp;

public record SkillOnReviewMcpDto(
    string Id,
    string OriginalName,
    string NormalizedName,
    int Counter,
    string SuggestedDisplayName,
    string? SkillType,
    string? Direction,
    string? Level);

public record CleanSkillMcpDto(
    string Id,
    string NormalizedName,
    string DisplayName,
    string? Description,
    int Counter,
    string? SkillType,
    IReadOnlyList<string> Synonyms,
    IReadOnlyList<string> Parents,
    IReadOnlyList<string> Children);
