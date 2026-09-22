import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { Tooltip } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';

import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { DialogHelperService } from '../../../../../../../core/services/dialog-helper.service';

import { InterviewScheduleStore } from '../../interview-schedule.store';
import { InterviewScheduleFacade } from '../../interview-schedule.facade';
import { RescheduleDialogComponent } from '../../components/reschedule-dialog/reschedule-dialog';
import { AppointmentModel } from '../../models/appointment.model';
import {
  APPOINTMENT_STATUS_LABELS,
  APPOINTMENT_STATUS_PILL,
  AppointmentStatus,
  INTERVIEW_TYPE_LABELS,
  SCHEDULE_STATUS_LABELS,
  SCHEDULE_STATUS_PILL,
  ScheduleStatus,
} from '../../models/enums';
import { RescheduleAppointmentPayload } from '../../services/interview-schedule.service';

@Component({
  selector: 'app-schedule-detail',
  templateUrl: './schedule-detail.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, TranslatePipe, TableModule, Tooltip, HasPermissionDirective],
})
export class ScheduleDetailComponent {
  readonly Permissions = Permissions;

  store = inject(InterviewScheduleStore);
  service = inject(InterviewScheduleFacade);
  private dialogHelper = inject(DialogHelperService);
  private dialogService = inject(DialogService);
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

  interviewTypeLabel = computed(() => {
    const s = this.store.detailSchedule();
    return s ? INTERVIEW_TYPE_LABELS[s.defaultInterviewType] : '';
  });

  statusLabel(status: ScheduleStatus): string {
    return SCHEDULE_STATUS_LABELS[status];
  }

  statusPill(status: ScheduleStatus): string {
    return SCHEDULE_STATUS_PILL[status];
  }

  appointmentStatusLabel(status: AppointmentStatus): string {
    return APPOINTMENT_STATUS_LABELS[status];
  }

  appointmentStatusPill(status: AppointmentStatus): string {
    return APPOINTMENT_STATUS_PILL[status];
  }

  candidateName(appointment: AppointmentModel): string {
    return this.localized(appointment.candidateFullNameAr, appointment.candidateFullNameEn);
  }

  // Name plus personal ID, for the places that show the candidate as a single string (the reschedule dialog).
  candidateLabel(appointment: AppointmentModel): string {
    const name = this.candidateName(appointment);
    return appointment.candidateQid ? `${name} (${appointment.candidateQid})` : name;
  }

  location(appointment: AppointmentModel): string {
    if (appointment.roomId) return this.localized(appointment.roomNameAr, appointment.roomNameEn);
    return appointment.remoteMeetingUrl ?? '';
  }

  // Which lifecycle actions apply mirrors the backend's state machine (see InterviewSchedule entity /
  // SubmitScheduleCommand / ApproveScheduleCommand / ReturnScheduleCommand / CancelScheduleCommand).
  canEdit(status: ScheduleStatus): boolean {
    return status === ScheduleStatus.Proposed || status === ScheduleStatus.Returned;
  }

  canSubmit(status: ScheduleStatus): boolean {
    return status === ScheduleStatus.Proposed;
  }

  canDecide(status: ScheduleStatus): boolean {
    return status === ScheduleStatus.PendingApproval;
  }

  canCancel(status: ScheduleStatus): boolean {
    return status === ScheduleStatus.Proposed || status === ScheduleStatus.Approved;
  }

  canSendReminder(scheduleStatus: ScheduleStatus): boolean {
    return scheduleStatus === ScheduleStatus.Approved || scheduleStatus === ScheduleStatus.Closed;
  }

  canReschedule(scheduleStatus: ScheduleStatus): boolean {
    return scheduleStatus === ScheduleStatus.Approved || scheduleStatus === ScheduleStatus.Returned;
  }

  edit() {
    const schedule = this.store.detailSchedule();
    if (schedule) this.service.openEditWizard(schedule);
  }

