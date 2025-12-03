import { Lookups } from "../../../core/models/lookups.model";
import { GUID } from "../../../shared/types/guid.type";

export interface ResidentBreakdownResponse {
  id: GUID;
  nationality?: Lookups;
  percentage: number;
}