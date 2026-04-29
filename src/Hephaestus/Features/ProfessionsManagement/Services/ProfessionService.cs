using Hephaestus.Application;
using Hephaestus.Domain.Entities;
using Hephaestus.Features.ProfessionsManagement.DTOs;
using Hephaestus.Features.ProfessionsManagement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Features.ProfessionsManagement.Services;

public class ProfessionService : IProfessionService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProfessionService> _logger;

    public ProfessionService(AppDbContext db, ILogger<ProfessionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<ProfessionResponse>> GetAllAsync()
    {
        return await _db.Professions
            .Select(x => new ProfessionResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Direction = x.Direction,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ProfessionResponse?> GetByIdAsync(Guid id)
    {
        var profession = await _db.Professions.FindAsync(id);
        if (profession == null) return null;

        return new ProfessionResponse
        {
            Id = profession.Id,
            Name = profession.Name,
            Description = profession.Description,
            Direction = profession.Direction,
            CreatedAt = profession.CreatedAt,
            UpdatedAt = profession.UpdatedAt
        };
    }

    public async Task<ProfessionResponse> CreateAsync(CreateProfessionRequest request)
    {
        var profession = new Profession
        {
            Name = request.Name,
            Description = request.Description,
            Direction = request.Direction
        };

        _db.Professions.Add(profession);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Created profession: {ProfessionId}", profession.Id);

        return new ProfessionResponse
        {
            Id = profession.Id,
            Name = profession.Name,
            Description = profession.Description,
            Direction = profession.Direction,
            CreatedAt = profession.CreatedAt,
            UpdatedAt = profession.UpdatedAt
        };
    }

    public async Task<ProfessionResponse> UpdateAsync(Guid id, UpdateProfessionRequest request)
    {
        var profession = await _db.Professions.FindAsync(id);
        if (profession == null)
            throw new KeyNotFoundException($"Profession {id} not found");

        profession.Name = request.Name;
        profession.Description = request.Description;
        profession.Direction = request.Direction;
        profession.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Updated profession: {ProfessionId}", id);

        return new ProfessionResponse
        {
            Id = profession.Id,
            Name = profession.Name,
            Description = profession.Description,
            Direction = profession.Direction,
            CreatedAt = profession.CreatedAt,
            UpdatedAt = profession.UpdatedAt
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var profession = await _db.Professions.FindAsync(id);
        if (profession == null)
            throw new KeyNotFoundException($"Profession {id} not found");

        _db.Professions.Remove(profession);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Deleted profession: {ProfessionId}", id);
    }
}
