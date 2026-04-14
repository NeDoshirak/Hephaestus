using Hephaestus.Features.HeadHunterClient;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Features.PoC.HhApi;

[ApiController]
[Route("[controller]")]
public class HhVacancies(IHeadHunterClient client) : ControllerBase
{
    [HttpGet("vacancies")]
    public async Task<IActionResult> SearchVacancies([FromQuery] string query)
    {
        string? response = await client.SearchVacanciesAsync(query);

        if (response == null)
        {
            return Ok("No results found");
        }
        
        return Ok(response);
    }
}