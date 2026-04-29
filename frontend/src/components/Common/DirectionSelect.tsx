import { Direction } from '@/types/vacancy';

interface DirectionSelectProps {
  value?: Direction;
  onChange: (value: Direction) => void;
  required?: boolean;
}

const DIRECTIONS: { value: Direction; label: string }[] = [
  { value: Direction.Programming, label: 'Programming' },
  { value: Direction.Analytics, label: 'Analytics' },
  { value: Direction.Testing, label: 'Testing' },
  { value: Direction.Design, label: 'Design' },
  { value: Direction.DevOps, label: 'DevOps' },
  { value: Direction.DataScience, label: 'Data Science' },
  { value: Direction.Management, label: 'Management' },
  { value: Direction.General, label: 'General' },
];

export function DirectionSelect({ value, onChange, required }: DirectionSelectProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        Direction {required && <span className="text-red-500">*</span>}
      </label>
      <select
        value={value || ''}
        onChange={(e) => onChange(e.target.value as Direction)}
        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        required={required}
      >
        <option value="">-- Select Direction --</option>
        {DIRECTIONS.map((dir) => (
          <option key={dir.value} value={dir.value}>
            {dir.label}
          </option>
        ))}
      </select>
    </div>
  );
}
