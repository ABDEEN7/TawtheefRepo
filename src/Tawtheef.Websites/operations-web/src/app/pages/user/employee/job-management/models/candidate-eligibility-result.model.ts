import { CandidateEligibilityCondition } from './candidate-eligibility-condition.model';

export interface CandidateEligibilityProfileSummary {
  status: string;
  availableForRecruitment: boolean;
  targetEntityId?: string;
  genderId?: string;
  birthDate?: string;
  qualificationLevelIds: string[];
  skillIds: string[];
}

export interface CandidatePointsBreakdown {
  categoryPoints: number;
  educationPoints: number;
  experiencePoints: number;
  trainingPoints: number;
  skillPoints: number;
  languagePoints: number;
  certificatePoints: number;
  totalPoints: number;
  details?: PointDetail[];
}

export interface PointDetail {
  section: string;
  description: string;
  sectionAr?: string;
  sectionEn?: string;
  descriptionAr?: string;
  descriptionEn?: string;
  itemsAr?: string[];
  itemsEn?: string[];
  points: number;
}


export interface JobEligibilityRequirementSummary {
  targetEntityId?: string;
  genderId?: string;
  maximumAge?: number;
  minimumAge?: number;
  qualificationLevelIds: string[];
  requiredSkillIds: string[];
}



export interface CandidateEligibilityResult {
  jobId: string;
  candidateId: string;
  candidateName: string;
  nationalNumber: string;
  isEligible: boolean;
  appearsInEligibleCandidatesQuery: boolean;
  conditions: CandidateEligibilityCondition[];
  profile?: CandidateEligibilityProfileSummary;
  jobRequirements?: JobEligibilityRequirementSummary;
  pointsBreakdown?: CandidatePointsBreakdown;
}