using System.Text.Json;
using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Hephaestus.Domain.Enums;
using Hephaestus.Features.OpenAiClients;
using Hephaestus.Features.OpenRouterClient;
using Hephaestus.Features.VacancyAiParsing.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Features.VacancyAiParsing;

public class VacancyAiParsingService(
    IOpenRouterClient openRouterClient,
    IVacancyParsingPromptBuilder promptBuilder,
    AppDbContext dbContext,
    ILogger<VacancyAiParsingService> logger) : IVacancyAiParsingService
{
    private const int BatchSize = 1;
    private const string Model = "openai/gpt-oss-120b:free";
    private const int DelayBetweenRequests = 500;
    private const int MaxDescriptionLength = 800;

    public async Task<List<ParsedSkillDto>> ParseVacanciesAsync(
        IEnumerable<RawVacancy> vacancies,
        CancellationToken ct = default)
    {
        var allSkills = new List<ParsedSkillDto>();
        var vacancyList = vacancies.ToList();

        if (!vacancyList.Any())
            return allSkills;

        var professions = await dbContext.Professions.ToListAsync(ct);
        var batches = vacancyList
            .Chunk(BatchSize)
            .ToList();

        foreach (var batch in batches)
        {
            try
            {
                logger.LogInformation("Processing batch of {Count} vacancies with AI", batch.Length);

                var systemPrompt = promptBuilder.BuildSystemPrompt(professions);
                var userPrompt = promptBuilder.BuildUserPrompt(batch);

                var messages = new List<ChatMessage>
                {
                    new() { Role = "system", Content = systemPrompt },
                    new() { Role = "user", Content = userPrompt }
                };

                var response = await openRouterClient.CreateChatCompletionAsync(Model, messages, maxTokens: 1024, temperature: 0.1);

                await Task.Delay(DelayBetweenRequests);

                var responseText = response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
                var skills = ParseResponse(responseText);
                allSkills.AddRange(skills);

                logger.LogInformation("Parsed {SkillCount} skills from batch", skills.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Skipping batch due to parsing error: {Message}", ex.Message);
            }
        }

        return allSkills;
    }

    private List<ParsedSkillDto> ParseResponse(string response)
    {
        var skills = new List<ParsedSkillDto>();

        try
        {
            logger.LogDebug("AI Response (first 500 chars): {Response}", response[..Math.Min(500, response.Length)]);

            var jsonResponse = ExtractJsonFromResponse(response);
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                logger.LogWarning("Could not extract JSON from response. Full response: {FullResponse}", response);
                return skills;
            }

            var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            if (!root.TryGetProperty("skills", out var skillsArray))
            {
                logger.LogWarning("Response missing 'skills' property");
                return skills;
            }

            foreach (var skillElement in skillsArray.EnumerateArray())
            {
                try
                {
                    var skill = ParseSkillElement(skillElement);
                    if (skill != null)
                        skills.Add(skill);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to parse individual skill from response");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing JSON response from Groq");
        }

        return skills;
    }

    private ParsedSkillDto? ParseSkillElement(JsonElement skillElement)
    {
        var skill = new ParsedSkillDto();

        if (!skillElement.TryGetProperty("name", out var nameElement) || nameElement.ValueKind == JsonValueKind.Null)
            return null;

        skill.Name = nameElement.GetString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(skill.Name))
            return null;

        if (skillElement.TryGetProperty("type", out var typeElement) && typeElement.ValueKind == JsonValueKind.String)
        {
            var typeStr = typeElement.GetString();
            if (Enum.TryParse<SkillType>(typeStr, true, out var type))
                skill.Type = type;
        }

        if (skillElement.TryGetProperty("direction", out var directionElement) && directionElement.ValueKind == JsonValueKind.String)
        {
            var dirStr = directionElement.GetString();
            if (Enum.TryParse<Direction>(dirStr, true, out var direction))
                skill.Direction = direction;
        }

        if (skillElement.TryGetProperty("level", out var levelElement))
        {
            if (levelElement.ValueKind == JsonValueKind.Number && levelElement.TryGetInt32(out var levelNum))
            {
                skill.Level = ConvertLevelNumberToEnum(levelNum);
            }
            else if (levelElement.ValueKind == JsonValueKind.String)
            {
                var levelStr = levelElement.GetString();
                if (Enum.TryParse<SkillLevel>(levelStr, true, out var level))
                    skill.Level = level;
            }
        }

        return skill;
    }

    private SkillLevel? ConvertLevelNumberToEnum(int levelNum)
    {
        return levelNum switch
        {
            1 => SkillLevel.Junior,
            2 => SkillLevel.Middle,
            3 => SkillLevel.Senior,
            4 => SkillLevel.Lead,
            _ => null
        };
    }

    private string ExtractJsonFromResponse(string response)
    {
        var startIndex = response.IndexOf('{');
        var endIndex = response.LastIndexOf('}');

        if (startIndex >= 0 && endIndex > startIndex)
        {
            return response.Substring(startIndex, endIndex - startIndex + 1);
        }

        return string.Empty;
    }
}
