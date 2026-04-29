import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { professionsAPI } from '@/services/api';
import { ProfessionDirection } from '@/types/vacancy';

export function useProfessions() {
  return useQuery({
    queryKey: ['professions'],
    queryFn: () => professionsAPI.getAll(),
    staleTime: 1000 * 60 * 60, // 1 hour
  });
}

export function useProfessionsCreation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: { name: string; description?: string; direction: ProfessionDirection }) =>
      professionsAPI.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['professions'] });
    },
  });
}

export function useProfessionsUpdate() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, ...data }: { id: string; name: string; description?: string; direction: ProfessionDirection }) =>
      professionsAPI.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['professions'] });
    },
  });
}

export function useProfessionsDeletion() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => professionsAPI.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['professions'] });
    },
  });
}
