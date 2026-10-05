import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { Select } from 'primeng/select';
import { Tooltip } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';

import { AuthService } from '../../../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { DialogHelperService } from '../../../../../../../core/services/dialog-helper.service';
import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';

import {
  ATTENDANCE_STATUS_LABELS,
  AttendanceStatus,
  OperationalIssueStatus,
} from '../../../interview-evaluation/models/enums';

import { InterviewResultReportStore } from '../../interview-result-report.store';
import { InterviewResultReportFacade } from '../../interview-result-report.facade';
import { ResultCandidateAxisModel, ResultCandidateModel } from '../../models/result-report.model';
import {
  FINAL_DECISION_LABELS,
  FINAL_DECISION_PILL,
  FinalDecision,
  QUALIFIED_ONLY_DECISIONS,
  RESULT_REPORT_STATUS_LABELS,
  RESULT_REPORT_STATUS_PILL,
  ResultReportStatus,
  SELECTABLE_FINAL_DECISIONS,
} from '../../models/enums';
import { OperationalNotesDialogComponent } from '../../components/operational-notes-dialog/operational-notes-dialog';

@Component({
  selector: 'app-report-detail',
  templateUrl: './report-detail.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, DecimalPipe, FormsModule, TranslatePipe, TableModule, Select, Tooltip, HasPermissionDirective],
})
export class ReportDetailComponent {
  readonly Permissions = Permissions;

  store = inject(InterviewResultReportStore);
  service = inject(InterviewResultReportFacade);
  private auth = inject(AuthService);
  private dialogService = inject(DialogService);
  private dialogHelper = inject(DialogHelperService);
  private translate = inject(TranslateService);

  private readonly canManage = this.auth.hasPermission(Permissions.InterviewResultReport.Manage);

  // Decisions are editable only while the report is under review and only for Manage holders - an
  // approved report (or a View-only user) sees the persisted decision as a read-only badge.
  editable = computed(() => this.store.isUnderReview() && this.canManage);

  // Unqualified candidates can only be Rejected (InterviewResultCandidate.Decide) - the other two are
  // shown but disabled rather than hidden, so the reviewer sees why they can't pick them.
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

  // An open blocking operational issue makes the server refuse approval
  // (INTERVIEW_RESULT_REPORT_BLOCKED_BY_OPERATIONAL_ISSUE) - surfaced up front instead of on click.
  hasOpenBlockingIssue = computed(
    () =>
      this.store
        .report()
        ?.candidates.some((c) => c.operationalIssues.some((i) => i.isBlocking && i.status === OperationalIssueStatus.Open)) ??
      false,
  );

  canApprove = computed(
    () => this.editable() && this.store.allDecided() && !this.hasOpenBlockingIssue() && !this.store.approving(),
  );

  jobName = computed(() => {
    const r = this.store.report();
    return r ? this.service.localized(r.jobNameAr, r.jobNameEn) : '';
  });

  scheduleTitle = computed(() => {
    const r = this.store.report();
    return r ? this.service.localized(r.scheduleTitleAr, r.scheduleTitleEn) : '';
  });

  statusLabel(status: ResultReportStatus): string {
    return RESULT_REPORT_STATUS_LABELS[status];
  }

  statusPill(status: ResultReportStatus): string {
    return RESULT_REPORT_STATUS_PILL[status];
  }

  decisionOptions(candidate: ResultCandidateModel) {
    return candidate.isQualified ? this.qualifiedOptions : this.unqualifiedOptions;
  }

  decisionLabel(decision: FinalDecision): string {
    return FINAL_DECISION_LABELS[decision];
  }

  decisionPill(decision: FinalDecision): string {
    return FINAL_DECISION_PILL[decision];
  }

  candidateName(candidate: ResultCandidateModel): string {
    return this.service.localized(candidate.candidateNameAr, candidate.candidateNameEn);
  }

  // "Absent" / "Withdrew" - the reason the candidate's appointment was closed with a 0 score.
  closedAttendanceLabel(candidate: ResultCandidateModel): string | null {
    const status = candidate.attendanceStatus as AttendanceStatus | null;
    return status === AttendanceStatus.NoShow || status === AttendanceStatus.Withdrew
      ? ATTENDANCE_STATUS_LABELS[status]
      : null;
  }

  schoolStageName(candidate: ResultCandidateModel): string {
    return this.service.localized(candidate.recommendedSchoolStageNameAr, candidate.recommendedSchoolStageNameEn);
  }

  axisName(axis: ResultCandidateAxisModel): string {
    return this.service.localized(axis.axisNameAr, axis.axisNameEn);
  }

  isOverridden(candidate: ResultCandidateModel): boolean {
    return this.store.decisions()[candidate.id] !== candidate.suggestedDecision;
  }

  openIssuesCount(candidate: ResultCandidateModel): number {
    return candidate.operationalIssues.filter((i) => i.status === OperationalIssueStatus.Open).length;
  }

  viewNotes(candidate: ResultCandidateModel) {
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

  approve() {
    if (!this.canApprove()) return;
    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_RESULT_REPORT.DETAIL.CONFIRM_APPROVE_TITLE',
        description: 'INTERVIEW_RESULT_REPORT.DETAIL.CONFIRM_APPROVE_DESCRIPTION',
        confirmText: 'INTERVIEW_RESULT_REPORT.DETAIL.APPROVE',
        cancelText: 'INTERVIEW_RESULT_REPORT.CANCEL',
      })
      ?.onClose.subscribe((confirmed: boolean | undefined) => {
        if (confirmed) this.service.approve();
      });
  }
}
