namespace Hephaestus.Features.OpenRouterClient;

public class OpenRouterOptions
{
    public const string SectionName = "OpenRouter";

    public string ApiKey { get; set; } = string.Empty;
    public string AppUrl { get; set; } = "https://hephaestus.local";
    public string AppTitle { get; set; } = "Hephaestus";
}
