import { CommitteeRole, EvaluationScope } from './enums';

export interface CommitteeMemberAxisModel {
  id: string;
  interviewTemplateEvaluationAxisId: string;
  axisNameAr: string;
  axisNameEn?: string | null;
}

export interface CommitteeMemberModel {
  id: string;
  interviewCommitteeId: string;
  memberUserId: string;
  memberNameAr: string;
  memberNameEn: string;
  role: CommitteeRole;
  participatesInEvaluation: boolean;
  evaluationScope: EvaluationScope;
  canViewCandidates: boolean;
  canAddNotes: boolean;
  canSubmitEvaluation: boolean;
  canViewOtherEvaluations: boolean;
  canViewCommitteeSummary: boolean;
  isActive: boolean;
  removedAt?: string | null;
  removalReason?: string | null;
  evaluationAxes: CommitteeMemberAxisModel[];
}

/** An employee who holds InterviewCommittee View/Manage - the only users the pickers may offer. */
export interface EligibleMemberModel {
  id: string;
  nameAr: string;
  nameEn: string;
  email?: string | null;
}
