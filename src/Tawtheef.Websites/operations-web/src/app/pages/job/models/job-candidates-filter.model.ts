import { GUID } from '../../../shared/types/guid.type';

export interface JobCandidatesFilter {
  searchTerm?: string;
  jobCategoryId?: GUID;
  candidateTypeId?: GUID;
  genderId?: GUID;
  minimumPoints?: number;
}