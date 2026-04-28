using System.Text.RegularExpressions;

namespace Hephaestus.Features.TextCleaning;

public class TextCleaningService : ITextCleaningService
{
    public string RemoveHtmlTags(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return Regex.Replace(text, @"<[^>]*>", string.Empty);
    }
}
