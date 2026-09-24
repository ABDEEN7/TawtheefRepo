import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { Select } from 'primeng/select';
import { Tooltip } from 'primeng/tooltip';

import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';

import { InterviewResultReportStore } from '../../interview-result-report.store';
import { InterviewResultReportFacade } from '../../interview-result-report.facade';
import { ResultReportListItemModel } from '../../models/result-report.model';
import {
  DISCARDED_REPORT_FILTER,
  RESULT_REPORT_STATUS_LABELS,
  RESULT_REPORT_STATUS_PILL,
  ResultReportListStatusFilter,
  ResultReportStatus,
} from '../../models/enums';

@Component({
  selector: 'app-reports-list',
  templateUrl: './reports-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, DecimalPipe, FormsModule, TranslatePipe, TableModule, ButtonModule, Select, Tooltip, PaginationComponent],
})
export class ReportsListComponent {
  readonly ResultReportStatus = ResultReportStatus;

  store = inject(InterviewResultReportStore);
  service = inject(InterviewResultReportFacade);

  // Creating is transient and never persisted (InterviewResultReport.Create/MarkReadyForReview), so
  // it is never offered as a filter.
  readonly statusOptions: { value: ResultReportListStatusFilter; label: string }[] = [
    ...[
      ResultReportStatus.UnderReview,
      ResultReportStatus.Returned,
      ResultReportStatus.Approved,
      ResultReportStatus.Closed,
    ].map((status) => ({ value: status, label: RESULT_REPORT_STATUS_LABELS[status] })),
    { value: DISCARDED_REPORT_FILTER, label: 'INTERVIEW_RESULT_REPORT.STATUS.DISCARDED' },
  ];

  jobName(row: ResultReportListItemModel): string {
    return this.service.localized(row.jobNameAr, row.jobNameEn);
  }

  scheduleTitle(row: ResultReportListItemModel): string {
    return this.service.localized(row.scheduleTitleAr, row.scheduleTitleEn);
  }

  statusLabel(status: ResultReportStatus): string {
    return RESULT_REPORT_STATUS_LABELS[status];
  }

  statusPill(status: ResultReportStatus): string {
    return RESULT_REPORT_STATUS_PILL[status];
  }

  isReviewable(row: ResultReportListItemModel): boolean {
    return !row.isDiscarded && row.status === ResultReportStatus.UnderReview;
  }

  actionLabel(row: ResultReportListItemModel): string {
    if (row.isDiscarded) return 'INTERVIEW_RESULT_REPORT.LIST.DISCARDED_NO_OPEN';
    return this.isReviewable(row)
      ? 'INTERVIEW_RESULT_REPORT.LIST.REVIEW_AND_APPROVE'
      : 'INTERVIEW_RESULT_REPORT.LIST.VIEW_RESULTS';
  }
}
