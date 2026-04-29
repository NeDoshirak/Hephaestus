using Hephaestus.Features.ProfessionsManagement.DTOs;
using Hephaestus.Features.ProfessionsManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Features.ProfessionsManagement;

[ApiController]
[Route("api/[controller]")]
public class ProfessionsController : ControllerBase
{
    private readonly IProfessionService _service;

    public ProfessionsController(IProfessionService service)
        => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ProfessionResponse>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProfessionResponse>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProfessionResponse>> Create([FromBody] CreateProfessionRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProfessionResponse>> Update(Guid id, [FromBody] UpdateProfessionRequest request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
