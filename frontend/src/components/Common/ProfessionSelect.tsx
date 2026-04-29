import { useProfessions } from '@/hooks/useProfessions';

interface ProfessionSelectProps {
  value?: string;
  onChange: (value: string | undefined) => void;
  label?: string;
}

export function ProfessionSelect({ value, onChange, label = 'Профессия' }: ProfessionSelectProps) {
  const { data: professions = [] } = useProfessions();

  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        {label}
      </label>
      <select
        value={value || ''}
        onChange={(e) => onChange(e.target.value || undefined)}
        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
      >
        <option value="">Без профессии</option>
        {professions.map((profession) => (
          <option key={profession.id} value={profession.id}>
            {profession.name}
          </option>
        ))}
      </select>
    </div>
  );
}
