import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { AppointmentModel } from '../../../interview-schedule/models/appointment.model';
import { AppointmentStatus } from '../../../interview-schedule/models/enums';
// Local label maps, not interview-schedule's - this page only loads its own i18nNamespace JSON
// (see EVAL_SCHEDULE_STATUS_LABELS's comment in models/enums.ts).
import {
  ATTENDANCE_STATUS_LABELS,
  ATTENDANCE_STATUS_PILL,
  AttendanceStatus,
  EVAL_APPOINTMENT_STATUS_LABELS,
  EVAL_APPOINTMENT_STATUS_PILL,
  EVAL_INTERVIEW_TYPE_LABELS,
} from '../../models/enums';

export interface AppointmentDetailsDialogData {
  appointment: AppointmentModel;
  candidateName: string;
  location: string;
}

@Component({
  selector: 'app-appointment-details-dialog',
  templateUrl: './appointment-details-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, TranslatePipe],
})
export class AppointmentDetailsDialogComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig<AppointmentDetailsDialogData>);

  data!: AppointmentDetailsDialogData;

  ngOnInit(): void {
    this.data = this.dialogConfig.data!;
  }

  statusLabel(status: AppointmentStatus): string {
    return EVAL_APPOINTMENT_STATUS_LABELS[status];
  }

  statusPill(status: AppointmentStatus): string {
    return EVAL_APPOINTMENT_STATUS_PILL[status];
  }

  interviewTypeLabel(): string {
    return EVAL_INTERVIEW_TYPE_LABELS[this.data.appointment.interviewType];
  }

  attendanceLabel(): string | null {
    const status = this.data.appointment.attendanceStatus;
    return status ? ATTENDANCE_STATUS_LABELS[status as AttendanceStatus] : null;
  }

  attendancePill(): string {
    const status = this.data.appointment.attendanceStatus;
    return status ? ATTENDANCE_STATUS_PILL[status as AttendanceStatus] : 'neutral';
  }

  close() {
    this.dialogRef.close();
  }
}
