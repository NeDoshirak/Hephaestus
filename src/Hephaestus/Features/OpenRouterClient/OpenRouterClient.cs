using Hephaestus.Features.OpenAiClients;
using Microsoft.Extensions.Options;

namespace Hephaestus.Features.OpenRouterClient;

public class OpenRouterClient : IOpenRouterClient
{
    private readonly IOpenAiHttpClient _httpClient;
    private readonly OpenRouterOptions _options;
    private readonly ILogger<OpenRouterClient> _logger;

    public OpenRouterClient(
        IOpenAiHttpClient httpClient,
        IOptions<OpenRouterOptions> options,
        ILogger<OpenRouterClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.Configure(
            baseUrl: "https://openrouter.ai/api/v1",
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
            throw new OpenAiClientException("OpenRouter API key is not configured");
        }

        var request = new ChatCompletionRequest
        {
            Model = model,
            Messages = messages,
            MaxTokens = maxTokens,
            Temperature = temperature
        };

        var headers = new Dictionary<string, string>
        {
            ["HTTP-Referer"] = _options.AppUrl,
            ["X-OpenRouter-Title"] = _options.AppTitle
        };

        _logger.LogInformation("Creating chat completion via OpenRouter. Model: {Model}, Messages: {Count}",
            model, messages.Count);

        return await _httpClient.CreateChatCompletionAsync(request, headers);
    }
}
