import { useState, useCallback } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { vacancyAPI } from '@/services/api';
import { RawVacancy, CreateVacancyRequest, UpdateVacancyRequest } from '@/types/vacancy';

export function useVacancies(page: number = 1, pageSize: number = 10, search?: string, skillFilter?: string) {
  const queryClient = useQueryClient();

  // Fetch vacancies
  const {
    data: listData,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['vacancies', page, pageSize, search, skillFilter],
    queryFn: () => vacancyAPI.list(page, pageSize, search, skillFilter),
  });

  // Create vacancy mutation
  const createMutation = useMutation({
    mutationFn: (data: CreateVacancyRequest) => vacancyAPI.create(data),
    onSuccess: () => {
      // Инвалидировать все vacancies queries чтобы они перезагрузились
      queryClient.invalidateQueries({ queryKey: ['vacancies'] });
    },
  });

  // Update vacancy mutation
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateVacancyRequest }) =>
      vacancyAPI.update(id, data),
    onSuccess: () => {
      // Инвалидировать все vacancies queries чтобы они перезагрузились
      queryClient.invalidateQueries({ queryKey: ['vacancies'] });
    },
  });

  // Delete vacancy mutation
  const deleteMutation = useMutation({
    mutationFn: (id: string) => vacancyAPI.delete(id),
    onSuccess: (_, deletedId) => {
      // Инвалидировать все vacancies queries чтобы они перезагрузились
      queryClient.invalidateQueries({ queryKey: ['vacancies'] });
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
