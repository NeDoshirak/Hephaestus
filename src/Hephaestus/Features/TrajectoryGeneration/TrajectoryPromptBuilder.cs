using Hephaestus.Domain.Entities;

namespace Hephaestus.Features.TrajectoryGeneration;

public class TrajectoryPromptBuilder
{
    public string BuildSystemPrompt() =>
        """
        Ты эксперт по построению образовательных траекторий для IT-специалистов.

        Твоя задача — получить список навыков профессии и построить из них ГРАФ обучения,
        разбитый на несколько независимых ТРЕКОВ.

        ## Что такое трек
        Трек — это смысловая группа навыков, которую можно изучать параллельно с другими треками.
        Примеры треков для разработчика: "Язык и основы", "Архитектура и паттерны",
        "Тестирование", "Инфраструктура", "Soft Skills", "Безопасность и домен".
        Треки определяй сам исходя из конкретного набора навыков — не используй шаблонный список.

        ## Что такое граф внутри трека
        Граф — это упорядоченный набор навыков (nodes) и направленных зависимостей (edges).
        - nodes: каждый навык имеет порядковый номер (order), уровень (Junior/Middle/Senior/Lead),
          признак обязательности (isCore) и краткое обоснование
        - edges: From → To означает "сначала изучи From, потом To"
          Тип Prerequisite — строгая зависимость, Recommended — желательная последовательность

        ## Правила
        - Каждый навык входит РОВНО в один трек
        - Рёбра (edges) возможны ТОЛЬКО внутри одного трека
        - Порядок (order) начинается с 1, соответствует последовательности изучения
        - Level — уровень, начиная с которого навык актуален: Junior | Middle | Senior | Lead
        - isCore=true — без этого навыка нельзя перейти к следующим в цепочке
        - Используй ВСЕ переданные навыки, не выбрасывай ни одного

        Верни СТРОГО JSON без markdown-обёрток, по схеме:
        {
          "tracks": [
            {
              "name": "Язык и основы",
              "description": "Фундамент: синтаксис языка, типы данных, базовые конструкции",
              "nodes": [
                {
                  "displayName": "C#",
                  "level": "Junior",
                  "order": 1,
                  "isCore": true,
                  "reason": "Основной язык — точка входа в профессию"
                }
              ],
              "edges": [
                { "from": "C#", "to": "LINQ", "type": "Prerequisite" }
              ]
            }
          ],
          "aiComment": "Общий комментарий к траектории"
        }
        """;

    public string BuildUserPrompt(Profession profession, IEnumerable<CleanSkill> skills, bool includeRelations)
    {
        var skillLines = skills.Select(s =>
        {
            var deps = includeRelations
                ? s.ChildRelations.Where(r => r.ParentSkill != null).Select(r => r.ParentSkill!.DisplayName).ToList()
                : new List<string>();

            var depsStr = deps.Count > 0 ? $"зависит от: {string.Join(", ", deps)}" : "нет зависимостей";
            return $"- {s.DisplayName} | {s.SkillType?.ToString() ?? "?"} | {s.Direction?.ToString() ?? "?"} | {s.Level?.ToString() ?? "?"} | {depsStr}";
        });

        return $"""
            Профессия: {profession.Name} (направление: {profession.Direction})

            Навыки профессии (DisplayName | SkillType | Direction | Level | зависимости):
            {string.Join("\n", skillLines)}

            Построй граф траектории обучения по трекам. Используй ВСЕ {skills.Count()} навыков.
            """;
    }
}
