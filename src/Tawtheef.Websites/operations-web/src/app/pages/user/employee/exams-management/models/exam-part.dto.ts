import { ExamCategoryDto } from "./exam-category.dto";

export interface ExamPartDto {
  partNo: number;
  titleAr: string;
  titleEn: string | null;
  durationMinutes: number;
  qualificationScore: number | null;
  categories: ExamCategoryDto[];
}