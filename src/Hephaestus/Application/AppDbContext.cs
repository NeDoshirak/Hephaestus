using Hephaestus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Application;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<RawVacancy> RawVacancies { get; set; }
    public DbSet<KeySkill> KeySkills { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RawVacancy>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<KeySkill>().HasKey(r => r.Id);

        modelBuilder.Entity<RawVacancy>()
            .HasMany(r => r.KeySkills)
            .WithOne();
        
        modelBuilder.Entity<RawVacancy>().HasIndex(r => r.HeadHunterId).IsUnique();
    }
}