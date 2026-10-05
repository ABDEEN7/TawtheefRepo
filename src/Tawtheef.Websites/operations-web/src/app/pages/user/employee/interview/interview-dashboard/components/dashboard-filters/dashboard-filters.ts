import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';

import { LazySelectComponent } from '../../../interview-committees/components/lazy-select/lazy-select';
import { INTERVIEW_TYPE_LABELS, InterviewType, SCHEDULE_STATUS_LABELS, ScheduleStatus } from '../../../interview-schedule/models/enums';
import {
  FINAL_DECISION_LABELS,
  FinalDecision,
  RESULT_REPORT_STATUS_LABELS,
  ResultReportStatus,
} from '../../../interview-result-report/models/enums';
import { InterviewDashboardFacade } from '../../interview-dashboard.facade';
import { InterviewDashboardStore } from '../../interview-dashboard.store';
import { DatePreset } from '../../models/interview-dashboard.model';

interface EnumOption {
  label: string;
  value: number;
}

const enumOptions = <T extends number>(values: T[], labels: Record<T, string>): EnumOption[] =>
  values.map((value) => ({ value, label: labels[value] }));

@Component({
  selector: 'app-dashboard-filters',
  templateUrl: './dashboard-filters.html',
  styleUrl: './dashboard-filters.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [NgTemplateOutlet, FormsModule, TranslatePipe, SelectModule, DatePickerModule, LazySelectComponent],
})
export class DashboardFiltersComponent {
  readonly store = inject(InterviewDashboardStore);
  readonly facade = inject(InterviewDashboardFacade);

  readonly expanded = signal(true);

  readonly presets: { value: DatePreset; labelKey: string }[] = [
    { value: 'all', labelKey: 'INTERVIEW_DASHBOARD.FILTERS.PRESET.ALL' },
    { value: 'today', labelKey: 'INTERVIEW_DASHBOARD.FILTERS.PRESET.TODAY' },
    { value: 'week', labelKey: 'INTERVIEW_DASHBOARD.FILTERS.PRESET.WEEK' },
    { value: 'month', labelKey: 'INTERVIEW_DASHBOARD.FILTERS.PRESET.MONTH' },
    { value: 'custom', labelKey: 'INTERVIEW_DASHBOARD.FILTERS.PRESET.CUSTOM' },
  ];

  // Proposed shares Draft's label in the Schedule module; offering both would show "Draft" twice.
  readonly scheduleStatusOptions = enumOptions(
    [
      ScheduleStatus.Draft,
      ScheduleStatus.PendingApproval,
      ScheduleStatus.Returned,
      ScheduleStatus.Approved,
      ScheduleStatus.ReadyForExecution,
      ScheduleStatus.InProgress,
      ScheduleStatus.Closed,
      ScheduleStatus.Cancelled,
    ],
    SCHEDULE_STATUS_LABELS,
  );
  readonly interviewTypeOptions = enumOptions([InterviewType.InPerson, InterviewType.Remote], INTERVIEW_TYPE_LABELS);
  readonly reportStatusOptions = enumOptions(
    [
      ResultReportStatus.Creating,
      ResultReportStatus.UnderReview,
      ResultReportStatus.Returned,
      ResultReportStatus.Approved,
      ResultReportStatus.Closed,
    ],
    RESULT_REPORT_STATUS_LABELS,
  );
  readonly decisionOptions = enumOptions(
    [
      FinalDecision.CandidateForHiringProcess,
      FinalDecision.WaitingList,
      FinalDecision.Rejected,
      FinalDecision.NeedsAction,
      FinalDecision.NoShow,
    ],
    FINAL_DECISION_LABELS,
  );

  // The picker's model is local while a range is half-picked; the URL is only updated once it's complete.
  readonly pickerRange = signal<Date[] | null>(null);
  readonly customRange = computed<Date[] | null>(() => {
    const picked = this.pickerRange();
    if (picked) return picked;
    const { fromDate, toDate } = this.store.filters();
    return fromDate ? [parseDate(fromDate), parseDate(toDate ?? fromDate)] : null;
  });

  onRangeChange(value: Date[] | null): void {
    this.pickerRange.set(value);
    if (value?.[0] && value?.[1]) {
      this.facade.setCustomRange(value[0], value[1]);
      this.pickerRange.set(null);
    }
  }

  onRangeClose(): void {
    // A single picked day means a one-day range.
    const picked = this.pickerRange();
    if (picked?.[0] && !picked[1]) this.facade.setCustomRange(picked[0], picked[0]);
    this.pickerRange.set(null);
  }
}

function parseDate(value: string): Date {
  const [y, m, d] = value.split('-').map(Number);
  return new Date(y, m - 1, d);
}
