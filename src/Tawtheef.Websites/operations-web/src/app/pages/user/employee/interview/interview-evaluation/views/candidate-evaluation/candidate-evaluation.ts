import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { InputNumber } from 'primeng/inputnumber';
import { Textarea } from 'primeng/textarea';
import { DialogService } from 'primeng/dynamicdialog';

import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { DialogHelperService } from '../../../../../../../core/services/dialog-helper.service';

import { AppointmentModel } from '../../../interview-schedule/models/appointment.model';
import { AppointmentStatus, CommitteeRole } from '../../../interview-schedule/models/enums';

import { InterviewEvaluationStore } from '../../interview-evaluation.store';
import { InterviewEvaluationFacade } from '../../interview-evaluation.facade';
import { SaveMemberEvaluationDraftPayload } from '../../services/interview-evaluation.service';
// Local label maps, not interview-schedule's - this page only loads its own i18nNamespace JSON
// (see EVAL_SCHEDULE_STATUS_LABELS's comment in models/enums.ts).
import {
  ATTENDANCE_STATUS_LABELS,
  ATTENDANCE_STATUS_PILL,
  AttendanceStatus,
  EVAL_APPOINTMENT_STATUS_LABELS,
  EVAL_APPOINTMENT_STATUS_PILL,
  EVAL_INTERVIEW_TYPE_LABELS,
  MEMBER_EVALUATION_STATUS_LABELS,
  MEMBER_EVALUATION_STATUS_PILL,
  MEMBER_ROLE_LABELS,
  MemberEvaluationStatus,
} from '../../models/enums';
import { AttendanceDialogComponent } from '../../components/attendance-dialog/attendance-dialog';
import { OperationalIssuesDialogComponent } from '../../components/operational-issues-dialog/operational-issues-dialog';

@Component({
  selector: 'app-candidate-evaluation',
  templateUrl: './candidate-evaluation.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, FormsModule, TranslatePipe, InputNumber, Textarea, HasPermissionDirective],
})
export class CandidateEvaluationComponent {
  readonly Permissions = Permissions;
  readonly MemberEvaluationStatus = MemberEvaluationStatus;

  store = inject(InterviewEvaluationStore);
  service = inject(InterviewEvaluationFacade);
  private dialogService = inject(DialogService);
  private dialogHelper = inject(DialogHelperService);
  private translate = inject(TranslateService);

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

  // Chair/HR see the whole committee's submission progress; anyone else falls back to their own
  // form status (candidateSummary is null for them - see loadCandidate/getContext in the facade).
  overallProgress = computed(() => {
    const summary = this.store.candidateSummary();
    if (!summary) return null;
    const submitted = summary.members.filter((m) => m.status === MemberEvaluationStatus.Submitted).length;
    return { submitted, total: summary.members.length };
  });

  candidateName(appointment: AppointmentModel): string {
    return this.localized(appointment.candidateFullNameAr, appointment.candidateFullNameEn);
  }

  location(appointment: AppointmentModel): string {
    if (appointment.roomId) return this.localized(appointment.roomNameAr, appointment.roomNameEn);
    return appointment.remoteMeetingUrl ? EVAL_INTERVIEW_TYPE_LABELS[appointment.interviewType] : '';
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

  memberStatusLabel(status: MemberEvaluationStatus): string {
    return MEMBER_EVALUATION_STATUS_LABELS[status];
  }

  memberStatusPill(status: MemberEvaluationStatus): string {
    return MEMBER_EVALUATION_STATUS_PILL[status];
  }

  memberRoleLabel(role: CommitteeRole): string {
    return MEMBER_ROLE_LABELS[role];
  }

  axisName(axisNameAr?: string | null, axisNameEn?: string | null): string {
    return this.localized(axisNameAr, axisNameEn);
  }

  criterionName(nameAr?: string | null, nameEn?: string | null): string {
    return this.localized(nameAr, nameEn);
  }

  criterionDescription(descriptionAr?: string | null, descriptionEn?: string | null): string {
    return this.localized(descriptionAr, descriptionEn);
  }

  // ======== Attendance ========
  openAttendanceDialog() {
    const appointment = this.store.selectedAppointment();
    if (!appointment) return;

    const ref = this.dialogService.open(AttendanceDialogComponent, {
      header: this.translate.instant('INTERVIEW_EVALUATION.ATTENDANCE_DIALOG.TITLE'),
      width: '28rem',
      data: {
        candidateName: this.candidateName(appointment),
        candidateQid: appointment.candidateQid ?? null,
        jobTitle: this.jobTitle(),
        currentStatus: appointment.attendanceStatus,
      },
    });

    ref?.onClose.subscribe((status: AttendanceStatus | undefined) => {
      if (status === undefined) return;
      this.service.registerAttendance(appointment.id, status);
    });
  }

  // ======== Scoring ========
  private buildPayload(): SaveMemberEvaluationDraftPayload | null {
    const form = this.store.candidateForm();
    if (!form) return null;

    const scores = form.axes
      .flatMap((axis) => axis.criteria)
      .filter((criterion) => criterion.score !== null && criterion.score !== undefined)
      .map((criterion) => ({ criterionId: criterion.criterionId, score: criterion.score as number, notes: criterion.notes }));

    return { appointmentId: form.interviewAppointmentId, scores, generalNotes: form.generalNotes };
  }

  saveDraft() {
    const payload = this.buildPayload();
    if (payload) this.service.saveDraft(payload);
  }

  submit() {
    const payload = this.buildPayload();
    if (!payload) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_EVALUATION.CANDIDATE.CONFIRM_SUBMIT_TITLE',
        description: 'INTERVIEW_EVALUATION.CANDIDATE.CONFIRM_SUBMIT_DESCRIPTION',
        confirmText: 'INTERVIEW_EVALUATION.CANDIDATE.SUBMIT',
        cancelText: 'INTERVIEW_EVALUATION.CANCEL',
      })
      ?.onClose.subscribe((confirmed: boolean | undefined) => {
        if (confirmed) this.service.submitEvaluation(payload);
      });
  }

  // ======== Operational issues ========
  manageIssues() {
    const appointment = this.store.selectedAppointment();
    if (!appointment) return;

    const ref = this.dialogService.open(OperationalIssuesDialogComponent, {
      header: this.translate.instant('INTERVIEW_EVALUATION.ISSUES.TITLE'),
      width: '40rem',
      data: { appointmentId: appointment.id, candidateLabel: this.candidateName(appointment) },
    });
    ref?.onClose.subscribe(() => this.service.refreshAppointmentIssues(appointment.id));
  }
}
