using Microsoft.Extensions.Options;

namespace Hephaestus.Features.HeadHunterClient;

public class HeadHunterClient(
    IOptions<HeadHunterClientSettings> options,
    HttpClient httpClient,
    ILogger<HeadHunterClient> logger) : IHeadHunterClient
{
    public async Task<string?> SearchVacanciesAsync(string text, CancellationToken ct = default)
    {
        try
        {
            var requestUrl = $"vacancies?text={Uri.EscapeDataString(text)}&per_page=10";
            
            logger.LogInformation("Поиск вакансий: {Text}", text);
            var response = await httpClient.GetAsync(options.Value.BaseUrl + requestUrl, ct);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStringAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка поиска вакансий: {Text}", text);
            return null;
        }
    }
}