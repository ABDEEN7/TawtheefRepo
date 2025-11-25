import { Lookups } from "../../../core/models/lookups.model";

export interface ResidentBreakdownResponseDto {
  nationality?: Lookups; 
  percentage: number;     
}
