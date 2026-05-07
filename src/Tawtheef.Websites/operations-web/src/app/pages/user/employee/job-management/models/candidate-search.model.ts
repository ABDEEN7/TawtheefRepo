import { GUID } from "../../../../../shared/types/guid.type";

export interface CandidateSearchDto {
  candidateId: GUID;
  fullName: string;
  nationalId: string;
  email: string;
}
