using Hephaestus.Features.OpenAiClients;

namespace Hephaestus.Features.GroqClient;

public interface IGroqClient
{
    Task<ChatCompletionResponse> CreateChatCompletionAsync(
        string model,
        List<ChatMessage> messages,
        int? maxTokens = null,
        double? temperature = null);
}
