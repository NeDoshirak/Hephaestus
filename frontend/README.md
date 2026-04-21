# Hephaestus Frontend

Админка на React для управления вакансиями. Три основные вкладки: управление вакансиями в БД, загрузка с HH, поиск на HH.

## О проекте

### Архитектура
- **React 18** + **TypeScript** + **Vite** для быстрой разработки
- **React Router** для навигации между вкладками
- **React Query** для кеширования и управления данными (оптимизировано: 30 мин стейлтайм)
- **Tailwind CSS** для стилей
- **Axios** для API запросов

### Три основные страницы
1. **Вакансии** - CRUD операции (список, создание, редактирование, удаление) с пагинацией и поиском
2. **Загрузить с HH** - импорт вакансий с HeadHunter по поисковому запросу
3. **Поиск на HH** - прямой поиск и просмотр деталей вакансий с HH API

### Цветовая схема
- Основной: #5CBBF1 (голубой)
- Тёмный: #313642 (для sidebar)
- Белый: #FFFFFF

### Кеширование (оптимизировано)
- Список вакансий: 30 минут
- Результаты поиска HH: 15 минут  
- Детали вакансии: 1 час
- CRUD операции обновляют только нужные данные в кеше (не перезагружают всё)
- Retry логика: 2 повтора при ошибках

## Быстрый старт

### Требования
- Node.js 16+
- npm или yarn

### Установка и запуск
```bash
cd frontend
npm install
npm run dev
```

Откроется http://localhost:3001 (если 3000 занят)

### Переменные окружения
Скопируй `.env.example` в `.env` и обнови если нужно:
```bash
VITE_API_BASE_URL=http://localhost:5147  # Адрес бэкенда
```

### Сборка для продакшена
```bash
npm run build
```

## Структура проекта

```
src/
├── components/
│   ├── Layout/           # Sidebar, Layout wrapper
│   ├── Vacancies/        # VacancyCard, VacancyForm
│   └── Common/           # Button, Input, Modal, Pagination, Loading
├── pages/
│   ├── VacanciesPage.tsx      # CRUD вакансий
│   ├── ImportPage.tsx         # Загрузка с HH
│   └── HhSearchPage.tsx       # Поиск на HH (с пагинацией)
├── hooks/
│   ├── useVacancies.ts   # Управление вакансиями (optimized mutations)
│   ├── useImport.ts      # Импорт с HH
│   └── useHhSearch.ts    # Поиск на HH (React Query)
├── services/
│   └── api.ts            # Axios клиент + три API (vacancy, import, hhAPI)
├── types/
│   └── vacancy.ts        # TypeScript типы
├── utils/
│   └── html.ts           # Утилиты для очистки/обработки HTML
├── App.tsx               # Роутинг + QueryClient
└── main.tsx
```

## Как расширять

### Добавить новую страницу
1. Создать компонент в `src/pages/NewPage.tsx`
2. Добавить маршрут в `App.tsx`
3. Добавить вкладку в `Sidebar.tsx` (если нужна)

### Добавить новый API запрос
1. Добавить метод в `src/services/api.ts`
2. Создать хук в `src/hooks/` если часто используется
3. Использовать React Query с правильным стейлтайм

### Изменить цвета
- Обновить `tailwind.config.js` (extends.colors)
- Или изменить классы Tailwind в компонентах (#5CBBF1, #313642)

## Docker

### Запуск в контейнере
```bash
docker build -t hephaestus-frontend .
docker run -p 3001:80 hephaestus-frontend
```

### С docker-compose
```bash
docker-compose up
```

### Переменные в контейнере
Установить VITE_API_BASE_URL при запуске:
```bash
docker run -e VITE_API_BASE_URL=http://api:5147 -p 3001:80 hephaestus-frontend
```

## Важные детали для разработки

- **staleTime vs gcTime**: Первый - когда данные считаются "старыми", второй - сколько хранить в памяти
- **Мутации оптимизированы**: Create/Update/Delete не перезагружают весь список - обновляют кеш
- **HTML теги в описаниях**: Отображаются как полноценный HTML (не удаляются)
- **Пагинация**: Показывает максимум totalPages кнопок, не даёт перейти на несуществующую страницу

## API Endpoints (из бэкенда)

**Вакансии:**
- `GET /api/vacancy?page=1&pageSize=10&search=` - список
- `GET /api/vacancy/{id}` - одна вакансия
- `POST /api/vacancy` - создать
- `PUT /api/vacancy/{id}` - обновить
- `DELETE /api/vacancy/{id}` - удалить

**Импорт:**
- `GET /api/vacancy/save?search=React` - загрузить с HH

**Прямой доступ к HH API:**
- `GET /api/hh/vacancies?text=React&page=0&perPage=20` - поиск
- `GET /api/hh/vacancies/{vacancyId}` - детали

## Troubleshooting

- **CORS ошибка** → проверить что бэкенд запущен на правильном порту
- **Кнопка не работает** → проверить DevTools (Network/Console)
- **Данные не обновляются** → очистить кеш: откроется = перезагрузить страницу
