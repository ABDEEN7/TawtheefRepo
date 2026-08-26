import { StatusCount } from './dashboard-common.model';

export interface ProfileBreakdown {
  byStatus: StatusCount[];
  byCandidateType: CandidateTypeCount[];
  cohorts: CandidateCohorts;
}

export interface CandidateCohorts {
  includeOfficeProfiles: boolean;
  registeredKawaderProfiles: number;
  registeredMinisterOfficeProfiles: number;
  qatarGraduateProfiles: number;
  officeProfiles: number;
}

export interface CandidateTypeCount {
  candidateTypeId?: string;
  key: string;
  label: string;
  count: number;
}
