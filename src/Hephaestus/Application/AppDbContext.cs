using Hephaestus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hephaestus.Application;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<RawVacancy> RawVacancies { get; set; }
    public DbSet<KeySkill> KeySkills { get; set; }
    public DbSet<CleanSkill> CleanSkills { get; set; }
    public DbSet<SkillSynonym> SkillSynonyms { get; set; }
    public DbSet<SkillRelation> SkillRelations { get; set; }
    public DbSet<SkillOnReview> SkillsOnReview { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RawVacancy>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<KeySkill>().HasKey(r => r.Id);

        modelBuilder.Entity<RawVacancy>()
            .HasMany(r => r.KeySkills)
            .WithOne();

        modelBuilder.Entity<RawVacancy>().HasIndex(r => r.HeadHunterId).IsUnique();

        modelBuilder.Entity<CleanSkill>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<CleanSkill>()
            .HasIndex(c => c.NormalizedName)
            .IsUnique();

        modelBuilder.Entity<SkillSynonym>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<SkillSynonym>()
            .HasOne(s => s.CleanSkill)
            .WithMany(c => c.Synonyms)
            .HasForeignKey(s => s.CleanSkillId);

        modelBuilder.Entity<SkillRelation>()
            .HasKey(r => r.Id);
        modelBuilder.Entity<SkillRelation>()
            .HasOne(r => r.ParentSkill)
            .WithMany(c => c.ParentRelations)
            .HasForeignKey(r => r.ParentSkillId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SkillRelation>()
            .HasOne(r => r.ChildSkill)
            .WithMany(c => c.ChildRelations)
            .HasForeignKey(r => r.ChildSkillId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SkillOnReview>()
            .HasKey(r => r.Id);
    }
}