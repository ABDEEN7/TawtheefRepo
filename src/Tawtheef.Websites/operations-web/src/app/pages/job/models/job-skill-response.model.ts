import { GUID } from "../../../shared/types/guid.type";
import { SkillResponseDto } from "./skill-response.model";

export interface JobSkillResponseDto {
  id: GUID;
  skill?: SkillResponseDto;
  showToApplicants: boolean;
}