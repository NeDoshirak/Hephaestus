namespace Hephaestus.Features.TrajectoryGeneration;

public class TrajectoryGenerationRequest
{
    public Guid ProfessionId { get; set; }
    public bool IncludeRelations { get; set; } = true;
}

public class TrajectoryResponse
{
    public Guid ProfessionId { get; set; }
    public string ProfessionName { get; set; } = string.Empty;
    /// <summary>Независимые треки, которые можно изучать параллельно</summary>
    public List<TrajectoryTrack> Tracks { get; set; } = new();
    public string? AiComment { get; set; }
}

/// <summary>Смысловой трек обучения — направленный граф навыков</summary>
public class TrajectoryTrack
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>Узлы графа — навыки в порядке изучения</summary>
    public List<TrajectoryNode> Nodes { get; set; } = new();
    /// <summary>Рёбра графа — направленные зависимости между навыками</summary>
    public List<TrajectoryEdge> Edges { get; set; } = new();
}

public class TrajectoryNode
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? SkillType { get; set; }
    public string? Direction { get; set; }
    /// <summary>Уровень, на котором навык становится актуальным</summary>
    public string Level { get; set; } = string.Empty;
    /// <summary>Порядковый номер в треке (для линейного рендера)</summary>
    public int Order { get; set; }
    /// <summary>Обязателен (true) или желателен (false)</summary>
    public bool IsCore { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class TrajectoryEdge
{
    /// <summary>DisplayName навыка-предпосылки</summary>
    public string From { get; set; } = string.Empty;
    /// <summary>DisplayName зависимого навыка</summary>
    public string To { get; set; } = string.Empty;
    /// <summary>Тип связи: Prerequisite | Recommended</summary>
    public string Type { get; set; } = string.Empty;
}
