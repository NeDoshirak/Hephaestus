namespace Hephaestus.Features.PoC.AiModelsTesting;

public class TestCompletionRequest
{
    public string Provider { get; set; } = "openrouter"; // "openrouter" or "groq"
    public string Model { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? MaxTokens { get; set; } = 500;
    public double? Temperature { get; set; } = 0.7;
}

public class TestCompletionResponse
{
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public int? PromptTokens { get; set; }
    public int? CompletionTokens { get; set; }
    public int? TotalTokens { get; set; }
    public long ElapsedMilliseconds { get; set; }
}

public class ModelsResponse
{
    public string Provider { get; set; } = string.Empty;
    public List<string> AvailableModels { get; set; } = new();
}

public class HealthCheckResponse
{
    public bool OpenRouterConfigured { get; set; }
    public bool GroqConfigured { get; set; }
    public string Message { get; set; } = string.Empty;
}
