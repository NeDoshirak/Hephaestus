import axios, { AxiosInstance } from 'axios';
import { RawVacancy, CreateVacancyRequest, UpdateVacancyRequest, VacancyListResponse, ApproveSkillRequest, ApproveSkillResponse, SkillsOnReviewResponse, CleanSkillsResponse, ImportResult } from '@/types/vacancy';

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
  // Get paginated list of vacancies with optional search and skill filter
  async list(page: number = 1, pageSize: number = 10, search?: string, skillFilter?: string): Promise<VacancyListResponse> {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (search) params.append('search', search);
    if (skillFilter) params.append('skillFilter', skillFilter);

    const response = await apiClient.get<VacancyListResponse>('/vacancy', { params });
    return response.data;
  },

  // Get all available skills
  async getSkills(): Promise<{ items: string[] }> {
    const response = await apiClient.get<{ items: string[] }>('/vacancy/skills');
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
  // Import vacancies from HeadHunter with optional filter
  async importVacancies(search: string, vacancyNameFilter?: string): Promise<ImportResult> {
    const params: Record<string, string> = { search };
    if (vacancyNameFilter) params.vacancyNameFilter = vacancyNameFilter;
    const response = await apiClient.get<ImportResult>('/vacancy/save', { params });
    return response.data;
  },
};

export const skillsAPI = {
  // Get skills on review
  async getOnReview(): Promise<SkillsOnReviewResponse> {
    const response = await apiClient.get<SkillsOnReviewResponse>('/skills/on-review');
    return response.data;
  },

  // Get clean (verified) skills
  async getClean(): Promise<CleanSkillsResponse> {
    const response = await apiClient.get<CleanSkillsResponse>('/skills/clean');
    return response.data;
  },

  // Import skills from unprocessed vacancies
  async importSkills(vacancyNameFilter?: string, limit?: number): Promise<ImportResult> {
    const params: Record<string, string | number> = {};
    if (vacancyNameFilter) params.vacancyNameFilter = vacancyNameFilter;
    if (limit) params.limit = limit;
    const response = await apiClient.post<ImportResult>('/skills/import-from-vacancies', {}, { params });
    return response.data;
  },

  // Approve a skill on review
  async approveSkill(skillId: string, request: ApproveSkillRequest): Promise<ApproveSkillResponse> {
    const response = await apiClient.post<ApproveSkillResponse>(`/skills/on-review/${skillId}/approve`, request);
    return response.data;
  },

  // Reject a skill on review
  async rejectSkill(skillId: string): Promise<void> {
    await apiClient.post(`/skills/on-review/${skillId}/reject`);
  },

  // Add a new clean skill
  async addCleanSkill(data: { displayName: string; description?: string; normalizedName?: string }): Promise<any> {
    const response = await apiClient.post('/skills/clean/add', data);
    return response.data;
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
