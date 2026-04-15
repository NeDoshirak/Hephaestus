namespace Hephaestus.Features.VacancySaver;

public interface IVacancySaver
{
    public Task SaveVacancyToDatabaseAsync(string vacancyName, int? vacancyCount = null);
}