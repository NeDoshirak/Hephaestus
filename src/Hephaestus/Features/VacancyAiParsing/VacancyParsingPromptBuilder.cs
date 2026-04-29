using System.Text;
using Hephaestus.Domain.Entities;
using Hephaestus.Domain.Enums;

namespace Hephaestus.Features.VacancyAiParsing;

public class VacancyParsingPromptBuilder : IVacancyParsingPromptBuilder
{
    public string BuildSystemPrompt(IEnumerable<Profession> professions)
    {
        var professionList = professions.Take(20).Select(p => p.Name).ToList();
        var professionStr = string.Join(", ", professionList);

        return $$"""
                Classify skills from two sources: explicit KeySkills list + additional skills found in job description.
                
                For EACH skill (from both sources), assign:
                - Type: Language | Framework | Tool | Hard | Soft | DomainKnowledge
                - Level: integer from 1 to 7
                
                Level scale:
                1 — Beginner / Intern: basic familiarity, learning stage
                2 — Junior: can use with guidance
                3 — Junior+: some independent work
                4 — Middle: confident independent use
                5 — Middle+: deep practical experience
                6 — Senior: strong expertise, architecture decisions, mentoring
                7 — Lead / Expert: technical leadership, defines standards, leads teams
                
                IMPORTANT LEVEL RULES:
                - Infer level from job title, responsibilities, and wording in the description.
                - If a skill is only mentioned without strong senior signals, assign level 3–4.
                - Use level 6–7 ONLY if the description clearly indicates senior leadership, architecture ownership, or mentoring responsibility.
                - Avoid assigning the same high level to all skills.
                
                Other rules:
                - DO NOT rename skills — use the exact names as provided in KeySkills or found in the description.
                - Include both explicit KeySkills AND additional skills mentioned in the job description.
                - Each skill must appear only once in the output.
                
                Return JSON in the following format:
                
                {
                  "skills": [
                    {"name": "Python", "type": "Language", "level": 4},
                    {"name": "REST API", "type": "Hard", "level": 3}
                  ]
                }
                """;
    }

    public string BuildUserPrompt(IEnumerable<RawVacancy> vacancies)
    {
        var sb = new StringBuilder();

        foreach (var vacancy in vacancies)
        {
            sb.Append($"Job: {vacancy.VacancyName}. ");

            var description = vacancy.VacancyDescription;
            if (description.Length > 200)
                description = description[..200];
            sb.Append($"Description: {description}. ");

            if (vacancy.KeySkills.Count > 0)
            {
                sb.Append("Required skills: ");
                var skillNames = string.Join(", ", vacancy.KeySkills.Take(10).Select(ks => ks.Name));
                sb.Append(skillNames);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
