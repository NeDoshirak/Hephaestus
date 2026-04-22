import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { skillsAPI } from '@/services/api';

export function useSkillsOnReview() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['skillsOnReview'],
    queryFn: () => skillsAPI.getOnReview(),
    staleTime: 1000 * 60 * 5, // 5 minutes
  });

  return {
    skills: data?.items || [],
    isLoading,
    error: error instanceof Error ? error.message : null,
  };
}

export function useCleanSkills() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['cleanSkills'],
    queryFn: () => skillsAPI.getClean(),
    staleTime: 1000 * 60 * 30, // 30 minutes
  });

  return {
    skills: data?.items || [],
    isLoading,
    error: error instanceof Error ? error.message : null,
  };
}

export function useImportSkills() {
  const queryClient = useQueryClient();
  const [successMessage, setSuccessMessage] = useState<string>('');

  const importMutation = useMutation({
    mutationFn: (filters?: { vacancyNameFilter?: string; limit?: number }) =>
      skillsAPI.importSkills(filters?.vacancyNameFilter, filters?.limit),
    onSuccess: (data) => {
      setSuccessMessage(
        `Успешно! Добавлено на проверку: ${data.skillsAddedToReview}, совпадает существующих: ${data.skillsMatchedExisting}`
      );
      queryClient.invalidateQueries({ queryKey: ['skillsOnReview'] });
      queryClient.invalidateQueries({ queryKey: ['cleanSkills'] });
      setTimeout(() => setSuccessMessage(''), 5000);
    },
    onError: (error) => {
      const message = error instanceof Error ? error.message : 'Ошибка при импорте скиллов';
      setSuccessMessage(`Ошибка: ${message}`);
      setTimeout(() => setSuccessMessage(''), 5000);
    },
  });

  return {
    import: importMutation.mutate,
    isImporting: importMutation.isPending,
    successMessage,
  };
}

export function useApproveSkill() {
  const queryClient = useQueryClient();
  const [successMessage, setSuccessMessage] = useState<string>('');

  const approveMutation = useMutation({
    mutationFn: (params: { skillId: string; request: Parameters<typeof skillsAPI.approveSkill>[1] }) =>
      skillsAPI.approveSkill(params.skillId, params.request),
    onSuccess: (data) => {
      setSuccessMessage(`Навык "${data.displayName}" успешно верифицирован!`);
      queryClient.invalidateQueries({ queryKey: ['skillsOnReview'] });
      queryClient.invalidateQueries({ queryKey: ['cleanSkills'] });
      setTimeout(() => setSuccessMessage(''), 5000);
    },
    onError: (error) => {
      const message = error instanceof Error ? error.message : 'Ошибка при аппруве навыка';
      setSuccessMessage(`Ошибка: ${message}`);
      setTimeout(() => setSuccessMessage(''), 5000);
    },
  });

  return {
    approve: approveMutation.mutate,
    isApproving: approveMutation.isPending,
    successMessage,
  };
}

export function useRejectSkill() {
  const queryClient = useQueryClient();
  const [successMessage, setSuccessMessage] = useState<string>('');

  const rejectMutation = useMutation({
    mutationFn: (skillId: string) => skillsAPI.rejectSkill(skillId),
    onSuccess: () => {
      setSuccessMessage('Навык успешно отклонён');
      queryClient.invalidateQueries({ queryKey: ['skillsOnReview'] });
      setTimeout(() => setSuccessMessage(''), 3000);
    },
    onError: (error) => {
      const message = error instanceof Error ? error.message : 'Ошибка при отклонении навыка';
      setSuccessMessage(`Ошибка: ${message}`);
      setTimeout(() => setSuccessMessage(''), 3000);
    },
  });

  return {
    reject: rejectMutation.mutate,
    isRejecting: rejectMutation.isPending,
    successMessage,
  };
}

export function useAddCleanSkill() {
  const queryClient = useQueryClient();
  const [successMessage, setSuccessMessage] = useState<string>('');

  const addMutation = useMutation({
    mutationFn: (data: { displayName: string; description?: string; normalizedName?: string }) =>
      skillsAPI.addCleanSkill(data),
    onSuccess: (data) => {
      setSuccessMessage(`Навык "${data.displayName}" успешно добавлен!`);
      queryClient.invalidateQueries({ queryKey: ['cleanSkills'] });
      setTimeout(() => setSuccessMessage(''), 5000);
    },
    onError: (error) => {
      const message = error instanceof Error ? error.message : 'Ошибка при добавлении навыка';
      setSuccessMessage(`Ошибка: ${message}`);
      setTimeout(() => setSuccessMessage(''), 5000);
    },
  });

  return {
    add: addMutation.mutate,
    isAdding: addMutation.isPending,
    successMessage,
  };
}
