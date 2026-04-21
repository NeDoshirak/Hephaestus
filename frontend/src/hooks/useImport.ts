import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { importAPI } from '@/services/api';

export function useImport() {
  const queryClient = useQueryClient();
  const [successMessage, setSuccessMessage] = useState<string>('');

  const importMutation = useMutation({
    mutationFn: (search: string) => importAPI.importVacancies(search),
    onSuccess: () => {
      setSuccessMessage('Вакансии успешно загружены!');
      // Инвалидировать только первую страницу - там появятся новые вакансии
      queryClient.invalidateQueries({ queryKey: ['vacancies', 1] });
      setTimeout(() => setSuccessMessage(''), 5000);
    },
    onError: (error) => {
      const message = error instanceof Error ? error.message : 'Ошибка при загрузке вакансий';
      setSuccessMessage(`Ошибка: ${message}`);
      setTimeout(() => setSuccessMessage(''), 5000);
    },
  });

  return {
    import: importMutation.mutate,
    isImporting: importMutation.isPending,
    isError: importMutation.isError,
    error: importMutation.error instanceof Error ? importMutation.error.message : null,
    successMessage,
  };
}
