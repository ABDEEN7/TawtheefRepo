import {ResidentBreakdown} from './resident-breakdown.model';

export interface JobQuota {
  qatariCitizens: number;
  qatarMother: number;
  nonQatariSpouse: number;
  gcc: number;
  quGrads: number;
  residents: number;
  residentsBreakdowns?: ResidentBreakdown[];
}
