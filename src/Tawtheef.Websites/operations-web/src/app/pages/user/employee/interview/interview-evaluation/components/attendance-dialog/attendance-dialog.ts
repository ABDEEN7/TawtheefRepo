import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { Select } from 'primeng/select';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../../../../core/services/notification.service';
import { ATTENDANCE_STATUS_LABELS, AttendanceStatus } from '../../models/enums';

export interface AttendanceDialogData {
  candidateName: string;
  candidateQid: string | null;
  jobTitle: string;
  currentStatus: number | null | undefined;
}

@Component({
  selector: 'app-attendance-dialog',
  templateUrl: './attendance-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, TranslatePipe, Select],
})
export class AttendanceDialogComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig<AttendanceDialogData>);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);

  readonly statusOptions = [AttendanceStatus.Present, AttendanceStatus.NoShow, AttendanceStatus.Withdrew, AttendanceStatus.Late].map(
    (status) => ({ value: status, label: ATTENDANCE_STATUS_LABELS[status] }),
  );

  candidateName = '';
  candidateQid: string | null = null;
  jobTitle = '';
  status: AttendanceStatus | null = null;

  ngOnInit(): void {
    const data = this.dialogConfig.data!;
    this.candidateName = data.candidateName;
    this.candidateQid = data.candidateQid;
    this.jobTitle = data.jobTitle;
    this.status = (data.currentStatus as AttendanceStatus | null | undefined) ?? null;
  }

  confirm() {
    if (this.status === null) {
      this.notify.error(this.translate.instant('INTERVIEW_EVALUATION.ATTENDANCE_DIALOG.STATUS_REQUIRED'));
      return;
    }
    this.dialogRef.close(this.status);
  }

  cancel() {
    this.dialogRef.close();
  }
}
