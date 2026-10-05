import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { Select } from 'primeng/select';
import { Tooltip } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';

import { DialogHelperService } from '../../../../../../../core/services/dialog-helper.service';

import {
  FINAL_DECISION_LABELS,
  FINAL_DECISION_PILL,
  FinalDecision,
  QUALIFIED_ONLY_DECISIONS,
  RESULT_REPORT_STATUS_LABELS,
  RESULT_REPORT_STATUS_PILL,
  ResultReportStatus,
  SELECTABLE_FINAL_DECISIONS,
} from '../../../interview-result-report/models/enums';
import { ResultCandidateAxisModel } from '../../../interview-result-report/models/result-report.model';
import { OperationalNotesDialogComponent } from '../../../interview-result-report/components/operational-notes-dialog/operational-notes-dialog';

import { InterviewEvaluationStore } from '../../interview-evaluation.store';
import { InterviewEvaluationFacade } from '../../interview-evaluation.facade';
import {
  ATTENDANCE_STATUS_LABELS,
  AttendanceStatus,
  MEMBER_EVALUATION_STATUS_LABELS,
  MEMBER_EVALUATION_STATUS_PILL,
  MEMBER_ROLE_LABELS,
  MemberEvaluationStatus,
  OperationalIssueStatus,
} from '../../models/enums';
import {
  CommitteeReviewAxisModel,
  CommitteeReviewCandidateModel,
  CommitteeReviewCriterionModel,
  CommitteeReviewMemberModel,
} from '../../models/committee-review.model';

// Committee Head Review: the whole evaluation of every candidate in the schedule (member scores per
// criterion, axis and final scores, qualification, system suggestion, operational info) before the
// report goes to Approve Interview Results. The chair may override the suggestion only with the
// OverrideSuggestion permission (also enforced server-side) and may add a recommended school stage,
// which never affects any score or the suggestion.
@Component({
  selector: 'app-committee-review',
  templateUrl: './committee-review.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, DecimalPipe, FormsModule, TranslatePipe, Select, Tooltip],
})
export class CommitteeReviewComponent {
  store = inject(InterviewEvaluationStore);
  service = inject(InterviewEvaluationFacade);
  private dialogService = inject(DialogService);
  private dialogHelper = inject(DialogHelperService);
  private translate = inject(TranslateService);

  canOverride = computed(() => this.store.review()?.canOverrideSuggestion ?? false);

  jobName = computed(() => {
    const r = this.store.review();
    return r ? this.service.localized(r.jobNameAr, r.jobNameEn) : '';
  });

  scheduleTitle = computed(() => {
    const r = this.store.review();
    return r ? this.service.localized(r.scheduleTitleAr, r.scheduleTitleEn) : '';
  });

  schoolStageOptions = computed(() =>
    this.store.schoolStages().map((s) => ({
      value: s.id,
      label: this.service.localized(s.additionalData?.nameAr, s.additionalData?.nameEn) || s.name,
    })),
  );

  // Unqualified candidates can only be Rejected - the other two stay visible but disabled, same as
  // the approval screen (InterviewResultCandidate.SetChairRecommendation enforces it server-side).
  private readonly qualifiedOptions = SELECTABLE_FINAL_DECISIONS.map((value) => ({
    value,
    label: FINAL_DECISION_LABELS[value],
    disabled: false,
  }));
  private readonly unqualifiedOptions = SELECTABLE_FINAL_DECISIONS.map((value) => ({
    value,
    label: FINAL_DECISION_LABELS[value],
    disabled: QUALIFIED_ONLY_DECISIONS.includes(value),
  }));

  decisionOptions(candidate: CommitteeReviewCandidateModel) {
    return candidate.isQualified ? this.qualifiedOptions : this.unqualifiedOptions;
  }

  statusLabel(status: ResultReportStatus): string {
    return RESULT_REPORT_STATUS_LABELS[status];
  }

  statusPill(status: ResultReportStatus): string {
    return RESULT_REPORT_STATUS_PILL[status];
  }

