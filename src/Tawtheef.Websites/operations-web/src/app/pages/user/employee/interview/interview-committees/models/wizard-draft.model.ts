import { CommitteeMemberModel, EligibleMemberModel } from './committee-member.model';
import { CommitteeRole, EvaluationScope } from './enums';

export type WizardMode = 'create' | 'edit';

export interface WizardInfoDraft {
  jobId: string;
  templateId: string;
  nameAr: string;
  nameEn: string;
  // Not editable in the wizard, but UpdateCommittee replaces them - carry them through so an edit never wipes them.
  scopeDescription: string | null;
  notes: string | null;
}

export function emptyWizardInfoDraft(): WizardInfoDraft {
  return { jobId: '', templateId: '', nameAr: '', nameEn: '', scopeDescription: null, notes: null };
}

/** The job facts the wizard's info panel shows - only what it renders, not the whole job record. */
export interface WizardJobContext {
  id: string;
  titleAr: string;
  titleEn: string | null;
  jobNumber: string | null;
  departmentName: string | null;
  categoryName: string | null;
}

/** Assignable axis: `id` is the InterviewTemplateEvaluationAxis id of the template's Approved version. */
export interface WizardTemplateAxis {
  id: string;
  nameAr: string;
  nameEn: string | null;
}

export interface WizardTemplateContext {
  id: string;
  titleAr: string;
  titleEn: string | null;
  versionNo: number;
  finalScore: number;
  qualificationScore: number | null;
  axes: WizardTemplateAxis[];
}

export interface WizardUser {
  id: string;
  nameAr: string;
  nameEn: string;
  email: string | null;
}

export interface WizardMemberDraft {
  localId: string;
  user: WizardUser | null;
  role: CommitteeRole;
  evaluationScope: EvaluationScope;
  /** InterviewTemplateEvaluationAxis ids; only meaningful when the role evaluates and scope is SelectedAxes. */
  axisIds: string[];
}

export interface MemberPermissions {
  canViewCandidates: boolean;
  canAddNotes: boolean;
  canSubmitEvaluation: boolean;
  canViewOtherEvaluations: boolean;
  canViewCommitteeSummary: boolean;
}

// Only the Chair and Evaluators score candidates; Observers/Admins sit on the committee without an evaluation scope.
export function participatesInEvaluation(role: CommitteeRole): boolean {
  return role === CommitteeRole.Chair || role === CommitteeRole.Evaluator;
}

// Same derivation the approved demo uses (candidates always visible, notes for evaluators/observers, submitting
// for evaluators), extended with the two visibility flags the backend added: only the Chair sees the rest.
export function permissionsForRole(role: CommitteeRole): MemberPermissions {
  const isChair = role === CommitteeRole.Chair;
  return {
    canViewCandidates: true,
    canAddNotes: participatesInEvaluation(role) || role === CommitteeRole.Observer,
    canSubmitEvaluation: participatesInEvaluation(role),
    canViewOtherEvaluations: isChair,
    canViewCommitteeSummary: isChair,
  };
}

export function userFromEligible(member: EligibleMemberModel): WizardUser {
  return { id: member.id, nameAr: member.nameAr, nameEn: member.nameEn, email: member.email ?? null };
}

export function userFromMember(member: CommitteeMemberModel): WizardUser {
  return { id: member.memberUserId, nameAr: member.memberNameAr, nameEn: member.memberNameEn, email: null };
}

export function newWizardMemberDraft(): WizardMemberDraft {
  return {
    localId: crypto.randomUUID(),
    user: null,
    role: CommitteeRole.Evaluator,
    evaluationScope: EvaluationScope.AllAxes,
    axisIds: [],
  };
}

// Axes saved against an older approved version of the template no longer exist on the current one, so they
// are dropped rather than sent back (the backend would reject them).
export function memberDraftFromExisting(member: CommitteeMemberModel, assignableAxisIds: ReadonlySet<string>): WizardMemberDraft {
  const axisIds = member.evaluationAxes
    .map((a) => a.interviewTemplateEvaluationAxisId)
    .filter((id) => assignableAxisIds.has(id));

  return {
    localId: crypto.randomUUID(),
    user: userFromMember(member),
    role: member.role,
    evaluationScope: member.evaluationScope,
    axisIds,
  };
}
