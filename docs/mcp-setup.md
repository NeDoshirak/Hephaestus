# Подключение MCP сервера для ревью навыков

## Требования

- Docker и Docker Compose
- .NET 10 SDK (для миграций)
- Claude Code CLI

---

## 1. Запуск бэкенда

```bash
docker-compose up -d
```

Это поднимает PostgreSQL и само приложение. Бэк доступен на `http://localhost:7168` (HTTP) и `https://localhost:7169` (HTTPS) — см. `compose.yaml`.

### Применить миграцию (первый запуск или после обновления)

```bash
dotnet ef database update --project src/Hephaestus --startup-project src/Hephaestus
```

---

## 2. Проверить что MCP сервер работает

```bash
curl http://localhost:7168/mcp
```

Должен вернуть SSE-поток. Если видишь ответ — сервер готов.

---

## 3. Подключить MCP к Claude Code

### Вариант A — через CLI

```bash
claude mcp add hephaestus-skills --transport sse http://localhost:7168/mcp
```

### Вариант B — вручную в `.claude/settings.json` (локальный проект)

```json
{
  "mcpServers": {
    "hephaestus-skills": {
      "type": "sse",
      "url": "http://localhost:7168/mcp"
    }
  }
}
```

### Вариант C — глобально в `~/.claude/settings.json`

Тот же JSON, но в глобальном конфиге — MCP будет доступен во всех проектах.

---

## 4. Убедиться что инструменты загружены

Запусти новую сессию Claude Code и напиши:

```
/mcp
```

Должен появиться `hephaestus-skills` со списком инструментов.

---

## 5. Запуск ревью

После подключения можно передать агенту промт из файла [`agent-review-prompt.md`](./agent-review-prompt.md):

```bash
claude "$(cat docs/agent-review-prompt.md)"
```

Или вставить содержимое промта вручную в чат.

---

## Порт

Если бэк запущен не на `8080`, найди актуальный в `compose.yaml`:

```yaml
ports:
  - "ХОСТ_ПОРТ:8080"
```

И замени в URL выше.
