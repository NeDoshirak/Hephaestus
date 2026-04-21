namespace Hephaestus.Domain.Entities;

public class KeySkill
{
    public Guid Id { get; } =  Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}