import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

import { DynamicDialogConfig } from 'primeng/dynamicdialog';

import { OperationalIssueModel } from '../../../interview-evaluation/models/operational-issue.model';
// Reused from interview-evaluation, not copied - the page loads that module's i18nNamespace too
// (see interview-result-report.page.html).
import {
  OPERATIONAL_ISSUE_STATUS_LABELS,
  OPERATIONAL_ISSUE_STATUS_PILL,
  OPERATIONAL_ISSUE_TYPE_LABELS,
  OperationalIssueStatus,
  OperationalIssueType,
} from '../../../interview-evaluation/models/enums';

export interface OperationalNotesDialogData {
  candidateLabel: string;
  issues: OperationalIssueModel[];
}

// Read-only on purpose: the approval screen is a review screen, and the issues already arrive on
// ResultCandidateDto.OperationalIssues - no second call, and no InterviewSchedule/Evaluation
// permission needed (the manage dialog in interview-evaluation requires those).
@Component({
  selector: 'app-operational-notes-dialog',
  templateUrl: './operational-notes-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, TranslatePipe],
})
export class OperationalNotesDialogComponent implements OnInit {
  private dialogConfig = inject(DynamicDialogConfig<OperationalNotesDialogData>);

  data!: OperationalNotesDialogData;

  ngOnInit(): void {
    this.data = this.dialogConfig.data!;
  }

  typeLabel(type: OperationalIssueType): string {
    return OPERATIONAL_ISSUE_TYPE_LABELS[type];
  }

  statusLabel(status: OperationalIssueStatus): string {
    return OPERATIONAL_ISSUE_STATUS_LABELS[status];
  }

  statusPill(status: OperationalIssueStatus): string {
    return OPERATIONAL_ISSUE_STATUS_PILL[status];
  }
}
