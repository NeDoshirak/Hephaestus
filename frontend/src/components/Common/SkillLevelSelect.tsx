import { SkillLevel } from '@/types/vacancy';

interface SkillLevelSelectProps {
  value?: SkillLevel;
  onChange: (value: SkillLevel) => void;
  required?: boolean;
}

const SKILL_LEVELS: { value: SkillLevel; label: string }[] = [
  { value: SkillLevel.Intern, label: 'Intern (1)' },
  { value: SkillLevel.Junior, label: 'Junior (2)' },
  { value: SkillLevel.JuniorPlus, label: 'Junior+ (3)' },
  { value: SkillLevel.Middle, label: 'Middle (4)' },
  { value: SkillLevel.MiddlePlus, label: 'Middle+ (5)' },
  { value: SkillLevel.Senior, label: 'Senior (6)' },
  { value: SkillLevel.LeadExpert, label: 'Lead/Expert (7)' },
];

export function SkillLevelSelect({ value, onChange, required }: SkillLevelSelectProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        Level {required && <span className="text-red-500">*</span>}
      </label>
      <select
        value={value !== undefined ? value : ''}
        onChange={(e) => onChange(parseInt(e.target.value) as SkillLevel)}
        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        required={required}
      >
        <option value="">-- Select Level --</option>
        {SKILL_LEVELS.map((level) => (
          <option key={level.value} value={level.value}>
            {level.label}
          </option>
        ))}
      </select>
    </div>
  );
}
