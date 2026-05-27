# Полный гайд по функциям Hephaestus

## 📋 Вакансии (Vacancies)

### Что это
Управление всеми вакансиями в системе. Здесь вы можете просмотреть, отредактировать, добавить или удалить вакансии.

### Основные операции

#### Получить список вакансий
```
GET /api/vacancy
Параметры:
- page: номер страницы (по умолчанию 1)
- pageSize: сколько вакансий на странице (по умолчанию 10)
- search: поиск по названию или описанию
- skillFilter: фильтр по ключевому навыку

Пример: GET /api/vacancy?page=1&pageSize=20&search=React
```

#### Получить одну вакансию
```
GET /api/vacancy/{id}
```

#### Загрузить вакансии с HeadHunter
```
GET /api/vacancy/save?search=JavaScript
```
**Что происходит:**
1. Система подключается к HeadHunter API
2. Ищет вакансии по вашему запросу
3. Скачивает описания и требования
4. Автоматически запускает AI анализ для извлечения навыков
5. Сохраняет всё в базу
6. Возвращает количество загруженных вакансий

#### Создать вакансию вручную
```
POST /api/vacancy
Body:
{
  "vacancyName": "React Developer",
  "vacancyDescription": "We need a React developer...",
  "url": "https://hh.ru/vacancy/123"
}
```

#### Редактировать вакансию
```
PUT /api/vacancy/{id}
Body:
{
  "vacancyName": "Senior React Developer",
  "vacancyDescription": "Updated description",
  "url": "https://...",
  "isProcessed": true
}
```

#### Удалить вакансию
```
DELETE /api/vacancy/{id}
```

#### Получить все уникальные навыки из вакансий
```
GET /api/vacancy/skills
Возвращает: ["JavaScript", "React", "Node.js", ...]
```

---

## 🤖 Управление навыками (Skills)

### Процесс работы с навыками

```
Вакансия загружена
       ↓
Инициировать: POST /skills/import-from-vacancies
       ↓
AI извлекает навыки
       ↓
Смотреть: GET /skills/on-review
       ↓
Одобрить или отклонить каждый навык
       ↓
Навык в справочнике: GET /skills/clean
```

### Операции

#### Импортировать навыки из непроцессированных вакансий
```
POST /api/skills/import-from-vacancies
Параметры (опциональные):
- vacancyNameFilter: фильтр по названию вакансии
- limit: максимальное количество вакансий для обработки

Пример: POST /api/skills/import-from-vacancies?limit=50
```

**Что происходит:**
1. Система находит вакансии, которые ещё не обработаны
2. Запускает AI (используя OpenRouter, Groq или OpenAI)
3. AI читает описание и подчёркивает все навыки
4. Создаёт записи "требует проверки"
5. Возвращает результаты

#### Получить навыки в ожидании проверки
```
GET /api/skills/on-review
Возвращает список с информацией:
- OriginalName: как записана в вакансии
- NormalizedName: нормализованная версия
- Counter: сколько раз встречается
- SuggestedDisplayName: предложенное отображаемое имя
- Status: "pending" (ожидает проверки)
```

#### Одобрить навык
```
POST /api/skills/on-review/{id}/approve
Body:
{
  "displayName": "React",
  "skillType": "Framework",
  "direction": "Frontend",
  "level": "Middle",
  "professionId": "guid-here"
}
```

**Что происходит:**
1. Навык перемещается из "требует проверки" в справочник
2. Если уже существует синоним — связывается с ним
3. Добавляются все метаданные (тип, направление, уровень)
4. Обновляется счётчик встречаемости
5. Становится доступен в справочнике

#### Отклонить навык
```
POST /api/skills/on-review/{id}/reject
```

