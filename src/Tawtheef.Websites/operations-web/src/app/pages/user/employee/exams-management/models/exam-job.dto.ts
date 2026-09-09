import { ExamLookupItemDto } from "./exam-lookups.dto";

export interface ExamJobDto extends ExamLookupItemDto {
  jobNumber: string;
  managementAr: string | null;
  managementEn: string | null;
  departmentAr: string | null;
  departmentEn: string | null;
  specializationsAr: string[];
  specializationsEn: string[];
}