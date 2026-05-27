using System.Text.Json;
using System.Text.Json.Serialization;
using Hephaestus.Application;
using Hephaestus.Features.OpenAiClients;
using Hephaestus.Features.OpenRouterClient;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Features.TrajectoryGeneration;

public class TrajectoryGenerationService(
    AppDbContext dbContext,
    IOpenRouterClient openRouterClient,
    TrajectoryPromptBuilder promptBuilder,
    ILogger<TrajectoryGenerationService> logger) : ITrajectoryGenerationService
{
    private const string Model = "openai/gpt-oss-120b:free";

    public async Task<TrajectoryResponse> GenerateAsync(TrajectoryGenerationRequest request, CancellationToken ct = default)
    {
        var profession = await dbContext.Professions
            .FirstOrDefaultAsync(p => p.Id == request.ProfessionId, ct)
            ?? throw new KeyNotFoundException($"Profession {request.ProfessionId} not found");

        var skills = await dbContext.CleanSkills
            .Where(s => s.ProfessionId == request.ProfessionId && !s.IsDeleted)
            .Include(s => s.ChildRelations).ThenInclude(r => r.ParentSkill)
            .ToListAsync(ct);

        if (skills.Count < 5)
            throw new InvalidOperationException(
                $"Profession '{profession.Name}' has only {skills.Count} skills. At least 5 required.");

        var systemPrompt = promptBuilder.BuildSystemPrompt();
        var userPrompt = promptBuilder.BuildUserPrompt(profession, skills, request.IncludeRelations);

        var messages = new List<ChatMessage>
        {
            new() { Role = "system", Content = systemPrompt },
            new() { Role = "user", Content = userPrompt }
        };

        logger.LogInformation(
            "Generating trajectory for profession '{Name}' ({SkillCount} skills)",
            profession.Name, skills.Count);

        var aiResponse = await openRouterClient.CreateChatCompletionAsync(
            Model, messages, maxTokens: 6000, temperature: 0.3);

        var rawContent = aiResponse.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;

        logger.LogDebug("AI raw response length: {Length}", rawContent.Length);

        return ParseAndEnrich(rawContent, profession.Id, profession.Name, skills);
    }

    private static TrajectoryResponse ParseAndEnrich(
        string rawJson,
        Guid professionId,
        string professionName,
        IReadOnlyList<Domain.Entities.CleanSkill> skills)
    {
        var skillIndex = skills.ToDictionary(
            s => s.DisplayName,
            s => s,
            StringComparer.OrdinalIgnoreCase);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        AiTrajectoryResult? parsed = null;
        try
        {
            var json = ExtractJson(rawJson);
            parsed = JsonSerializer.Deserialize<AiTrajectoryResult>(json, options);
        }
        catch (Exception ex)
        {
            // Return a degraded response so the caller still gets something useful
            return new TrajectoryResponse
            {
                ProfessionId = professionId,
                ProfessionName = professionName,
                AiComment = $"Не удалось распарсить ответ ИИ: {ex.Message}",
                Tracks = new()
            };
        }

        if (parsed?.Tracks == null)
        {
            return new TrajectoryResponse
            {
                ProfessionId = professionId,
                ProfessionName = professionName,
                AiComment = "ИИ вернул пустой ответ.",
                Tracks = new()
            };
        }

        var tracks = parsed.Tracks.Select(t => new TrajectoryTrack
        {
            Name = t.Name ?? string.Empty,
            Description = t.Description ?? string.Empty,
            Nodes = (t.Nodes ?? new()).Select((n, idx) =>
            {
                skillIndex.TryGetValue(n.DisplayName ?? string.Empty, out var skill);
                return new TrajectoryNode
                {
                    Id = skill?.Id ?? Guid.Empty,
                    DisplayName = n.DisplayName ?? string.Empty,
                    SkillType = skill?.SkillType?.ToString(),
                    Direction = skill?.Direction?.ToString(),
                    Level = n.Level ?? string.Empty,
                    Order = n.Order > 0 ? n.Order : idx + 1,
                    IsCore = n.IsCore,
                    Reason = n.Reason ?? string.Empty
                };
            }).OrderBy(n => n.Order).ToList(),
            Edges = (t.Edges ?? new()).Select(e => new TrajectoryEdge
            {
                From = e.From ?? string.Empty,
                To = e.To ?? string.Empty,
                Type = e.Type ?? "Prerequisite"
            }).ToList()
        }).ToList();

        return new TrajectoryResponse
        {
            ProfessionId = professionId,
            ProfessionName = professionName,
            Tracks = tracks,
            AiComment = parsed.AiComment
        };
    }

    private static string ExtractJson(string raw)
    {
        var start = raw.IndexOf('{');
        var end = raw.LastIndexOf('}');
        return start >= 0 && end > start ? raw[start..(end + 1)] : raw;
    }

    // ── Internal AI response models ──────────────────────────────────────────

    private class AiTrajectoryResult
    {
        public List<AiTrack>? Tracks { get; set; }
        public string? AiComment { get; set; }
    }

    private class AiTrack
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<AiNode>? Nodes { get; set; }
        public List<AiEdge>? Edges { get; set; }
    }

    private class AiNode
    {
        public string? DisplayName { get; set; }
        public string? Level { get; set; }
        public int Order { get; set; }
        public bool IsCore { get; set; }
        public string? Reason { get; set; }
    }

    private class AiEdge
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Type { get; set; }
    }
}
