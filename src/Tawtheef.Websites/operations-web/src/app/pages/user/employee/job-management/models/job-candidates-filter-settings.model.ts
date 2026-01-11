import { GUID } from '../../../../../shared/types/guid.type';

export interface JobCandidateTypePercentage {
  candidateTypeId: GUID;
  percentage: number;
}

export interface JobCandidateNationalityPercentage {
  candidateTypeId: GUID;
  nationalityId: GUID;
  percentage: number;
}

export interface JobCandidatesFilterSettings {
  jobId: GUID;
  genderId?: GUID;
  minimumPoints?: number;
  candidateTypePercentages: JobCandidateTypePercentage[];
  nationalityPercentages: JobCandidateNationalityPercentage[];
}