#### Получить справочник всех навыков
```
GET /api/skills/clean
Возвращает полный справочник с:
- displayName: отображаемое имя
- normalizedName: нормализованное имя
- description: описание навыка
- counter: в скольких вакансиях встречается
- skillType: тип (язык, фреймворк, etc)
- direction: направление (Frontend, Backend, etc)
- level: требуемый уровень
- synonyms: синонимы этого навыка
- parentSkills: надставляющие навыки (если React → то JavaScript)
- dependentSkills: зависимые навыки
```

#### Добавить новый навык вручную
```
POST /api/skills/clean/add
Body:
{
  "displayName": "Kubernetes",
  "normalizedName": "kubernetes",
  "description": "Container orchestration platform",
  "skillType": "DevOps",
  "direction": "DevOps",
  "level": "Middle",
  "professionId": "guid-here"
}
```

#### Сгенерировать описание для навыка
```
GET /api/skills/generate-description?skillName=React
```

**Что происходит:**
1. Система отправляет запрос к AI (OpenRouter GPT-4)
2. AI генерирует краткое описание на русском языке
3. Возвращает описание в 1-2 предложения

**Пример ответа:**
```json
{
  "description": "React — это JavaScript библиотека для создания пользовательских интерфейсов с помощью компонентов. Позволяет эффективно управлять состоянием приложения и повторно использовать компоненты."
}
```

---

## 👔 Управление профессиями (Professions)

### Что это
Профессии — это категории работ (Frontend Developer, Backend Developer, DevOps Engineer, etc). Каждой профессии можно назначить навыки и траектории развития.

### Операции

#### Получить все профессии
```
GET /api/professions
Возвращает список всех профессий с описаниями
```

#### Получить одну профессию
```
GET /api/professions/{id}
```

#### Создать профессию
```
POST /api/professions
Body:
{
  "name": "Frontend Developer",
  "description": "Разработчик пользовательских интерфейсов",
  "direction": "Frontend"
}
```

#### Редактировать профессию
```
PUT /api/professions/{id}
Body:
{
  "name": "Senior Frontend Developer",
  "description": "Опытный разработчик с ответственностью за архитектуру",
  "direction": "Frontend"
}
```

#### Удалить профессию
```
DELETE /api/professions/{id}
```

### Направления (Directions)
Каждая профессия имеет направление:
- **Frontend** — разработка пользовательских интерфейсов
- **Backend** — серверная логика и базы данных
- **FullStack** — полная разработка
- **DevOps** — управление инфраструктурой
- **QA** — тестирование
- **Mobile** — мобильная разработка
- **Data** — работа с данными
- **AI/ML** — машинное обучение
- **Security** — безопасность
- **Other** — другое

---

## 📈 Карьерные траектории (Trajectory Generation)

### Что это
Система может автоматически сгенерировать рекомендуемый путь развития для каждой профессии, разделяя навыки на три уровня: Junior, Middle, Senior.

### Как использовать

#### Генерировать траекторию
```
POST /api/trajectory/generate
Body:
{
  "professionId": "guid-of-profession",
  "maxItemsPerLevel": 10
}
```

**Параметры:**
- `professionId` (обязательно): ID профессии, для которой генерируем
- `maxItemsPerLevel` (опционально): максимум навыков на уровень

**Что происходит:**
1. Система находит профессию
2. Ищет все навыки, связанные с этой профессией
3. Запускает AI для анализа требуемых уровней
4. Разделяет навыки на Junior, Middle, Senior
5. Возвращает структурированную траекторию

**Пример ответа:**
```json
{
  "professionId": "guid",
  "professionName": "Frontend Developer",
  "trajectory": {
    "junior": [
      "HTML",
      "CSS",
      "JavaScript",
      "Git",
      "REST API basics"
    ],
    "middle": [
      "React",
      "TypeScript",
      "Redux",
      "Testing (Jest)",
      "Performance optimization"
    ],
    "senior": [
      "System Architecture",
      "Mentoring",
      "Code Review",
      "Team Leadership",
      "Accessibility (A11y)"
    ]
  }
}
```

