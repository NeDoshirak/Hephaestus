namespace Hephaestus.Features.HeadHunterClient;

public class HeadHunterClientSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    
    public string TokenEndpoint { get; set; } = string.Empty;
    
    public string ClientName { get; set; } = string.Empty;
    
    public string ClientId { get; set; } = string.Empty;
    
    public string ClientSecret { get; set; } = string.Empty;
    
    public string ClientToken { get; set; } = string.Empty;
    
    public string GrantType { get; set; } = string.Empty;
    
    public int TimeoutSeconds { get; set; }
}