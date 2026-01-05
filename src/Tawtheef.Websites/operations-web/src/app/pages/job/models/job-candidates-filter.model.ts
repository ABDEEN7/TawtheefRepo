import { GUID } from '../../../shared/types/guid.type';
import { NationalityPreferenceRow } from './nationality-preference.model';

export interface JobCandidatesFilter {
  searchTerm?: string;
  genderId?: GUID;
  candidateTypeId?: GUID;
  minimumPoints?: number;
  nationalityPreferences?: NationalityPreferenceRow[];
}