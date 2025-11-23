import {ResidentBreakdown} from './resident-breakdown.model';

export interface JobQuotas {
  qatariCitizens: number;
  qatarMother: number;
  nonQatariSpouse: number;
  gcc: number;
  quGrads: number;
  residents: number;
  residentsBreakdown: ResidentBreakdown[];
}
