using System.Text.Json;
using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Hephaestus.Features.OpenAiClients;
using Hephaestus.Features.OpenRouterClient;

namespace Hephaestus.Features.VacancyAiParsing;

public class SkillExtractionService(
    IOpenRouterClient openRouterClient,
    ILogger<SkillExtractionService> logger) : ISkillExtractionService
{
    private const string Model = "openai/gpt-oss-120b:free";
    private const int DelayBetweenRequests = 500;

    public async Task<Dictionary<string, int>> ExtractSkillsAsync(
        IEnumerable<RawVacancy> vacancies,
        CancellationToken ct = default)
    {
        var allSkills = new Dictionary<string, int>();
        var vacancyList = vacancies.ToList();

        if (!vacancyList.Any())
            return allSkills;

        var batches = vacancyList.Chunk(1).ToList();

        foreach (var batch in batches)
        {
            try
            {
                logger.LogInformation("Extracting skills from {Count} vacancies", batch.Length);

                var userPrompt = BuildExtractionPrompt(batch);
                var messages = new List<ChatMessage>
                {
                    new() { Role = "system", Content = GetExtractionSystemPrompt() },
                    new() { Role = "user", Content = userPrompt }
                };

                var response = await openRouterClient.CreateChatCompletionAsync(Model, messages, maxTokens: 1024, temperature: 0.1);

                await Task.Delay(DelayBetweenRequests);

                var responseText = response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
                var skills = ParseExtractionResponse(responseText);

                foreach (var skill in skills)
                {
                    if (allSkills.ContainsKey(skill))
                        allSkills[skill]++;
                    else
                        allSkills[skill] = 1;
                }

                logger.LogInformation("Extracted {SkillCount} skills from batch", skills.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error extracting skills from batch: {Message}", ex.Message);
            }
        }

        var uniqueCount = allSkills.Count;
        var totalCount = allSkills.Values.Sum();
        logger.LogInformation("Total: {UniqueSkills} unique skills, {TotalCount} occurrences", uniqueCount, totalCount);
        return allSkills;
    }

    private string GetExtractionSystemPrompt()
    {
        return """
               You are a skill extraction expert for job vacancies.
               Extract all skills from the job posting.

               Sources:
               1. Explicit KeySkills list
               2. Skills mentioned in job description

               Rules:
               - Use EXACT names as provided in KeySkills or found in description
               - Do NOT rename or modify skill names
               - Include each skill only once
               - Return skills that are relevant and meaningful
               - SIMPLIFY complex skill names to basic single concepts
                 Examples: "Security best practices for crypto assets" → "Security"
                          "MIPS architecture" → "MIPS" or "Architecture"
                          "REST API design patterns" → "REST API"
               - Prefer short, simple skill names (1-3 words maximum)
               - If a skill name is too complex/long, extract the core concept

               Return JSON with exact format:
               {
                 "skills": ["Python", "React", "Docker"]
               }
               """;
    }

    private string BuildExtractionPrompt(RawVacancy[] vacancies)
    {
        if (vacancies.Length == 0)
            return string.Empty;

        var vacancy = vacancies[0];
        var prompt = $"""
                      Job Title: {vacancy.VacancyName}

                      Description: {vacancy.VacancyDescription[..Math.Min(500, vacancy.VacancyDescription.Length)]}

                      """;

        if (vacancy.KeySkills.Count > 0)
        {
            var skillNames = string.Join(", ", vacancy.KeySkills.Select(ks => ks.Name));
            prompt += $"Explicit KeySkills: {skillNames}";
        }

        return prompt;
    }

    private List<string> ParseExtractionResponse(string response)
    {
        var skills = new List<string>();

        try
        {
            logger.LogDebug("Extraction response (first 300 chars): {Response}", response[..Math.Min(300, response.Length)]);

            var jsonResponse = ExtractJsonFromResponse(response);
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                logger.LogWarning("Could not extract JSON from extraction response");
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
                if (skillElement.ValueKind == JsonValueKind.String)
                {
                    var skillName = skillElement.GetString();
                    if (!string.IsNullOrWhiteSpace(skillName))
                    {
                        skills.Add(skillName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing extraction response");
        }

        return skills;
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
