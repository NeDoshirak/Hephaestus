using Hephaestus.Features.HeadHunterClient;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Features.HhApi;

[ApiController]
[Route("api/hh")]
public class HhApiController(IHeadHunterClient client) : ControllerBase
{
    [HttpGet("vacancies")]
    public async Task<IActionResult> SearchVacancies(
        [FromQuery] string text,
        [FromQuery] int page = 0,
        [FromQuery] int perPage = 10)
    {
        var result = await client.SearchVacanciesAsync(text, page, perPage);
        if (result == null)
            return StatusCode(500, new { error = "Ошибка при поиске вакансий на HH" });

        return Ok(result);
    }

    [HttpGet("vacancies/{vacancyId}")]
    public async Task<IActionResult> GetVacancy(string vacancyId)
    {
        var result = await client.GetVacancyAsync(vacancyId);
        if (result == null)
            return StatusCode(500, new { error = "Ошибка при получении деталей вакансии" });

        return Ok(result);
    }
}
