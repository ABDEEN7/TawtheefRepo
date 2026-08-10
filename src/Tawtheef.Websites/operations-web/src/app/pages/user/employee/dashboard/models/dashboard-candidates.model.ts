import { StatusCount } from './dashboard-common.model';

export interface ProfileBreakdown {
  byStatus: StatusCount[];
  byCandidateType: CandidateTypeCount[];
}

export interface CandidateTypeCount {
  candidateTypeId?: string;
  key: string;
  label: string;
  count: number;
}

export interface CandidateTypeKpis {
  total: number;
  qatari: number;
  nonQatari: number;
  sonOfQatariMother: number;
  wifeOfQatari: number;
  gcc: number;
  residentQatar: number;
  unknown: number;
}
