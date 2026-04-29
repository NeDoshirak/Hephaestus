namespace Hephaestus.Features.SkillManagement;

public class ImportStatusService
{
    private int _totalVacancies;
    private int _processedVacancies;
    private int _totalSkills;
    private bool _isImporting;
    private readonly object _lock = new();

    public void StartImport(int totalVacancies)
    {
        lock (_lock)
        {
            _totalVacancies = totalVacancies;
            _processedVacancies = 0;
            _totalSkills = 0;
            _isImporting = true;
        }
    }

    public void UpdateProgress(int processedVacancies, int totalSkills)
    {
        lock (_lock)
        {
            _processedVacancies = processedVacancies;
            _totalSkills = totalSkills;
        }
    }

    public void CompleteImport()
    {
        lock (_lock)
        {
            _isImporting = false;
        }
    }

    public ImportStatus GetStatus()
    {
        lock (_lock)
        {
            var progress = _totalVacancies > 0 ? (_processedVacancies * 100) / _totalVacancies : 0;
            return new ImportStatus
            {
                IsImporting = _isImporting,
                ProcessedVacancies = _processedVacancies,
                TotalVacancies = _totalVacancies,
                TotalSkills = _totalSkills,
                ProgressPercent = progress
            };
        }
    }
}

public class ImportStatus
{
    public bool IsImporting { get; set; }
    public int ProcessedVacancies { get; set; }
    public int TotalVacancies { get; set; }
    public int TotalSkills { get; set; }
    public int ProgressPercent { get; set; }
}
