using Hephaestus.Domain.Enums;

namespace Hephaestus.Features.ProfessionsManagement.DTOs;

public class CreateProfessionRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ProfessionDirection Direction { get; set; }
}
