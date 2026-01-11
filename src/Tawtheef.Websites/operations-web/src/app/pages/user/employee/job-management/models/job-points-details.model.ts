import { JobPointRuleTypeEnum } from "../enums/job-point-rule-type";

export interface JobPointsDetail {
  type: JobPointRuleTypeEnum;   
  code: string;             
  name?: string;            
  points: number;
  referenceId?: string;     
}