  decisionLabel(decision: FinalDecision): string {
    return FINAL_DECISION_LABELS[decision];
  }

  decisionPill(decision: FinalDecision): string {
    return FINAL_DECISION_PILL[decision];
  }

  candidateName(candidate: CommitteeReviewCandidateModel): string {
    return this.service.localized(candidate.candidateNameAr, candidate.candidateNameEn);
  }

  memberName(member: CommitteeReviewMemberModel): string {
    return this.service.localized(member.memberFullNameAr, member.memberFullNameEn);
  }

  memberRoleLabel(member: CommitteeReviewMemberModel): string {
    return MEMBER_ROLE_LABELS[member.role];
  }

  memberStatusLabel(status: MemberEvaluationStatus): string {
    return MEMBER_EVALUATION_STATUS_LABELS[status];
  }

  memberStatusPill(status: MemberEvaluationStatus): string {
    return MEMBER_EVALUATION_STATUS_PILL[status];
  }

  axisName(axis: CommitteeReviewAxisModel): string {
    return this.service.localized(axis.nameAr, axis.nameEn);
  }

  criterionName(criterion: CommitteeReviewCriterionModel): string {
    return this.service.localized(criterion.nameAr, criterion.nameEn);
  }

  // "Absent" / "Withdrew" - why the candidate's appointment was closed with a 0 score.
  closedAttendanceLabel(candidate: CommitteeReviewCandidateModel): string | null {
    const status = candidate.attendanceStatus as AttendanceStatus | null;
    return status === AttendanceStatus.NoShow || status === AttendanceStatus.Withdrew
      ? ATTENDANCE_STATUS_LABELS[status]
      : null;
  }

  // Stored axis score from the report (not recalculated here).
  axisScore(candidate: CommitteeReviewCandidateModel, axisId: string): ResultCandidateAxisModel | null {
    return candidate.axes.find((a) => a.axisId === axisId) ?? null;
  }

  memberScore(member: CommitteeReviewMemberModel, criterionId: string): number | null {
    return member.scores.find((s) => s.criterionId === criterionId)?.score ?? null;
  }

  criterionAverage(candidate: CommitteeReviewCandidateModel, criterionId: string): number | null {
    return candidate.criterionAverages.find((s) => s.criterionId === criterionId)?.score ?? null;
  }

  hasScores(candidate: CommitteeReviewCandidateModel): boolean {
    return candidate.criterionAverages.length > 0;
  }

  isOverridden(candidate: CommitteeReviewCandidateModel): boolean {
    return this.store.reviewDecisions()[candidate.id] !== candidate.suggestedDecision;
  }

  openIssuesCount(candidate: CommitteeReviewCandidateModel): number {
    return candidate.operationalIssues.filter((i) => i.status === OperationalIssueStatus.Open).length;
  }

  viewNotes(candidate: CommitteeReviewCandidateModel) {
    const name = this.candidateName(candidate);
    this.dialogService.open(OperationalNotesDialogComponent, {
      header: this.translate.instant('INTERVIEW_RESULT_REPORT.NOTES_DIALOG.TITLE'),
      width: '40rem',
      data: {
        candidateLabel: candidate.candidateQid ? `${name} (${candidate.candidateQid})` : name,
        issues: candidate.operationalIssues,
      },
    });
  }

  save() {
    if (!this.store.reviewEditable() || this.store.reviewSaving()) return;
    this.service.saveReview(false);
  }

  sendForApproval() {
    if (!this.store.reviewInCommitteeStage() || this.store.reviewSaving()) return;
    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_EVALUATION.REVIEW.CONFIRM_SEND_TITLE',
        description: 'INTERVIEW_EVALUATION.REVIEW.CONFIRM_SEND_DESCRIPTION',
        confirmText: 'INTERVIEW_EVALUATION.REVIEW.SEND_FOR_APPROVAL',
        cancelText: 'INTERVIEW_EVALUATION.REVIEW.CANCEL',
      })
      ?.onClose.subscribe((confirmed: boolean | undefined) => {
        if (confirmed) this.service.saveReview(true);
      });
  }
}
