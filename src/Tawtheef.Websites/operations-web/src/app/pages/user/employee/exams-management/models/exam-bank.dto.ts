import { ExamLookupItemDto } from "./exam-lookups.dto";

export interface ExamBankDto extends ExamLookupItemDto {
  questionBankTypeId: string;
  versionNo: number;
  total: number;
  easy: number;
  medium: number;
  hard: number;
}