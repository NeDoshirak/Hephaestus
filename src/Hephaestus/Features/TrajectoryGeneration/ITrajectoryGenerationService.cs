namespace Hephaestus.Features.TrajectoryGeneration;

public interface ITrajectoryGenerationService
{
    Task<TrajectoryResponse> GenerateAsync(TrajectoryGenerationRequest request, CancellationToken ct = default);
}
