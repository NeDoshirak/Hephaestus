import { useState, useEffect } from 'react';
import { Profession, ProfessionDirection } from '@/types/vacancy';
import { Input } from '@/components/Common/Input';
import { Button } from '@/components/Common/Button';

interface ProfessionFormProps {
  profession?: Profession;
  onSubmit: (data: { name: string; description?: string; direction: ProfessionDirection }) => void;
  isLoading?: boolean;
}

const PROFESSION_DIRECTIONS: { value: ProfessionDirection; label: string }[] = [
  { value: ProfessionDirection.Programming, label: 'Программирование' },
  { value: ProfessionDirection.Analytics, label: 'Аналитика' },
  { value: ProfessionDirection.Testing, label: 'Тестирование' },
  { value: ProfessionDirection.Design, label: 'Дизайн' },
  { value: ProfessionDirection.DevOps, label: 'DevOps' },
  { value: ProfessionDirection.DataScience, label: 'Data Science' },
  { value: ProfessionDirection.Management, label: 'Управление' },
];

export function ProfessionForm({ profession, onSubmit, isLoading }: ProfessionFormProps) {
  const [formData, setFormData] = useState({
    name: profession?.name || '',
    description: profession?.description || '',
    direction: profession?.direction || ('' as ProfessionDirection | ''),
  });

  const [errors, setErrors] = useState<{ name?: string; direction?: string }>({});

  useEffect(() => {
    if (profession) {
      setFormData({
        name: profession.name,
        description: profession.description || '',
        direction: profession.direction,
      });
    }
  }, [profession]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const newErrors: { name?: string; direction?: string } = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Название обязательно';
    }
    if (!formData.direction) {
      newErrors.direction = 'Направление обязательно';
    }

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    setErrors({});
    onSubmit({
      name: formData.name.trim(),
      description: formData.description.trim() || undefined,
      direction: formData.direction as ProfessionDirection,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        label="Название"
        value={formData.name}
        onChange={(e) => {
          setFormData({ ...formData, name: e.target.value });
          if (errors.name) setErrors({ ...errors, name: undefined });
        }}
        placeholder="Например: Backend разработчик"
        error={errors.name}
        required
      />

      <div>
        <label className="block text-sm font-medium text-dark mb-2">Описание (опционально)</label>
        <textarea
          value={formData.description}
          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
          placeholder="Добавьте описание для этой профессии..."
          rows={3}
          className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-dark mb-2">
          Направление <span className="text-red-500">*</span>
        </label>
        <select
          value={formData.direction}
          onChange={(e) => {
            setFormData({ ...formData, direction: e.target.value as ProfessionDirection });
            if (errors.direction) setErrors({ ...errors, direction: undefined });
          }}
          className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent"
          required
        >
          <option value="">-- Выберите направление --</option>
          {PROFESSION_DIRECTIONS.map((dir) => (
            <option key={dir.value} value={dir.value}>
              {dir.label}
            </option>
          ))}
        </select>
        {errors.direction && <p className="text-red-500 text-sm mt-1">{errors.direction}</p>}
      </div>

      <Button type="submit" disabled={isLoading} isLoading={isLoading} className="w-full">
        {profession ? 'Обновить' : 'Создать'} профессию
      </Button>
    </form>
  );
}
