using System.Text;
using System.Text.RegularExpressions;
using Unidecode.NET;

namespace Hephaestus.Features.SkillManagement;

public class SkillNormalizationService : ISkillNormalizationService
{
    private static readonly Dictionary<char, string> SpecialCharMappings = new()
    {
        { '#', "sharp" },
        { '+', "plus" },
    };

    public string Normalize(string skillName)
    {
        if (string.IsNullOrWhiteSpace(skillName))
            return string.Empty;

        var input = skillName.Trim();

        input = RemoveVersions(input);

        input = input.Unidecode();

        var sb = new StringBuilder();
        foreach (var ch in input)
        {
            if (SpecialCharMappings.TryGetValue(ch, out var replacement))
            {
                sb.Append(replacement);
            }
            else
            {
                sb.Append(ch);
            }
        }
        input = sb.ToString();

        input = Regex.Replace(input, @"[^a-zA-Z0-9]", "");

        var normalized = input.ToLowerInvariant();

        return normalized.Trim();
    }

    private static string RemoveVersions(string input)
    {
        input = Regex.Replace(input, @"\s+[\d./+\-]+\s*$", "");

        input = Regex.Replace(input, @"\s+[\d./+\-]+\s+", " ");

        return input.Trim();
    }
}
