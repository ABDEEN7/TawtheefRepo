import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { Select } from 'primeng/select';
import { Tooltip } from 'primeng/tooltip';

import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { InterviewScheduleStore } from '../../interview-schedule.store';
import { InterviewScheduleFacade } from '../../interview-schedule.facade';
import { ScheduleListItemModel } from '../../models/schedule.model';
import {
  INTERVIEW_TYPE_LABELS,
  InterviewType,
  SCHEDULE_STATUS_LABELS,
  SCHEDULE_STATUS_PILL,
  ScheduleStatus,
} from '../../models/enums';

@Component({
  selector: 'app-schedules-list',
  templateUrl: './schedules-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    Select,
    Tooltip,
    HasPermissionDirective,
    PaginationComponent,
  ],
})
export class SchedulesListComponent {
  readonly Permissions = Permissions;
  store = inject(InterviewScheduleStore);
  service = inject(InterviewScheduleFacade);

  readonly statusOptions = [
    ScheduleStatus.Proposed,
    ScheduleStatus.PendingApproval,
    ScheduleStatus.Returned,
    ScheduleStatus.Approved,
    ScheduleStatus.Closed,
    ScheduleStatus.Cancelled,
  ].map((status) => ({ value: status, label: SCHEDULE_STATUS_LABELS[status] }));

  readonly typeOptions = [InterviewType.InPerson, InterviewType.Remote].map((type) => ({
    value: type,
    label: INTERVIEW_TYPE_LABELS[type],
  }));

  private localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  // The API sends a bare time of day ("HH:mm:ss"); the date pipe needs a Date, so build one on an arbitrary day.
  timeOf(value?: string | null): Date | null {
    if (!value) return null;
    const [hours, minutes] = value.split(':').map(Number);
    return new Date(2000, 0, 1, hours, minutes);
  }

  jobTitle(schedule: ScheduleListItemModel): string {
    return this.localized(schedule.jobTitleNameAr, schedule.jobTitleNameEn);
  }

  committeeName(schedule: ScheduleListItemModel): string {
    return this.localized(schedule.committeeNameAr, schedule.committeeNameEn);
  }

  interviewTypeLabel(schedule: ScheduleListItemModel): string {
    return INTERVIEW_TYPE_LABELS[schedule.defaultInterviewType];
  }

  statusLabel(status: ScheduleStatus): string {
    return SCHEDULE_STATUS_LABELS[status];
  }

  statusPill(status: ScheduleStatus): string {
    return SCHEDULE_STATUS_PILL[status];
  }
}
