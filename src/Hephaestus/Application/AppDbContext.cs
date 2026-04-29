using Hephaestus.Domain.Entities;
using Hephaestus.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hephaestus.Application;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<RawVacancy> RawVacancies { get; set; }
    public DbSet<KeySkill> KeySkills { get; set; }
    public DbSet<CleanSkill> CleanSkills { get; set; }
    public DbSet<SkillSynonym> SkillSynonyms { get; set; }
    public DbSet<SkillRelation> SkillRelations { get; set; }
    public DbSet<SkillOnReview> SkillsOnReview { get; set; }
    public DbSet<Profession> Professions { get; set; }

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

        // Professions
        modelBuilder.Entity<Profession>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Profession>()
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<Profession>()
            .HasMany(x => x.CleanSkills)
            .WithOne(x => x.Profession)
            .HasForeignKey(x => x.ProfessionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Profession>()
            .HasMany(x => x.SkillsOnReview)
            .WithOne(x => x.Profession)
            .HasForeignKey(x => x.ProfessionId)
            .OnDelete(DeleteBehavior.SetNull);

        // CleanSkill enum conversions
        modelBuilder.Entity<CleanSkill>()
            .Property(x => x.Direction)
            .HasConversion(new EnumToStringConverter<Direction>());

        modelBuilder.Entity<CleanSkill>()
            .Property(x => x.SkillType)
            .HasConversion(new EnumToStringConverter<SkillType>());

        // SkillOnReview enum conversions
        modelBuilder.Entity<SkillOnReview>()
            .Property(x => x.Direction)
            .HasConversion(new EnumToStringConverter<Direction>());

        modelBuilder.Entity<SkillOnReview>()
            .Property(x => x.SkillType)
            .HasConversion(new EnumToStringConverter<SkillType>());
    }
}