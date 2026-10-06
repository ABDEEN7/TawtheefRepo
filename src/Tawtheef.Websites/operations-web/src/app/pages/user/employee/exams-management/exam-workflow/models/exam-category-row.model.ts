export type ExamCategoryDifficultyRow = {
  key: 'EASY' | 'MEDIUM' | 'HARD';
  control: 'easyQuestionCount' | 'mediumQuestionCount' | 'hardQuestionCount';
  available: 'easy' | 'medium' | 'hard';
};

export type ExamCategoryTotalRow = { key: 'TOTAL' };
