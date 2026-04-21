import { useQuery } from '@tanstack/react-query';
import { hhAPI } from '@/services/api';

export function useHhSearch(searchQuery: string, page: number = 0, pageSize: number = 20) {
  return useQuery({
    queryKey: ['hhSearch', searchQuery, page],
    queryFn: () => hhAPI.searchVacancies(searchQuery, page, pageSize),
    staleTime: 1000 * 60 * 15, // 15 минут - результаты поиска кешируются
    gcTime: 1000 * 60 * 30, // 30 минут - держать в памяти дольше
    enabled: !!searchQuery.trim(), // Не запускать без query
    retry: 2,
  });
}

export function useHhVacancyDetails(vacancyId: string | null) {
  return useQuery({
    queryKey: ['hhVacancyDetails', vacancyId],
    queryFn: () => hhAPI.getVacancyDetails(vacancyId!),
    staleTime: 1000 * 60 * 60, // 1 час - детали вакансии не меняются
    gcTime: 1000 * 60 * 120, // 2 часа
    enabled: !!vacancyId,
    retry: 2,
  });
}
