import { CommitteeRole } from '../../interview-schedule/models/enums';
import { FinalDecision, ResultReportStatus } from '../../interview-result-report/models/enums';
import { ResultCandidateAxisModel } from '../../interview-result-report/models/result-report.model';
import { OperationalIssueModel } from './operational-issue.model';
import { MemberEvaluationStatus } from './enums';

// Mirrors CommitteeReviewListItemDto - one per schedule whose report the caller may review.
export interface CommitteeReviewListItemModel {
  interviewScheduleId: string;
  reportId: string;
  code: string;
  status: ResultReportStatus;
}

// Mirrors CommitteeReviewCriterionDto / CommitteeReviewAxisDto - the evaluation-form structure.
export interface CommitteeReviewCriterionModel {
  criterionId: string;
  nameAr?: string | null;
  nameEn?: string | null;
  maxScore: number;
}

export interface CommitteeReviewAxisModel {
  axisId: string;
  nameAr?: string | null;
  nameEn?: string | null;
  maxScore: number;
  qualificationScore: number | null;
  criteria: CommitteeReviewCriterionModel[];
}

export interface CommitteeReviewCriterionScoreModel {
  criterionId: string;
  score: number;
}

// Mirrors CommitteeReviewMemberDto. scores are only filled for a submitted evaluation.
export interface CommitteeReviewMemberModel {
  interviewCommitteeMemberId: string;
  memberFullNameAr: string;
  memberFullNameEn?: string | null;
  role: CommitteeRole;
  status: MemberEvaluationStatus | null;
  totalScore: number | null;
  submittedAt: string | null;
  scores: CommitteeReviewCriterionScoreModel[];
}

// Mirrors CommitteeReviewCandidateDto.
export interface CommitteeReviewCandidateModel {
  id: string;
  interviewAppointmentId: string;
  candidateNameAr: string;
  candidateNameEn?: string | null;
  candidateQid: string | null;
  finalScore: number;
  qualificationScore: number | null;
  isQualified: boolean;
  attendanceStatus: number | null;
  isLate: boolean;
  suggestedDecision: FinalDecision;
  chairRecommendedDecision: FinalDecision | null;
  chairRecommendationReason: string | null;
  recommendedSchoolStageId: string | null;
  operationalIssues: OperationalIssueModel[];
  axes: ResultCandidateAxisModel[];
  criterionAverages: CommitteeReviewCriterionScoreModel[];
  members: CommitteeReviewMemberModel[];
}

// Mirrors CommitteeReviewDto.
export interface CommitteeReviewModel {
  reportId: string;
  code: string;
  interviewScheduleId: string;
  scheduleTitleAr: string;
  scheduleTitleEn?: string | null;
  jobNameAr: string;
  jobNameEn?: string | null;
  appliedQualificationScore: number | null;
  status: ResultReportStatus;
  committeeReviewedAt: string | null;
  canEdit: boolean;
  canOverrideSuggestion: boolean;
  axes: CommitteeReviewAxisModel[];
  candidates: CommitteeReviewCandidateModel[];
}

// Mirrors CommitteeReviewCandidateInputDto. recommendedDecision null = keep the system suggestion.
export interface CommitteeReviewCandidateInput {
  candidateId: string;
  recommendedDecision: FinalDecision | null;
  reason: string | null;
  schoolStageId: string | null;
}

// Mirrors SaveCommitteeReviewCommand.
export interface SaveCommitteeReviewPayload {
  reportId: string;
  candidates: CommitteeReviewCandidateInput[];
  sendForApproval: boolean;
}

// Mirrors DropdownOptions as returned by the school-stages lookup.
export interface SchoolStageOption {
  id: string;
  backendName: string;
  name: string;
  additionalData?: { nameAr?: string; nameEn?: string } | null;
}
