using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Features.TrajectoryGeneration;

[ApiController]
[Route("api/[controller]")]
public class TrajectoryController(
    ITrajectoryGenerationService trajectoryService,
    ILogger<TrajectoryController> logger) : ControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromBody] TrajectoryGenerationRequest request,
        CancellationToken ct)
    {
        if (request.ProfessionId == Guid.Empty)
            return BadRequest("ProfessionId is required");

        try
        {
            var result = await trajectoryService.GenerateAsync(request, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Cannot generate trajectory for profession {ProfessionId}", request.ProfessionId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating trajectory for profession {ProfessionId}", request.ProfessionId);
            return StatusCode(500, new { error = "Failed to generate trajectory" });
        }
    }
}
