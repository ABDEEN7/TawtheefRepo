import { JobPointRuleTypeEnum } from "../enums/job-point-rule-type";

export interface JobPointsDetailResponse {
  id: string;
  type: JobPointRuleTypeEnum;
  code: string;
  name?: string | null;
  points: number;
  referenceId?: string | null;
}
