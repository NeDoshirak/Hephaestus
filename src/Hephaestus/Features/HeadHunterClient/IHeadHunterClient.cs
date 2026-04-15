namespace Hephaestus.Features.HeadHunterClient;

public interface IHeadHunterClient
{
    Task<HhVacanciesResponse?> SearchVacanciesAsync(string text, int page = 0, int perPage = 10, CancellationToken ct = default);
    
    Task<HhVacancyDetail?> GetVacancyAsync(string vacancyId, CancellationToken ct = default);
}