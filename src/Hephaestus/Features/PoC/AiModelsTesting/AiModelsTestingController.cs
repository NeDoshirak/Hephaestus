using Hephaestus.Features.GroqClient;
using Hephaestus.Features.OpenAiClients;
using Hephaestus.Features.OpenRouterClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Hephaestus.Features.PoC.AiModelsTesting;

[ApiController]
[Route("api/ai-models-test")]
public class AiModelsTestingController(
    IOpenRouterClient openRouterClient,
    IGroqClient groqClient,
    IOptions<OpenRouterOptions> openRouterOptions,
    IOptions<GroqOptions> groqOptions,
    ILogger<AiModelsTestingController> logger)
    : ControllerBase
{
    private readonly OpenRouterOptions _openRouterOptions = openRouterOptions.Value;
    private readonly GroqOptions _groqOptions = groqOptions.Value;

    [HttpPost("test")]
    public async Task<ActionResult<TestCompletionResponse>> TestCompletion(
        [FromBody] TestCompletionRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required");

            if (string.IsNullOrWhiteSpace(request.Model))
                return BadRequest("Model is required");

            var stopwatch = Stopwatch.StartNew();

            var messages = new List<ChatMessage>
            {
                new() { Role = "user", Content = request.Message }
            };

            ChatCompletionResponse response;

            switch (request.Provider?.ToLower())
            {
                case "openrouter":
                    if (string.IsNullOrEmpty(_openRouterOptions.ApiKey))
                        return BadRequest("OpenRouter API key not configured");

                    logger.LogInformation("Testing OpenRouter with model {Model}", request.Model);
                    response = await openRouterClient.CreateChatCompletionAsync(
                        request.Model,
                        messages,
                        request.MaxTokens,
                        request.Temperature);
                    break;

                case "groq":
                    if (string.IsNullOrEmpty(_groqOptions.ApiKey))
                        return BadRequest("Groq API key not configured");

                    logger.LogInformation("Testing Groq with model {Model}", request.Model);
                    response = await groqClient.CreateChatCompletionAsync(
                        request.Model,
                        messages,
                        request.MaxTokens,
                        request.Temperature);
                    break;

                default:
                    return BadRequest("Provider must be 'openrouter' or 'groq'");
            }

            stopwatch.Stop();

            return Ok(new TestCompletionResponse
            {
                Provider = request.Provider ?? "unknown",
                Model = request.Model,
                Response = response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty,
                PromptTokens = response.Usage?.PromptTokens,
                CompletionTokens = response.Usage?.CompletionTokens,
                TotalTokens = response.Usage?.TotalTokens,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            });
        }
        catch (OpenAiClientException ex)
        {
            logger.LogError(ex, "OpenAI client error");
            return BadRequest($"AI Provider error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in test completion");
            return StatusCode(500, $"Internal error: {ex.Message}");
        }
    }

    [HttpGet("health")]
    public ActionResult<HealthCheckResponse> HealthCheck()
    {
        var openRouterConfigured = !string.IsNullOrEmpty(_openRouterOptions.ApiKey);
        var groqConfigured = !string.IsNullOrEmpty(_groqOptions.ApiKey);

        return Ok(new HealthCheckResponse
        {
            OpenRouterConfigured = openRouterConfigured,
            GroqConfigured = groqConfigured,
            Message = openRouterConfigured || groqConfigured
                ? "At least one AI provider is configured"
                : "No AI providers configured"
        });
    }
}
