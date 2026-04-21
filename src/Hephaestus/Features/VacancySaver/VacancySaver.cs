using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Hephaestus.Features.HeadHunterClient;

namespace Hephaestus.Features.VacancySaver;

public class VacancySaver(IHeadHunterClient client, AppDbContext dbContext, ILogger<VacancySaver> logger) : IVacancySaver
{
    public async Task SaveVacancyToDatabaseAsync(string vacancyName)
    {
        int totalVacancyCount = (await client.SearchVacanciesAsync(vacancyName, 0, 10))!.Found;

        var pages = (totalVacancyCount + 9) / 10;
        int currentPage = 0;

        while (currentPage != pages)
        {
            var vacancyList = await client.SearchVacanciesAsync(vacancyName, currentPage, 10);

            await SaveVacancyBatchToDatabaseAsync(vacancyList!);

            currentPage++;
        }
    }

    private async Task SaveVacancyBatchToDatabaseAsync(HhVacanciesResponse vacanciesResponse)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            List<HhVacancy> vacancy = vacanciesResponse.Items;

            foreach (HhVacancy hv in vacancy)
            {
                var vacancyDetail = await client.GetVacancyAsync(hv.Id);

                SaveVacancyToDb(vacancyDetail!);
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Транзакция оборвалась: {EMessage}.", e.Message);
        }
    }

    private void SaveVacancyToDb(HhVacancyDetail vacancy)
    {
        RawVacancy rawVacancy = new RawVacancy
        {
            HeadHunterId = int.Parse(vacancy.Id),
            VacancyName = vacancy.Name,
            VacancyDescription = vacancy.Description!,
            Url = vacancy.Url ?? string.Empty,
            IsProcessed = false
        };

        if (dbContext.RawVacancies.Any(r => r.HeadHunterId == rawVacancy.HeadHunterId))
        {
            return;
        }

        var keySkills = ExtractKeySkill(vacancy);

        if (keySkills != null)
        {
            rawVacancy.KeySkills = keySkills;
        }

        dbContext.RawVacancies.Add(rawVacancy);
    }

    private List<KeySkill>? ExtractKeySkill(HhVacancyDetail vacancy)
    {
        if (vacancy.KeySkills != null && vacancy.KeySkills.Count != 0)
        {
            List<KeySkill> keySkills = new List<KeySkill>();

            foreach (var t in vacancy.KeySkills)
            {
                keySkills.Add(new KeySkill {Name = t.Name});
            }

            return keySkills;
        }

        return null;
    }
}