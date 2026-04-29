import { SkillLevel } from '@/types/vacancy';

interface SkillLevelSelectProps {
  value?: SkillLevel;
  onChange: (value: SkillLevel) => void;
  required?: boolean;
}

const SKILL_LEVELS: { value: SkillLevel; label: string }[] = [
  { value: SkillLevel.Junior, label: 'Джун' },
  { value: SkillLevel.Middle, label: 'Мидл' },
  { value: SkillLevel.Senior, label: 'Сеньор' },
  { value: SkillLevel.Lead, label: 'Лид' },
];

export function SkillLevelSelect({ value, onChange, required }: SkillLevelSelectProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        Уровень {required && <span className="text-red-500">*</span>}
      </label>
      <select
        value={value !== undefined ? value : ''}
        onChange={(e) => onChange(parseInt(e.target.value) as SkillLevel)}
        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        required={required}
      >
        <option value="">Выберите уровень</option>
        {SKILL_LEVELS.map((level) => (
          <option key={level.value} value={level.value}>
            {level.label}
          </option>
        ))}
      </select>
    </div>
  );
}
