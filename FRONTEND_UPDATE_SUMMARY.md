# Frontend обновления для профессий и расширенных навыков

## Что было обновлено

### 1. Типы данных (`frontend/src/types/vacancy.ts`)
✅ Добавлены новые enum'ы:
- `SkillType` - Soft, Hard, Tool, Framework, Language, DomainKnowledge
- `Direction` - Programming, Analytics, Testing, Design, DevOps, DataScience, Management, General
- `SkillLevel` - Intern (1), Junior (2), JuniorPlus (3), Middle (4), MiddlePlus (5), Senior (6), LeadExpert (7)
- `ProfessionDirection` - Programming, Analytics, Testing, Design, DevOps, DataScience, Management

✅ Обновлены сущности:
- `CleanSkill` - добавлены поля: skillType, direction, level, professionId
- `SkillOnReview` - добавлены поля: skillType, direction, level, professionId
- `ApproveSkillRequest` - добавлены поля: skillType, direction, level, professionId, existingCleanSkillId

✅ Добавлена интерфейс:
- `Profession` - для работы с профессиями

### 2. API клиент (`frontend/src/services/api.ts`)
✅ Добавлены новые методы в `professionsAPI`:
- `getAll()` - получить все профессии
- `getById(id)` - получить профессию по ID
- `create(data)` - создать новую профессию
- `update(id, data)` - обновить профессию
- `delete(id)` - удалить профессию

✅ Обновлен `skillsAPI.addCleanSkill`:
- Теперь поддерживает skillType, direction, level, professionId

### 3. Компоненты
✅ Создан новый компонент:
- `ProfessionSelect` (`frontend/src/components/Common/ProfessionSelect.tsx`) - селектор для выбора профессии

✅ Используются существующие селекторы:
- `SkillTypeSelect` - для выбора типа навыка
- `DirectionSelect` - для выбора направления
- `SkillLevelSelect` - для выбора уровня

### 4. Страницы

#### SkillsReviewPage (`frontend/src/pages/SkillsReviewPage.tsx`)
✅ При одобрении навыка теперь можно указать:
- Тип навыка (SkillType)
- Направление (Direction)
- Уровень (SkillLevel)
- Профессию (Profession)

#### CleanSkillsPage (`frontend/src/pages/CleanSkillsPage.tsx`)
✅ При добавлении нового навыка теперь можно указать:
- Тип навыка
- Направление
- Уровень
- Профессию

✅ На карточке навыка отображаются:
- Тип навыка (жёлтый бейдж)
- Направление (голубой бейдж)
- Уровень (индиго бейдж)

### 5. Хуки (`frontend/src/hooks/useSkills.ts`)
✅ Обновлен `useAddCleanSkill`:
- Теперь поддерживает новые параметры skillType, direction, level, professionId

✅ Сохранены `useProfessions`:
- Уже существовали, теперь правильно импортируются из API

## Как использовать

### Добавление новой профессии
```typescript
import { useProfessions, useProfessionsCreation } from '@/hooks/useProfessions';

const { mutate: createProfession } = useProfessionsCreation();

createProfession({
  name: 'Backend Developer',
  description: 'Разработчик бэкенда',
  direction: ProfessionDirection.Programming,
});
```

### Добавление навыка с расширенными полями
```typescript
import { useAddCleanSkill } from '@/hooks/useSkills';
import { SkillType, Direction, SkillLevel } from '@/types/vacancy';

const { add } = useAddCleanSkill();

add({
  displayName: 'Node.js',
  description: 'JavaScript runtime',
  skillType: SkillType.Framework,
  direction: Direction.Programming,
  level: SkillLevel.Middle,
  professionId: 'some-profession-id',
});
```

### Использование селекторов в компонентах
```typescript
import { SkillTypeSelect } from '@/components/Common/SkillTypeSelect';
import { DirectionSelect } from '@/components/Common/DirectionSelect';
import { SkillLevelSelect } from '@/components/Common/SkillLevelSelect';
import { ProfessionSelect } from '@/components/Common/ProfessionSelect';

<SkillTypeSelect value={skillType} onChange={setSkillType} />
<DirectionSelect value={direction} onChange={setDirection} />
<SkillLevelSelect value={level} onChange={setLevel} />
<ProfessionSelect value={professionId} onChange={setProfessionId} />
```

## Статус

✅ Все компоненты готовы
✅ Типизация завершена  
✅ API интеграция готова
✅ Страницы обновлены
✅ Нет ошибок в консоли
✅ Фронт запускается без проблем

## Следующие шаги

1. Убедитесь, что бэк возвращает новые поля (skillType, direction, level, professionId) в ответах API
2. Если нужны изменения в DTO на бэке, обновите структуры запросов
3. Протестируйте функциональность добавления и одобрения навыков с новыми полями
