import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { Select } from 'primeng/select';
import { Tooltip } from 'primeng/tooltip';

import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';

import { InterviewType, ScheduleStatus } from '../../../interview-schedule/models/enums';

import { InterviewEvaluationStore, ELIGIBLE_SESSION_STATUSES } from '../../interview-evaluation.store';
import { InterviewEvaluationFacade } from '../../interview-evaluation.facade';
import { SessionListRowModel } from '../../models/session-row.model';
// Local label maps, not interview-schedule's - see EVAL_SCHEDULE_STATUS_LABELS's own comment: this
// page only loads its own i18nNamespace JSON, so INTERVIEW_SCHEDULE.* keys render untranslated here.
import {
  EVAL_INTERVIEW_TYPE_LABELS,
  EVAL_SCHEDULE_STATUS_LABELS,
  EVAL_SCHEDULE_STATUS_PILL,
} from '../../models/enums';

@Component({
  selector: 'app-sessions-list',
  templateUrl: './sessions-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, FormsModule, TranslatePipe, TableModule, ButtonModule, Select, Tooltip, PaginationComponent],
})
export class SessionsListComponent {
  store = inject(InterviewEvaluationStore);
  service = inject(InterviewEvaluationFacade);

  // Only statuses the "Start Interview" stage cares about - Closed included so a finished session's
  // final progress stays reachable via the filter, even though it isn't shown by default.
  readonly statusOptions = [...ELIGIBLE_SESSION_STATUSES, ScheduleStatus.Closed].map((status) => ({
    value: status,
    label: EVAL_SCHEDULE_STATUS_LABELS[status],
  }));

  readonly typeOptions = [InterviewType.InPerson, InterviewType.Remote].map((type) => ({
    value: type,
    label: EVAL_INTERVIEW_TYPE_LABELS[type],
  }));

  private localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  timeOf(value?: string | null): Date | null {
    if (!value) return null;
    const [hours, minutes] = value.split(':').map(Number);
    return new Date(2000, 0, 1, hours, minutes);
  }

  jobTitle(row: SessionListRowModel): string {
    return this.localized(row.jobTitleNameAr, row.jobTitleNameEn);
  }

  committeeName(row: SessionListRowModel): string {
    return this.localized(row.committeeNameAr, row.committeeNameEn);
  }

  interviewTypeLabel(row: SessionListRowModel): string {
    return EVAL_INTERVIEW_TYPE_LABELS[row.defaultInterviewType];
  }

  statusLabel(status: ScheduleStatus): string {
    return EVAL_SCHEDULE_STATUS_LABELS[status];
  }

  statusPill(status: ScheduleStatus): string {
    return EVAL_SCHEDULE_STATUS_PILL[status];
  }

  location(row: SessionListRowModel): string {
    if (row.roomNames.length === 0) return '';
    if (row.roomNames.length === 1) return row.roomNames[0];
    return `${row.roomNames.length}`;
  }

  progressPercent(row: SessionListRowModel): number {
    if (row.totalAssignedAppointmentsCount === 0) return 0;
    return Math.round((row.completedAppointmentsCount / row.totalAssignedAppointmentsCount) * 100);
  }
}
