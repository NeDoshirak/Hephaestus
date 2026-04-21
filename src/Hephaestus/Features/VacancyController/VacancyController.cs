using Hephaestus.Features.VacancySaver;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Features.VacancyController;

[ApiController]
[Route("[controller]")]
public class VacancyController(IVacancySaver vacancySaver) : ControllerBase
{
    [HttpGet("vacancies/save")]
    public async Task<IActionResult> SaveVacancies([FromQuery] string search)
    {
        await vacancySaver.SaveVacancyToDatabaseAsync(search);
        
        return Ok();
    }
}