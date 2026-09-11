import { ExamPartDto } from "./exam-part.dto";

export interface ExamConfigurationDto {
  jobId: string;
  titleAr: string;
  titleEn: string | null;
  allowPreviousQuestion: boolean;
  interruptionPolicyId: string;
  notes: string | null;
  totalQuestions: number;
  decisionNotes: string | null;
  decisionByName?: string | null;
  decisionAt?: string | null;
  statusBackendName: string | null;
  parts: ExamPartDto[];
}
