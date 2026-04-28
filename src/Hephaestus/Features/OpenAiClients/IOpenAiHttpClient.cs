namespace Hephaestus.Features.OpenAiClients;

public interface IOpenAiHttpClient
{
    void Configure(string baseUrl, string apiKey, string endpoint = "chat/completions");

    Task<ChatCompletionResponse> CreateChatCompletionAsync(
        ChatCompletionRequest request,
        Dictionary<string, string>? customHeaders = null);
}
