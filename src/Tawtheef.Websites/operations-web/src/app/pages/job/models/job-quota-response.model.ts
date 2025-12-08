import { GUID } from "../../../shared/types/guid.type";
import { ResidentBreakdownResponse } from "./resident-breakdown-response.model";

export interface JobQuotaResponse {
  id: GUID;
  jobId: GUID;
  qatariCitizens: number;
  qatarMother: number;
  nonQatariSpouse: number;
  gcc: number;
  quGrads: number;
  residents: number;
  residentsBreakdowns?: ResidentBreakdownResponse[];
  createdDate: Date;
  lastModifiedDate?: Date;
}