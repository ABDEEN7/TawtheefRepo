import { GUID } from '../../../../../shared/types/guid.type';

export interface JobCandidatePointsBreakdown {
  categoryPoints: number;
  educationPoints: number;
  experiencePoints: number;
  trainingPoints: number;
  skillPoints: number;
  languagePoints: number;
  certificatePoints: number;
  totalPoints: number;
}

export interface JobCandidateProfile {
  candidateId: GUID;
  candidateName: string | null;
  email: string | null;
  phoneNumber: string | null;
  nationalNumber: string | null;
  age: number | null;
  candidateType: string | null;
  gender: string | null;
  nationality: string | null;
  experienceYears: number;
  points: JobCandidatePointsBreakdown;
}
