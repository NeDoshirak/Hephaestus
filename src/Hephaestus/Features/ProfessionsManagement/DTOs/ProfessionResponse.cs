using Hephaestus.Domain.Enums;

namespace Hephaestus.Features.ProfessionsManagement.DTOs;

public class ProfessionResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ProfessionDirection Direction { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
