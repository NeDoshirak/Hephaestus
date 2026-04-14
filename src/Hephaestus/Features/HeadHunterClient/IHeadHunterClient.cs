namespace Hephaestus.Features.HeadHunterClient;

public interface IHeadHunterClient
{
    Task<string?> SearchVacanciesAsync(string text, CancellationToken ct = default);
}