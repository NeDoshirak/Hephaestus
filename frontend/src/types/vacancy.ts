// Enums
export enum SkillType {
  Soft = 'Soft',
  Hard = 'Hard',
  Tool = 'Tool',
  Framework = 'Framework',
  Language = 'Language',
  DomainKnowledge = 'DomainKnowledge',
}

export enum Direction {
  Programming = 'Programming',
  Analytics = 'Analytics',
  Testing = 'Testing',
  Design = 'Design',
  DevOps = 'DevOps',
  DataScience = 'DataScience',
  Management = 'Management',
  General = 'General',
}

export enum SkillLevel {
  Junior = 1,
  Middle = 2,
  Senior = 3,
  Lead = 4,
}

export enum ProfessionDirection {
  Programming = 'Programming',
  Analytics = 'Analytics',
  Testing = 'Testing',
  Design = 'Design',
  DevOps = 'DevOps',
  DataScience = 'DataScience',
  Management = 'Management',
}

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
  skillType?: SkillType;
  direction?: Direction;
  level?: SkillLevel;
  professionId?: string;
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
  skillType?: SkillType;
  direction?: Direction;
  level?: SkillLevel;
  professionId?: string;
  professionName?: string;
  createdAt: string;
  updatedAt: string;
  synonyms: SkillSynonym[];
  dependentSkills?: CleanSkill[];
  parentSkills?: CleanSkill[];
}

export interface Profession {
  id: string;
  name: string;
  description?: string;
  direction: ProfessionDirection;
  createdAt: string;
  updatedAt: string;
}

export interface ApproveSkillRequest {
  displayName: string;
  description?: string;
  synonyms: string[];
  children: { childNormalizedName?: string; parentNormalizedName?: string; relationType: string }[];
  skillType?: string;
  direction?: string;
  level?: string;
  professionId?: string;
  existingCleanSkillId?: string;
}

export interface ApproveSkillResponse {
  cleanSkillId: string;
  normalizedName: string;
  displayName: string;
  skillType?: string;
  direction?: string;
  level?: string;
  professionId?: string;
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
