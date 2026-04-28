namespace Hephaestus.Features.OpenAiClients;

public class ChatCompletionRequest
{
    public string Model { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = new();
    public double? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public bool? Stream { get; set; }
}

public class ChatCompletionResponse
{
    public string Id { get; set; } = string.Empty;
    public string Object { get; set; } = string.Empty;
    public long Created { get; set; }
    public string Model { get; set; } = string.Empty;
    public List<Choice> Choices { get; set; } = new();
    public Usage? Usage { get; set; }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class Choice
{
    public int Index { get; set; }
    public ChatMessage Message { get; set; } = new();
    public string FinishReason { get; set; } = string.Empty;
}

public class Usage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

public class OpenAiClientException : Exception
{
    public int? StatusCode { get; set; }
    public string? ErrorCode { get; set; }

    public OpenAiClientException(string message) : base(message) { }

    public OpenAiClientException(string message, Exception innerException)
        : base(message, innerException) { }

    public OpenAiClientException(string message, int statusCode, string? errorCode = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
