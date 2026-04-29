using Hephaestus.Features.ProfessionsManagement.DTOs;

namespace Hephaestus.Features.ProfessionsManagement.Interfaces;

public interface IProfessionService
{
    Task<List<ProfessionResponse>> GetAllAsync();
    Task<ProfessionResponse?> GetByIdAsync(Guid id);
    Task<ProfessionResponse> CreateAsync(CreateProfessionRequest request);
    Task<ProfessionResponse> UpdateAsync(Guid id, UpdateProfessionRequest request);
    Task DeleteAsync(Guid id);
}
