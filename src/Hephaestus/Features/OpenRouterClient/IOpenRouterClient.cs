using Hephaestus.Features.OpenAiClients;

namespace Hephaestus.Features.OpenRouterClient;

public interface IOpenRouterClient
{
    Task<ChatCompletionResponse> CreateChatCompletionAsync(
        string model,
        List<ChatMessage> messages,
        int? maxTokens = null,
        double? temperature = null);
}
