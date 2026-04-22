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

export interface SkillOnReview {
  id: string;
  originalName: string;
  normalizedName: string;
  counter: number;
  status: string;
  suggestedDisplayName: string;
  createdAt: string;
  updatedAt: string;
}

export interface SkillSynonym {
  id: string;
  cleanSkillId: string;
  synonymName: string;
  isFromNormalization: boolean;
  createdAt: string;
}

export interface CleanSkill {
  id: string;
  normalizedName: string;
  displayName: string;
  description?: string;
  counter: number;
  createdAt: string;
  updatedAt: string;
  synonyms: SkillSynonym[];
}

export interface ApproveSkillRequest {
  displayName: string;
  description?: string;
  synonyms: string[];
  children: { childNormalizedName: string; relationType: string }[];
}

export interface ApproveSkillResponse {
  cleanSkillId: string;
  normalizedName: string;
  displayName: string;
  duplicatesProcessed: number;
}

export interface SkillsOnReviewResponse {
  items: SkillOnReview[];
}

export interface CleanSkillsResponse {
  items: CleanSkill[];
}

export interface ImportResult {
  vacanciesProcessed: number;
  skillsAddedToReview: number;
  skillsMatchedExisting: number;
  timestamp: string;
}
