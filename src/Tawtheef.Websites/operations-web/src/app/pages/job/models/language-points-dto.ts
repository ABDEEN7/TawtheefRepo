import { JobPointRuleTypeEnum } from "../enums/job-point-rule-type";

export interface LanguagePointDto {
  type: JobPointRuleTypeEnum.Language;
  ability: 'speaking' | 'reading' | 'conversation' | 'native';
  level?: 'excellent' | 'veryGood' | 'good';
  points: number;
}