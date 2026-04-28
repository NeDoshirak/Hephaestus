using Hephaestus.Features.OpenAiClients;
using Microsoft.Extensions.Options;

namespace Hephaestus.Features.GroqClient;

public class GroqClient : IGroqClient
{
    private readonly IOpenAiHttpClient _httpClient;
    private readonly GroqOptions _options;
    private readonly ILogger<GroqClient> _logger;

    public GroqClient(
        IOpenAiHttpClient httpClient,
        IOptions<GroqOptions> options,
        ILogger<GroqClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.Configure(
            baseUrl: "https://api.groq.com/openai/v1",
            apiKey: _options.ApiKey
        );
    }

    public async Task<ChatCompletionResponse> CreateChatCompletionAsync(
        string model,
        List<ChatMessage> messages,
        int? maxTokens = null,
        double? temperature = null)
    {
        if (string.IsNullOrEmpty(_options.ApiKey))
        {
            throw new OpenAiClientException("Groq API key is not configured");
        }

        var request = new ChatCompletionRequest
        {
            Model = model,
            Messages = messages,
            MaxTokens = maxTokens,
            Temperature = temperature
        };

        _logger.LogInformation("Creating chat completion via Groq. Model: {Model}, Messages: {Count}",
            model, messages.Count);

        return await _httpClient.CreateChatCompletionAsync(request);
    }
}
