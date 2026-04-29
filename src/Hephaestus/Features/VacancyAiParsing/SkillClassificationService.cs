using System.Text.Json;
using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Hephaestus.Domain.Enums;
using Hephaestus.Features.OpenAiClients;
using Hephaestus.Features.OpenRouterClient;
using Hephaestus.Features.VacancyAiParsing.Dtos;

namespace Hephaestus.Features.VacancyAiParsing;

public class SkillClassificationService(
    IOpenRouterClient openRouterClient,
    ILogger<SkillClassificationService> logger) : ISkillClassificationService
{
    private const string Model = "openai/gpt-oss-120b:free";
    private const int DelayBetweenRequests = 500;
    private const int BatchSize = 15;

    public async Task<List<ParsedSkillDto>> ClassifySkillsAsync(
        Dictionary<string, int> skillsWithCounts,
        IEnumerable<Profession> professions,
        CancellationToken ct = default)
    {
        var classifiedSkills = new List<ParsedSkillDto>();

        if (skillsWithCounts.Count == 0)
            return classifiedSkills;

        var professionList = professions.ToList();
        var professionNames = string.Join(", ", professionList.Select(p => p.Name));
        var professionNameToId = professionList.ToDictionary(p => p.Name, p => p.Id);

        var skillNames = skillsWithCounts.Keys.ToList();
        var batches = skillNames.Chunk(BatchSize).ToList();
        logger.LogInformation("Classifying {SkillCount} unique skills in {BatchCount} batches", skillNames.Count, batches.Count);

        foreach (var batch in batches)
        {
            try
            {
                logger.LogInformation("Classifying batch of {SkillCount} skills", batch.Length);

                var userPrompt = BuildClassificationPrompt(batch.ToList(), professionNames);
                var messages = new List<ChatMessage>
                {
                    new() { Role = "system", Content = GetClassificationSystemPrompt(professionNames) },
                    new() { Role = "user", Content = userPrompt }
                };

                var response = await openRouterClient.CreateChatCompletionAsync(Model, messages, maxTokens: 2048, temperature: 0.1);

                await Task.Delay(DelayBetweenRequests);

                var responseText = response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
                var batchResults = ParseClassificationResponse(responseText, professionNameToId);

                foreach (var skill in batchResults)
                {
                    if (skillsWithCounts.TryGetValue(skill.Name, out var count))
                    {
                        skill.Count = count;
                    }
                    classifiedSkills.Add(skill);
                }

                logger.LogInformation("Batch classified: {SkillCount} skills", batchResults.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error classifying batch: {Message}", ex.Message);
            }
        }

        logger.LogInformation("Total classified: {SkillCount} skills", classifiedSkills.Count);
        return classifiedSkills;
    }

    private string GetClassificationSystemPrompt(string professionNames)
    {
        var jsonExample = """{"skills": [{"name": "Python", "type": "Language", "direction": "Programming", "level": 1, "professions": ["Backend Developer", "Data Scientist"]}, {"name": "Communication", "type": "Soft", "direction": "General", "level": 2, "professions": []}]}""";

        return $"""
                You are a skill classification expert for job vacancies.
                Classify skills by assigning metadata.

                For EACH skill, assign:
                - Type: Language | Framework | Tool | Hard | Soft | DomainKnowledge
                - Direction: Programming | Analytics | Testing | Design | DevOps | DataScience | Management | General
                - Level: integer from 1 to 4
                  1 = Junior: entry-level skill, commonly taught, basic to intermediate knowledge required
                  2 = Middle: solid practical experience, confident independent work
                  3 = Senior: strong expertise, architecture decisions, mentoring
                  4 = Lead: technical leadership, sets standards, leads teams

                - Professions: list of professions from the available list where this skill is relevant
                  Available professions: {professionNames}
                  Return empty list if the skill is general/not profession-specific.

                PROFESSION ASSIGNMENT RULES:
                - ASSIGN professions ONLY for specialized/role-specific skills
                - LEAVE EMPTY for general/soft skills that apply to all roles

                Examples of skills that SHOULD have professions:
                - Python → ["Backend Developer", "Data Scientist"]
                - React → ["Frontend Developer", "Full Stack Developer"]
                - Testing → ["QA Engineer", "Test Automation Engineer"]
                - DevOps → ["DevOps Engineer", "Backend Developer"]

                Examples of skills that should be EMPTY (general):
                - Communication, Problem Solving, Teamwork, Leadership
                - English, Time Management, Critical Thinking
                - Agile, Scrum (methodology skills, not role-specific)

                LEVEL RULES - CRITICAL:
                - Level 1 (Junior) is the DEFAULT for most skills - use it liberally
                - Assign Level 1 to: programming languages, frameworks, tools, libraries, methodologies
                - Level 1 skills: Python, JavaScript, React, Docker, Git, SQL, REST API, Agile, etc
                - DO NOT be afraid to assign Level 1 - it's the most common level
                - Most skills should be Level 1 or Level 2
                - EXPECTED DISTRIBUTION: ~45-50% Level 1, ~35-40% Level 2, ~10-15% Level 3, ~5% Level 4
                - Level 2 (Middle): for skills requiring solid experience (frameworks with deep knowledge, complex tools)
                - Level 3 (Senior): ONLY for architecture, system design, mentoring, leadership technical decisions
                - Level 4 (Lead): ONLY for technical leadership roles, setting team standards

                Return JSON with skills array, each with: name, type, direction, level (integer), professions (array)
                Example: {jsonExample}
                """;
    }

    private string BuildClassificationPrompt(List<string> skillNames, string professionNames)
    {
        var skillsList = string.Join(", ", skillNames);
        return $"""
                Classify these skills:
                {skillsList}

                Available professions: {professionNames}

                Return JSON with type, direction, level, and professions for each skill.
                """;
    }

    private List<ParsedSkillDto> ParseClassificationResponse(string response, Dictionary<string, Guid> professionNameToId)
    {
        var skills = new List<ParsedSkillDto>();

        try
        {
            logger.LogDebug("Classification response (first 500 chars): {Response}", response[..Math.Min(500, response.Length)]);

            var jsonResponse = ExtractJsonFromResponse(response);
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                logger.LogWarning("Could not extract JSON from classification response");
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
                    var skill = ParseClassificationElement(skillElement, professionNameToId);
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
            logger.LogError(ex, "Error parsing classification response");
        }

        return skills;
    }

    private ParsedSkillDto? ParseClassificationElement(JsonElement skillElement, Dictionary<string, Guid> professionNameToId)
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
        }

        if (skillElement.TryGetProperty("professions", out var professionsElement))
        {
            logger.LogDebug("Professions element for {SkillName}: {ProfessionsJson}", skill.Name, professionsElement.ToString());

            if (professionsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var profElement in professionsElement.EnumerateArray())
                {
                    if (profElement.ValueKind == JsonValueKind.String)
                    {
                        var profName = profElement.GetString();
                        if (!string.IsNullOrWhiteSpace(profName))
                        {
                            if (professionNameToId.TryGetValue(profName, out var profId))
                            {
                                skill.ProfessionIds.Add(profId);
                            }
                            else
                            {
                                logger.LogWarning("Profession '{ProfessionName}' not found in database for skill '{SkillName}'", profName, skill.Name);
                            }
                        }
                    }
                }

                if (skill.ProfessionIds.Count > 0)
                {
                    logger.LogDebug("Skill {SkillName} matched to {ProfCount} professions from DB", skill.Name, skill.ProfessionIds.Count);
                }
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
