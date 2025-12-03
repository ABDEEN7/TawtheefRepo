import { Lookups } from "../../../core/models/lookups.model";

export interface SkillResponseDto {
  skill: Lookups;
  isEssential: boolean;
  major: Lookups;
  skillType: Lookups;
  skillRequirementType: Lookups;
}