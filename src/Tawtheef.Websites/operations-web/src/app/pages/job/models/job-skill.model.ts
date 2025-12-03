import { GUID } from "../../../shared/types/guid.type";

export interface JobSkill {
  id?: GUID;
  skillId?: GUID;
  showToApplicants: boolean;
}