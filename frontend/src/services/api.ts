import axios, { AxiosInstance } from 'axios';
import { RawVacancy, CreateVacancyRequest, UpdateVacancyRequest, VacancyListResponse } from '@/types/vacancy';

const API_BASE_URL = '/api';

const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 404) {
      return Promise.reject(new Error('Ресурс не найден'));
    }
    if (error.response?.status >= 500) {
      return Promise.reject(new Error('Ошибка сервера'));
    }
    return Promise.reject(error.response?.data || new Error('Ошибка сети'));
  }
);

export const vacancyAPI = {
  // Get paginated list of vacancies with optional search
  async list(page: number = 1, pageSize: number = 10, search?: string): Promise<VacancyListResponse> {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);

    const response = await apiClient.get<VacancyListResponse>('/vacancy', { params });
    return response.data;
  },

  // Get single vacancy by ID
  async getById(id: string): Promise<RawVacancy> {
    const response = await apiClient.get<RawVacancy>(`/vacancy/${id}`);
    return response.data;
  },

  // Create new vacancy
  async create(data: CreateVacancyRequest): Promise<RawVacancy> {
    const response = await apiClient.post<RawVacancy>('/vacancy', data);
    return response.data;
  },

  // Update existing vacancy
  async update(id: string, data: UpdateVacancyRequest): Promise<RawVacancy> {
    const response = await apiClient.put<RawVacancy>(`/vacancy/${id}`, data);
    return response.data;
  },

  // Delete vacancy
  async delete(id: string): Promise<void> {
    await apiClient.delete(`/vacancy/${id}`);
  },
};

export const importAPI = {
  // Import vacancies from HeadHunter
  async importVacancies(search: string): Promise<void> {
    await apiClient.get('/vacancy/save', { params: { search } });
  },
};

export const hhAPI = {
  // Search vacancies on HeadHunter (raw API)
  async searchVacancies(text: string, page: number = 0, perPage: number = 10) {
    const response = await apiClient.get('/hh/vacancies', {
      params: { text, page, perPage },
    });
    return response.data;
  },

  // Get vacancy details from HeadHunter (raw API)
  async getVacancyDetails(vacancyId: string) {
    const response = await apiClient.get(`/hh/vacancies/${vacancyId}`);
    return response.data;
  },
};

export default apiClient;