---

## 🧪 Тестирование AI моделей

### Что это
Встроенная поддержка для экспериментов с разными AI моделями (OpenAI, OpenRouter, Groq). Позволяет сравнивать результаты на одних и тех же промптах.

### Поддерживаемые модели

**OpenAI:**
- gpt-4
- gpt-4-turbo
- gpt-3.5-turbo

**OpenRouter:**
- openai/gpt-4
- openai/gpt-3.5-turbo
- deepseek/deepseek-chat
- meta-llama/llama-2-70b-chat

**Groq:**
- mixtral-8x7b-32768
- llama2-70b-4096

### API для тестирования

```
POST /api/ai-models-testing/test-extraction
Body:
{
  "vacancyDescription": "We need a React developer with 5+ years...",
  "model": "openai/gpt-4",
  "temperature": 0.7
}
```

---

## 🔌 MCP интеграция (для Claude Code)

### Что это
MCP (Model Context Protocol) — позволяет подключить систему к Claude Code и использовать AI для умного ревью навыков.

### Включённые инструменты

После подключения MCP, Claude Code получает доступ к инструментам для:
- Чтения списка навыков, требующих проверки
- Умного анализа соответствия навыков
- Автоматической нормализации и предложения синонимов
- Одобрения или отклонения навыков
- Предложения уровней и направлений

### Как подключить

1. Убедиться, что backend запущен:
```bash
docker-compose up
```

2. Добавить MCP сервер в Claude Code:
```bash
claude mcp add hephaestus-skills --transport sse http://localhost:7168/mcp
```

3. Использовать в Claude Code:
```
Помогите мне проверить и одобрить все новые навыки 
в системе Hephaestus. Используйте MCP tools.
```

Claude автоматически:
- Получит список навыков на проверку
- Проанализирует каждый
- Предложит действия
- Одобрит или отклонит с аргументацией

---

## 📊 Типичные сценарии использования

### Сценарий 1: Загрузить вакансии и извлечь навыки

```bash
# 1. Загрузить вакансии
curl -X GET "http://localhost:7168/api/vacancy/save?search=React"

# 2. Запустить извлечение навыков
curl -X POST "http://localhost:7168/api/skills/import-from-vacancies"

# 3. Смотреть что получилось
curl -X GET "http://localhost:7168/api/skills/on-review"

# 4. Одобрить первый навык (требует знания ID)
curl -X POST "http://localhost:7168/api/skills/on-review/{id}/approve" \
  -H "Content-Type: application/json" \
  -d '{
    "displayName": "React",
    "skillType": "Framework",
    "direction": "Frontend"
  }'
```

### Сценарий 2: Создать профессию и сгенерировать траекторию

```bash
# 1. Создать профессию
curl -X POST "http://localhost:7168/api/professions" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Frontend Developer",
    "description": "Web UI developer",
    "direction": "Frontend"
  }'

# 2. Генерировать траекторию (требует ID профессии)
curl -X POST "http://localhost:7168/api/trajectory/generate" \
  -H "Content-Type: application/json" \
  -d '{
    "professionId": "{guid-from-step-1}",
    "maxItemsPerLevel": 10
  }'

# 3. Посмотреть результат
curl -X GET "http://localhost:7168/api/professions/{guid}/trajectory"
```

### Сценарий 3: Используя Claude Code для ревью

```
Я добавил 50 новых вакансий в Hephaestus.
Помоги мне быстро проверить и одобрить все новые навыки.
Используй MCP tools из Hephaestus для этого.
```

Claude:
- Получит список 50+ новых навыков
- Проанализирует каждый
- Предложит решение (новый, синоним, ошибка)
- Одобрит правильные
- Предложит синонимы для похожих

---

## 🔧 Нормализация навыков (как это работает)

### Правила нормализации

1. **Маленькие буквы**
   - "React" → "react"
   - "JAVA" → "java"

