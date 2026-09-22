import { CommitteeRole } from '../../interview-schedule/models/enums';
import { AppointmentStatus } from '../../interview-schedule/models/enums';
import { MemberEvaluationStatus } from './enums';

// Mirrors MemberEvaluationFormCriterionDto.
export interface EvaluationCriterionModel {
  criterionId: string;
  nameAr?: string | null;
  nameEn?: string | null;
  descriptionAr?: string | null;
  descriptionEn?: string | null;
  maxScore: number;
  isRequired: boolean;
  orderNo: number;
  score: number | null;
  notes: string | null;
}

// Mirrors MemberEvaluationFormAxisDto.
export interface EvaluationAxisModel {
  axisId: string;
  axisNameAr?: string | null;
  axisNameEn?: string | null;
  maxScore: number;
  orderNo: number;
  criteria: EvaluationCriterionModel[];
}

// Mirrors MemberEvaluationFormDto - "my own form" for the current user on one appointment.
export interface MemberEvaluationFormModel {
  evaluationId: string | null;
  interviewAppointmentId: string;
  interviewCommitteeMemberId: string;
  status: MemberEvaluationStatus | null;
  totalScore: number | null;
  generalNotes: string | null;
  canEdit: boolean;
  axes: EvaluationAxisModel[];
}

// Mirrors MemberEvaluationSummaryDto.
export interface MemberEvaluationSummaryModel {
  interviewCommitteeMemberId: string;
  memberUserId: string;
  memberFullNameAr: string;
  memberFullNameEn?: string | null;
  role: CommitteeRole;
  status: MemberEvaluationStatus | null;
  totalScore: number | null;
  submittedAt: string | null;
}

// Mirrors AppointmentEvaluationSummaryDto - Chair/CanViewCommitteeSummary/HR-bypass only (403 otherwise).
export interface AppointmentEvaluationSummaryModel {
  interviewAppointmentId: string;
  appointmentStatus: AppointmentStatus;
  members: MemberEvaluationSummaryModel[];
}

// Mirrors AppointmentEvaluationContextDto - "who am I on this appointment's committee", resolved
// without the form's CanSubmitEvaluation requirement so a non-scoring Chair still resolves.
export interface AppointmentEvaluationContextModel {
  interviewCommitteeMemberId: string | null;
  role: CommitteeRole | null;
  canSubmitEvaluation: boolean;
  canViewCommitteeSummary: boolean;
  hasChairOrBypassAccess: boolean;
}
