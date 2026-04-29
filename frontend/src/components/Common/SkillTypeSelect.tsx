import { SkillType } from '@/types/vacancy';

interface SkillTypeSelectProps {
  value?: SkillType;
  onChange: (value: SkillType) => void;
  required?: boolean;
}

const SKILL_TYPES: { value: SkillType; label: string }[] = [
  { value: SkillType.Soft, label: 'Soft Skill' },
  { value: SkillType.Hard, label: 'Hard Skill' },
  { value: SkillType.Tool, label: 'Tool' },
  { value: SkillType.Framework, label: 'Framework' },
  { value: SkillType.Language, label: 'Language' },
  { value: SkillType.DomainKnowledge, label: 'Domain Knowledge' },
];

export function SkillTypeSelect({ value, onChange, required }: SkillTypeSelectProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        Skill Type {required && <span className="text-red-500">*</span>}
      </label>
      <select
        value={value || ''}
        onChange={(e) => onChange(e.target.value as SkillType)}
        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        required={required}
      >
        <option value="">-- Select Skill Type --</option>
        {SKILL_TYPES.map((type) => (
          <option key={type.value} value={type.value}>
            {type.label}
          </option>
        ))}
      </select>
    </div>
  );
}