2. **Удаление точек и специальных символов**
   - "C#" → "c"
   - ".NET" → "net"
   - "C++" → "c"

3. **Удаление лишних пробелов**
   - "React  JS" → "reactjs"

4. **Распознавание синонимов**
   - "JS" ← "JavaScript"
   - "PY" ← "Python"
   - "K8s" ← "Kubernetes"

5. **Поддержка кириллицы**
   - "Питон" → "python"
   - "Яваскрипт" → "javascript"
   - "Си-шарп" → "c#"

---

## ⚙️ Настройка и конфигурация

### Переменные окружения

```
# Database
DATABASE_URL=postgresql://user:password@localhost:5432/hephaestus

# HeadHunter API
HEADHUNTER_ACCESS_TOKEN=your_token_here
HEADHUNTER_CLIENT_SECRET=your_secret_here

# OpenAI API
OPENAI_API_KEY=your_key_here

# OpenRouter API
OPENROUTER_API_KEY=your_key_here

# Groq API
GROQ_API_KEY=your_key_here
```

### Порты

- **Backend API:** http://localhost:7168 (HTTP) или https://localhost:7169 (HTTPS)
- **PostgreSQL:** localhost:5432
- **Frontend:** http://localhost:3001 (если запущен отдельно)

---

## 📝 Примеры ответов API

### Вакансия
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "headHunterId": 123456,
  "vacancyName": "Senior React Developer",
  "vacancyDescription": "We are looking for...",
  "url": "https://hh.ru/vacancy/123456",
  "isProcessed": false,
  "keySkills": [
    {
      "name": "React",
      "id": "550e8400-e29b-41d4-a716-446655440001"
    },
    {
      "name": "TypeScript",
      "id": "550e8400-e29b-41d4-a716-446655440002"
    }
  ]
}
```

### Навык на проверке
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440003",
  "originalName": "React.js",
  "normalizedName": "reactjs",
  "suggestedDisplayName": "React",
  "counter": 5,
  "status": "pending",
  "skillType": null,
  "direction": null,
  "level": null,
  "professionId": null,
  "createdAt": "2024-05-27T10:30:00Z",
  "updatedAt": "2024-05-27T10:30:00Z"
}
```

### Нормализованный навык
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440004",
  "displayName": "React",
  "normalizedName": "react",
  "description": "JavaScript library for building user interfaces",
  "counter": 250,
  "skillType": "Framework",
  "direction": "Frontend",
  "level": "Middle",
  "professionId": "550e8400-e29b-41d4-a716-446655440005",
  "professionName": "Frontend Developer",
  "createdAt": "2024-05-20T08:15:00Z",
  "updatedAt": "2024-05-27T10:30:00Z",
  "synonyms": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440006",
      "synonymName": "react.js",
      "isFromNormalization": false,
      "createdAt": "2024-05-20T08:20:00Z"
    }
  ],
  "parentSkills": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440007",
      "displayName": "JavaScript",
      "normalizedName": "javascript",
      "relationType": "requires"
    }
  ],
  "dependentSkills": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440008",
      "displayName": "Redux",
      "normalizedName": "redux",
      "relationType": "complementary"
    }
  ]
}
```

### Профессия
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440005",
  "name": "Frontend Developer",
  "description": "Web UI developer",
  "direction": "Frontend",
  "createdAt": "2024-05-15T09:00:00Z",
  "updatedAt": "2024-05-27T10:30:00Z"
}
```

### Карьерная траектория
```json
{
  "professionId": "550e8400-e29b-41d4-a716-446655440005",
  "professionName": "Frontend Developer",
  "trajectory": {
    "junior": ["HTML", "CSS", "JavaScript", "Git", "REST API"],
    "middle": ["React", "TypeScript", "Redux", "Testing", "Performance"],
    "senior": ["Architecture", "Mentoring", "System Design", "Leadership"]
  }
}
```
