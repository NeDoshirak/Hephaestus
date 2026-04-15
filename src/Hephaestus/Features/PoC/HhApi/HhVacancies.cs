using Hephaestus.Features.HeadHunterClient;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Features.PoC.HhApi;

[ApiController]
[Route("[controller]")]
public class HhVacancies(IHeadHunterClient client) : ControllerBase
{
    [HttpGet("vacancies")]
    public async Task<IActionResult> SearchVacancies([FromQuery] string search, [FromQuery] int page = 0,
        [FromQuery(Name = "per_page")] int perPage = 10)
    {
        HhVacanciesResponse? response = await client.SearchVacanciesAsync(search,  page, perPage);

        if (response == null)
        {
            return Ok("No results found");
        }
        
        return Ok(response);
    }

    [HttpGet("vacancies/{vacancyId}")]
    public async Task<IActionResult> GetVacancy(string vacancyId)
    {
        HhVacancyDetail? detail = await client.GetVacancyAsync(vacancyId);
        
        if (detail == null)
        {
            return NotFound();
        }
        return Ok(detail);
    }
}