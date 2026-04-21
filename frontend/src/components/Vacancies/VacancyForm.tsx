import { FC, useState, useEffect } from 'react';
import { RawVacancy, CreateVacancyRequest, UpdateVacancyRequest } from '@/types/vacancy';
import { Button } from '@/components/Common/Button';
import { Input } from '@/components/Common/Input';

interface VacancyFormProps {
  vacancy?: RawVacancy;
  onSubmit: (data: CreateVacancyRequest | UpdateVacancyRequest) => void;
  isLoading?: boolean;
}

export const VacancyForm: FC<VacancyFormProps> = ({
  vacancy,
  onSubmit,
  isLoading,
}) => {
  const [formData, setFormData] = useState<CreateVacancyRequest>({
    vacancyName: '',
    vacancyDescription: '',
    url: '',
  });

  useEffect(() => {
    if (vacancy) {
      setFormData({
        vacancyName: vacancy.vacancyName,
        vacancyDescription: vacancy.vacancyDescription,
        url: vacancy.url || '',
      });
    }
  }, [vacancy]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        label="Название вакансии"
        name="vacancyName"
        value={formData.vacancyName}
        onChange={handleChange}
        placeholder="Введите название вакансии"
        required
      />

      <div>
        <label className="block text-sm font-medium text-dark mb-2">
          Описание
        </label>
        <textarea
          name="vacancyDescription"
          value={formData.vacancyDescription}
          onChange={handleChange}
          placeholder="Введите описание вакансии"
          rows={4}
          className="w-full px-4 py-2 rounded-lg border border-gray-200 focus:border-transparent focus:ring-2 focus:ring-primary text-dark placeholder-gray-400 resize-none transition-all"
          required
        />
      </div>

      <Input
        label="URL (опционально)"
        name="url"
        type="url"
        value={formData.url}
        onChange={handleChange}
        placeholder="https://example.com"
      />

      <div className="flex gap-2 pt-4">
        <Button
          type="submit"
          isLoading={isLoading}
          className="flex-1"
        >
          {vacancy ? 'Обновить' : 'Создать'}
        </Button>
      </div>
    </form>
  );
};
