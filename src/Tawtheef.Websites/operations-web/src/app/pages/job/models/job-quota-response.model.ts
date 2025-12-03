import { ResidentBreakdownResponse } from "./resident-breakdown-response.model";

export interface JobQuotaResponse {
  qatariCitizens: number;
  qatarMother: number;
  nonQatariSpouse: number;
  gcc: number;
  quGrads: number;
  residents: number;
  residentsBreakdowns: ResidentBreakdownResponse[];
}