import { ResidentBreakdownResponseDto } from "./resident-breakdownresponse-Dto";

export interface JobQuotasResponseDto {
  qatariCitizens: number;
  qatarMother: number;
  nonQatariSpouse: number;
  gcc: number;
  quGrads: number;
  residents: number;
  residentsBreakdowns: ResidentBreakdownResponseDto[];
}
