export interface ExamCategoryDto {
  categoryId: string;
  questionBankVersionId: string;
  questionCount: number;
  weightPercent: number;
  easyQuestionCount: number;
  mediumQuestionCount: number;
  hardQuestionCount: number;
}