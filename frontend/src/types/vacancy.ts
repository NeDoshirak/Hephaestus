export interface KeySkill {
  id: string;
  name: string;
}

export interface RawVacancy {
  id: string;
  headHunterId: number;
  vacancyName: string;
  vacancyDescription: string;
  url: string;
  isProcessed: boolean;
  keySkills?: KeySkill[];
}

export interface CreateVacancyRequest {
  vacancyName: string;
  vacancyDescription: string;
  url?: string;
}

export interface UpdateVacancyRequest {
  vacancyName?: string;
  vacancyDescription?: string;
  url?: string;
  isProcessed?: boolean;
}

export interface VacancyListResponse {
  total: number;
  page: number;
  pageSize: number;
  items: RawVacancy[];
}
