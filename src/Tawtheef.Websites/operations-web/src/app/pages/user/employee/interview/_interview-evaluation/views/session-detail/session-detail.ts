import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { Tooltip } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';

import { Permissions } from '../../../../../../../core/constants/permissions';

import { AppointmentModel } from '../../../interview-schedule/models/appointment.model';
import { AppointmentStatus, ScheduleStatus } from '../../../interview-schedule/models/enums';

import { InterviewEvaluationStore } from '../../interview-evaluation.store';
import { InterviewEvaluationFacade } from '../../interview-evaluation.facade';
import { AppointmentDetailsDialogComponent } from '../../components/appointment-details-dialog/appointment-details-dialog';
import { OperationalIssuesDialogComponent } from '../../components/operational-issues-dialog/operational-issues-dialog';
// Local label maps, not interview-schedule's - this page only loads its own i18nNamespace JSON
// (see EVAL_SCHEDULE_STATUS_LABELS's comment in models/enums.ts).
import {
  ATTENDANCE_STATUS_LABELS,
  ATTENDANCE_STATUS_PILL,
  AttendanceStatus,
  EVAL_APPOINTMENT_STATUS_LABELS,
  EVAL_APPOINTMENT_STATUS_PILL,
  EVAL_INTERVIEW_TYPE_LABELS,
  EVAL_SCHEDULE_STATUS_LABELS,
  EVAL_SCHEDULE_STATUS_PILL,
  MemberEvaluationStatus,
  OperationalIssueStatus,
} from '../../models/enums';

// Appointments that no longer represent a live, evaluable candidate slot - excluded from the stat
// cards below, same rule the sessions-list Progress column already uses (see the facade's
// INACTIVE_APPOINTMENT_STATUSES).
const INACTIVE_STATUSES = [AppointmentStatus.Cancelled, AppointmentStatus.Rescheduled];

@Component({
  selector: 'app-session-detail',
  templateUrl: './session-detail.html',
  styleUrl: './session-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, TranslatePipe, TableModule, Tooltip],
})
export class SessionDetailComponent {
  readonly Permissions = Permissions;
  readonly AppointmentStatus = AppointmentStatus;

  store = inject(InterviewEvaluationStore);
  service = inject(InterviewEvaluationFacade);
  private dialogService = inject(DialogService);
  private translate = inject(TranslateService);

  // 4-card summary matching the demo's session view: Total/Complete/Under evaluation/Awaiting
  // interview - pure display aggregation over the already-loaded appointment list, not a
  // duplicated backend rule.
  sessionStats = computed(() => {
    const live = this.store
      .detailAppointments()
      .filter((a) => !!a.invitationId && !INACTIVE_STATUSES.includes(a.status));

    return {
      total: live.length,
      complete: live.filter((a) => a.status === AppointmentStatus.Completed || a.status === AppointmentStatus.Closed).length,
      underEvaluation: live.filter((a) => a.status === AppointmentStatus.InInterview || a.status === AppointmentStatus.UnderEvaluation)
        .length,
      awaiting: live.filter((a) => a.status === AppointmentStatus.Scheduled).length,
    };
  });

  private localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  jobTitle = computed(() => {
    const s = this.store.detailSchedule();
    return s ? this.localized(s.jobTitleNameAr, s.jobTitleNameEn) : '';
  });

  committeeName = computed(() => {
    const s = this.store.detailSchedule();
    return s ? this.localized(s.committeeNameAr, s.committeeNameEn) : '';
  });

  statusLabel(status: ScheduleStatus): string {
    return EVAL_SCHEDULE_STATUS_LABELS[status];
  }

  statusPill(status: ScheduleStatus): string {
    return EVAL_SCHEDULE_STATUS_PILL[status];
  }

  appointmentStatusLabel(status: AppointmentStatus): string {
    return EVAL_APPOINTMENT_STATUS_LABELS[status];
  }

  appointmentStatusPill(status: AppointmentStatus): string {
    return EVAL_APPOINTMENT_STATUS_PILL[status];
  }

  attendanceLabel(appointment: AppointmentModel): string | null {
    return appointment.attendanceStatus ? ATTENDANCE_STATUS_LABELS[appointment.attendanceStatus as AttendanceStatus] : null;
  }

  attendancePill(appointment: AppointmentModel): string {
    return appointment.attendanceStatus ? ATTENDANCE_STATUS_PILL[appointment.attendanceStatus as AttendanceStatus] : 'neutral';
  }

  candidateName(appointment: AppointmentModel): string {
    return this.localized(appointment.candidateFullNameAr, appointment.candidateFullNameEn);
  }

  candidateLabel(appointment: AppointmentModel): string {
    const name = this.candidateName(appointment);
    return appointment.candidateQid ? `${name} (${appointment.candidateQid})` : name;
  }

  location(appointment: AppointmentModel): string {
    if (appointment.roomId) return this.localized(appointment.roomNameAr, appointment.roomNameEn);
    return appointment.remoteMeetingUrl ? EVAL_INTERVIEW_TYPE_LABELS[appointment.interviewType] : '';
  }

  // Evaluation progress badge - "—" while unresolved (403 for a plain evaluator, or still loading).
  evaluationProgress(appointment: AppointmentModel): { submitted: number; total: number } | null {
    const summary = this.store.appointmentSummaries()[appointment.id];
    if (!summary) return null;
    const submitted = summary.members.filter((m) => m.status === MemberEvaluationStatus.Submitted).length;
    return { submitted, total: summary.members.length };
  }

  // Powers the small count badge on the Issues button - "—" until fetched means no badge shown at
  // all rather than a misleading 0 (see loadAppointmentIssues in the facade).
  totalIssuesCount(appointment: AppointmentModel): number {
    return (this.store.appointmentIssues()[appointment.id] ?? []).length;
  }

  openIssuesCount(appointment: AppointmentModel): number {
    return (this.store.appointmentIssues()[appointment.id] ?? []).filter((i) => i.status === OperationalIssueStatus.Open).length;
  }

  canStartOrContinue(appointment: AppointmentModel): boolean {
    return !!appointment.invitationId && appointment.status !== AppointmentStatus.Cancelled && appointment.status !== AppointmentStatus.Rescheduled;
  }

  startOrContinueLabel(appointment: AppointmentModel): string {
    return appointment.status === AppointmentStatus.Scheduled
      ? 'INTERVIEW_EVALUATION.SESSION.START'
      : 'INTERVIEW_EVALUATION.SESSION.CONTINUE';
  }

  startOrContinue(appointment: AppointmentModel) {
    this.service.openCandidate(appointment.id);
  }

  viewDetails(appointment: AppointmentModel) {
    this.dialogService.open(AppointmentDetailsDialogComponent, {
      header: this.translate.instant('INTERVIEW_EVALUATION.SESSION.APPOINTMENT_DETAILS'),
      width: '32rem',
      data: { appointment, candidateName: this.candidateName(appointment), location: this.location(appointment) },
    });
  }

  manageIssues(appointment: AppointmentModel) {
    const ref = this.dialogService.open(OperationalIssuesDialogComponent, {
      header: this.translate.instant('INTERVIEW_EVALUATION.ISSUES.TITLE'),
      width: '40rem',
      data: { appointmentId: appointment.id, candidateLabel: this.candidateLabel(appointment) },
    });
    ref?.onClose.subscribe(() => this.service.refreshAppointmentIssues(appointment.id));
  }
}
