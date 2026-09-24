import { OperationalIssueModel } from '../../interview-evaluation/models/operational-issue.model';
import { FinalDecision, ResultReportStatus } from './enums';

// Mirrors ResultReportListItemDto.
export interface ResultReportListItemModel {
  id: string;
  code: string;
  interviewScheduleId: string;
  scheduleTitleAr: string;
  scheduleTitleEn?: string | null;
  jobNameAr: string;
  jobNameEn?: string | null;
  appliedQualificationScore: number | null;
  status: ResultReportStatus;
  candidateCount: number;
  approvedAt: string | null;
  // Soft-deleted because an appointment was rescheduled before approval - listed read-only, never opened.
  isDiscarded: boolean;
  discardedAt: string | null;
  discardReason: string | null;
}

// Mirrors ResultCandidateAxisDto.
export interface ResultCandidateAxisModel {
  axisId: string;
  axisNameAr?: string | null;
  axisNameEn?: string | null;
  score: number;
  qualificationScore: number | null;
  qualificationMet: boolean | null;
}

// Mirrors ResultCandidateDto. suggestedDecision is computed server-side at read time
// (ResultCandidateSuggestionService) and only seeds the Final Decision dropdown - never persisted.
export interface ResultCandidateModel {
  id: string;
  interviewAppointmentId: string;
  candidateNameAr: string;
  candidateNameEn?: string | null;
  candidateQid: string | null;
  finalScore: number;
  qualificationScore: number | null;
  isQualified: boolean;
  finalDecision: FinalDecision | null;
  decisionReason: string | null;
  suggestedDecision: FinalDecision;
  snapshotAt: string;
  operationalIssues: OperationalIssueModel[];
  axes: ResultCandidateAxisModel[];
}

// Mirrors ResultReportDto.
export interface ResultReportModel {
  id: string;
  code: string;
  interviewScheduleId: string;
  scheduleTitleAr: string;
  scheduleTitleEn?: string | null;
  jobNameAr: string;
  jobNameEn?: string | null;
  appliedQualificationScore: number | null;
  status: ResultReportStatus;
  approvedById: string | null;
  approvedAt: string | null;
  decisionNotes: string | null;
  candidates: ResultCandidateModel[];
}
