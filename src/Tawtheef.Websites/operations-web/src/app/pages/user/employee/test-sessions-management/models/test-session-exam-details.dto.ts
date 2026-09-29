export interface TestSessionExamPartDto {
  partNo: number;
  durationMinutes: number;
}

export interface TestSessionExamDetailsDto {
  examId: string;
  examName: string;
  jobTitle: string;
  specialization?: string | null;
  mainSpecialization?: string | null;
  subSpecialization?: string | null;
  examStatus: string;
  totalCandidates: number;
  durationMinutes: number;
  parts: TestSessionExamPartDto[];
  numberOfQuestions: number;
  qualificationScore?: number | null;
}
