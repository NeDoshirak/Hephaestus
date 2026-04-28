using System.Text;
using System.Text.Json;
using Hephaestus.Features.OpenAiClients;

namespace Hephaestus.Features.OpenAiClients;

public class OpenAiHttpClient : IOpenAiHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAiHttpClient> _logger;
    private string _baseUrl = string.Empty;
    private string _apiKey = string.Empty;
    private string _endpoint = "chat/completions";

    public OpenAiHttpClient(HttpClient httpClient, ILogger<OpenAiHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public void Configure(string baseUrl, string apiKey, string endpoint = "chat/completions")
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _apiKey = apiKey;
        _endpoint = endpoint;
    }

    public async Task<ChatCompletionResponse> CreateChatCompletionAsync(
        ChatCompletionRequest request,
        Dictionary<string, string>? customHeaders = null)
    {
        if (string.IsNullOrEmpty(_baseUrl) || string.IsNullOrEmpty(_apiKey))
        {
            throw new OpenAiClientException("OpenAI client is not configured. Call Configure() first.");
        }

        var url = $"{_baseUrl}/{_endpoint}";
        var requestBody = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };

        httpRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        if (customHeaders != null)
        {
            foreach (var header in customHeaders)
            {
                httpRequest.Headers.Add(header.Key, header.Value);
            }
        }

        try
        {
            _logger.LogInformation("Requesting chat completion from {Url}", url);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("OpenAI API error {StatusCode}: {Content}", response.StatusCode, errorContent);

                throw new OpenAiClientException(
                    $"OpenAI API returned status {response.StatusCode}: {errorContent}",
                    (int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ChatCompletionResponse>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            if (result == null)
            {
                throw new OpenAiClientException("Failed to deserialize response from OpenAI API");
            }

            _logger.LogInformation("Successfully received chat completion response. Usage: {Usage}", result.Usage?.TotalTokens);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed");
            throw new OpenAiClientException("Failed to communicate with OpenAI API", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization failed");
            throw new OpenAiClientException("Invalid JSON response from OpenAI API", ex);
        }
    }
}
