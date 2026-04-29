import { SkillType } from '@/types/vacancy';

interface SkillTypeSelectProps {
  value?: SkillType;
  onChange: (value: SkillType) => void;
  required?: boolean;
}

const SKILL_TYPES: { value: SkillType; label: string }[] = [
  { value: SkillType.Soft, label: 'Soft Skill (мягкий)' },
  { value: SkillType.Hard, label: 'Hard Skill (жесткий)' },
  { value: SkillType.Tool, label: 'Инструмент' },
  { value: SkillType.Framework, label: 'Фреймворк' },
  { value: SkillType.Language, label: 'Язык' },
  { value: SkillType.DomainKnowledge, label: 'Доменное знание' },
];

export function SkillTypeSelect({ value, onChange, required }: SkillTypeSelectProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        Тип навыка {required && <span className="text-red-500">*</span>}
      </label>
      <select
        value={value || ''}
        onChange={(e) => onChange(e.target.value as SkillType)}
        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        required={required}
      >
        <option value="">Выберите тип</option>
        {SKILL_TYPES.map((type) => (
          <option key={type.value} value={type.value}>
            {type.label}
          </option>
        ))}
      </select>
    </div>
  );
}
