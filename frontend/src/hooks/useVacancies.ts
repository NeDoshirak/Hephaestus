import { useState, useCallback } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { vacancyAPI } from '@/services/api';
import { RawVacancy, CreateVacancyRequest, UpdateVacancyRequest } from '@/types/vacancy';

export function useVacancies(page: number = 1, pageSize: number = 10, search?: string) {
  const queryClient = useQueryClient();

  // Fetch vacancies
  const {
    data: listData,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['vacancies', page, pageSize, search],
    queryFn: () => vacancyAPI.list(page, pageSize, search),
  });

  // Create vacancy mutation
  const createMutation = useMutation({
    mutationFn: (data: CreateVacancyRequest) => vacancyAPI.create(data),
    onSuccess: (newVacancy) => {
      // Добавить новую вакансию в кеш первой страницы (где она появится)
      queryClient.setQueryData(['vacancies', 1, pageSize, search], (old: any) => {
        if (!old) return old;
        return {
          ...old,
          items: [newVacancy, ...old.items],
          total: old.total + 1,
        };
      });
      // Инвалидировать остальные страницы так как позиции сдвинулись
      queryClient.invalidateQueries({ queryKey: ['vacancies'], refetchPage: (page) => page !== 0 });
    },
  });

  // Update vacancy mutation
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateVacancyRequest }) =>
      vacancyAPI.update(id, data),
    onSuccess: (updatedVacancy) => {
      // Обновить вакансию во всех кешах где она есть
      queryClient.setQueryData(['vacancies', page, pageSize, search], (old: any) => {
        if (!old) return old;
        return {
          ...old,
          items: old.items.map((item: RawVacancy) =>
            item.id === updatedVacancy.id ? updatedVacancy : item
          ),
        };
      });
    },
  });

  // Delete vacancy mutation
  const deleteMutation = useMutation({
    mutationFn: (id: string) => vacancyAPI.delete(id),
    onSuccess: (_, deletedId) => {
      // Удалить вакансию из кеша текущей страницы
      queryClient.setQueryData(['vacancies', page, pageSize, search], (old: any) => {
        if (!old) return old;
        return {
          ...old,
          items: old.items.filter((item: RawVacancy) => item.id !== deletedId),
          total: Math.max(0, old.total - 1),
        };
      });
      // Инвалидировать первую страницу чтобы обновился счётчик
      queryClient.invalidateQueries({ queryKey: ['vacancies', 1, pageSize, search] });
    },
  });

  return {
    vacancies: listData?.items ?? [],
    total: listData?.total ?? 0,
    page: listData?.page ?? page,
    pageSize: listData?.pageSize ?? pageSize,
    isLoading,
    isError,
    error: error instanceof Error ? error.message : 'Неизвестная ошибка',
    createVacancy: createMutation.mutate,
    updateVacancy: updateMutation.mutate,
    deleteVacancy: deleteMutation.mutate,
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
    isDeleting: deleteMutation.isPending,
  };
}
