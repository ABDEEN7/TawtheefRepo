import { Lookups } from "../../../core/models/lookups.model";
import { GUID } from "../../../shared/types/guid.type";

export interface JobDegreeResponseDto {
  id: GUID;
  degree: Lookups;
}