namespace Hephaestus.Domain.Entities;

public class RawVacancy
{
    public Guid Id { get; } = Guid.NewGuid();
    public int HeadHunterId { get; set; }
    public string VacancyName { get; set; } = string.Empty;
    public string VacancyDescription { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsProcessed { get; set; } = false;
    public ICollection<KeySkill> KeySkills { get; set; } = new List<KeySkill>();
}