  submit() {
    const schedule = this.store.detailSchedule();
    if (!schedule) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_SUBMIT_TITLE',
        description: 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_SUBMIT_DESCRIPTION',
        confirmText: 'INTERVIEW_SCHEDULE.DETAIL.SUBMIT',
        cancelText: 'INTERVIEW_SCHEDULE.WIZARD.CANCEL',
      })
      ?.onClose.subscribe((confirmed: boolean | undefined) => {
        if (confirmed) this.service.submitSchedule(schedule.id);
      });
  }

  approve() {
    const schedule = this.store.detailSchedule();
    if (!schedule) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_APPROVE_TITLE',
        description: 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_APPROVE_DESCRIPTION',
        confirmText: 'INTERVIEW_SCHEDULE.DETAIL.APPROVE',
        cancelText: 'INTERVIEW_SCHEDULE.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_SCHEDULE.DETAIL.DECISION_NOTES_LABEL',
        inputPlaceholder: 'INTERVIEW_SCHEDULE.DETAIL.DECISION_NOTES_PLACEHOLDER',
      })
      ?.onClose.subscribe((notes: string | undefined) => {
        if (notes === undefined) return;
        this.service.approveSchedule(schedule.id, notes?.trim() || null);
      });
  }

  returnForEdit() {
    const schedule = this.store.detailSchedule();
    if (!schedule) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'warning',
        title: 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_RETURN_TITLE',
        description: 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_RETURN_DESCRIPTION',
        confirmText: 'INTERVIEW_SCHEDULE.DETAIL.RETURN',
        cancelText: 'INTERVIEW_SCHEDULE.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_SCHEDULE.DETAIL.REASON_LABEL',
        inputPlaceholder: 'INTERVIEW_SCHEDULE.DETAIL.REASON_PLACEHOLDER',
      })
      ?.onClose.subscribe((reason: string | undefined) => {
        if (!reason?.trim()) return;
        this.service.returnSchedule(schedule.id, reason.trim());
      });
  }

  cancelSchedule() {
    const schedule = this.store.detailSchedule();
    if (!schedule) return;

    this.dialogHelper
      .openConfirmDialog({
        type: 'delete',
        title: 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_CANCEL_TITLE',
        description: 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_CANCEL_DESCRIPTION',
        confirmText: 'INTERVIEW_SCHEDULE.DETAIL.CANCEL_SCHEDULE',
        cancelText: 'INTERVIEW_SCHEDULE.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_SCHEDULE.DETAIL.REASON_LABEL',
        inputPlaceholder: 'INTERVIEW_SCHEDULE.DETAIL.REASON_PLACEHOLDER',
      })
      ?.onClose.subscribe((reason: string | undefined) => {
        if (!reason?.trim()) return;
        this.service.cancelSchedule(schedule.id, reason.trim());
      });
  }

  sendReminder(appointment: AppointmentModel) {
    const alreadySent = !!appointment.invitationSentAt;

    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: alreadySent ? 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_REMINDER_TITLE' : 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_NOTIFY_TITLE',
        description: alreadySent
          ? 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_REMINDER_DESCRIPTION'
          : 'INTERVIEW_SCHEDULE.DETAIL.CONFIRM_NOTIFY_DESCRIPTION',
        confirmText: alreadySent ? 'INTERVIEW_SCHEDULE.DETAIL.SEND_REMINDER' : 'INTERVIEW_SCHEDULE.DETAIL.SEND_NOTIFICATION',
        cancelText: 'INTERVIEW_SCHEDULE.WIZARD.CANCEL',
      })
      ?.onClose.subscribe((confirmed: boolean | undefined) => {
        if (confirmed) this.service.sendReminder(appointment.id, alreadySent);
      });
  }

  reschedule(appointment: AppointmentModel) {
    const ref = this.dialogService.open(RescheduleDialogComponent, {
      header: this.translate.instant('INTERVIEW_SCHEDULE.DETAIL.RESCHEDULE'),
      width: '32rem',
      data: {
        appointment,
        candidateName: this.candidateLabel(appointment),
        searchRooms: this.service.searchRooms,
      },
    });

    ref?.onClose.subscribe((payload: RescheduleAppointmentPayload | undefined) => {
      if (payload) this.service.rescheduleAppointment(payload);
    });
  }
}
