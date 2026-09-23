import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { Select } from 'primeng/select';
import { Checkbox } from 'primeng/checkbox';
import { Textarea } from 'primeng/textarea';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { NotificationService } from '../../../../../../../core/services/notification.service';
import { DialogHelperService } from '../../../../../../../core/services/dialog-helper.service';

import { InterviewEvaluationService } from '../../services/interview-evaluation.service';
import { OperationalIssueModel } from '../../models/operational-issue.model';
import {
  OPERATIONAL_ISSUE_STATUS_LABELS,
  OPERATIONAL_ISSUE_STATUS_PILL,
  OPERATIONAL_ISSUE_TYPE_LABELS,
  OperationalIssueStatus,
  OperationalIssueType,
} from '../../models/enums';

export interface OperationalIssuesDialogData {
  appointmentId: string;
  candidateLabel: string;
}

@Component({
  selector: 'app-operational-issues-dialog',
  templateUrl: './operational-issues-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, TranslatePipe, Select, Checkbox, Textarea, HasPermissionDirective],
})
export class OperationalIssuesDialogComponent implements OnInit {
  readonly Permissions = Permissions;
  readonly OperationalIssueStatus = OperationalIssueStatus;

  // Create/resolve/waive/mark-blocking accept either permission on the backend (see
  // InterviewOperationalIssueController) - a Chair/committee member with only InterviewEvaluation.Manage
  // must see these controls too, not just InterviewSchedule.Manage holders.
  readonly canManage = [Permissions.InterviewSchedule.Manage, Permissions.InterviewEvaluation.Manage];

  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig<OperationalIssuesDialogData>);
  private api = inject(InterviewEvaluationService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private dialogHelper = inject(DialogHelperService);

  readonly typeOptions = [
    OperationalIssueType.NoShow,
    OperationalIssueType.CandidateWithdrawal,
    OperationalIssueType.IncompleteEvaluation,
    OperationalIssueType.TechnicalProblem,
    OperationalIssueType.CouldNotBeConducted,
    OperationalIssueType.RescheduleRequest,
  ].map((type) => ({ value: type, label: OPERATIONAL_ISSUE_TYPE_LABELS[type] }));

  appointmentId = '';
  candidateLabel = '';

  readonly issues = signal<OperationalIssueModel[]>([]);
  readonly loading = signal(false);

  showCreateForm = signal(false);
  newType: OperationalIssueType | null = null;
  newDescription = '';
  newIsBlocking = false;

  ngOnInit(): void {
    const data = this.dialogConfig.data!;
    this.appointmentId = data.appointmentId;
    this.candidateLabel = data.candidateLabel;
    this.load();
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

  load() {
    this.loading.set(true);
    this.api.listOperationalIssues(this.appointmentId).subscribe({
      next: (issues) => {
        this.issues.set(issues);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  createIssue() {
    if (this.newType === null) {
      this.notify.error(this.translate.instant('INTERVIEW_EVALUATION.ISSUES.TYPE_REQUIRED'));
      return;
    }

    this.api
      .createOperationalIssue({
        appointmentId: this.appointmentId,
        issueType: this.newType,
        description: this.newDescription.trim() || null,
        isBlocking: this.newIsBlocking,
      })
      .subscribe({
        next: () => {
          this.notify.success(this.translate.instant('INTERVIEW_EVALUATION.ISSUES.CREATED'));
          this.newType = null;
          this.newDescription = '';
          this.newIsBlocking = false;
          this.showCreateForm.set(false);
          this.load();
        },
      });
  }

  resolve(issue: OperationalIssueModel) {
    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_EVALUATION.ISSUES.CONFIRM_RESOLVE_TITLE',
        description: 'INTERVIEW_EVALUATION.ISSUES.CONFIRM_RESOLVE_DESCRIPTION',
        confirmText: 'INTERVIEW_EVALUATION.ISSUES.RESOLVE',
        cancelText: 'INTERVIEW_EVALUATION.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_EVALUATION.ISSUES.RESOLUTION_NOTES_LABEL',
        inputPlaceholder: 'INTERVIEW_EVALUATION.ISSUES.RESOLUTION_NOTES_PLACEHOLDER',
      })
      ?.onClose.subscribe((notes: string | undefined) => {
        if (notes === undefined) return;
        this.api.resolveOperationalIssue(issue.id, notes.trim() || null).subscribe({
          next: () => {
            this.notify.success(this.translate.instant('INTERVIEW_EVALUATION.ISSUES.RESOLVED'));
            this.load();
          },
        });
      });
  }

  waive(issue: OperationalIssueModel) {
    this.dialogHelper
      .openConfirmDialog({
        type: 'warning',
        title: 'INTERVIEW_EVALUATION.ISSUES.CONFIRM_WAIVE_TITLE',
        description: 'INTERVIEW_EVALUATION.ISSUES.CONFIRM_WAIVE_DESCRIPTION',
        confirmText: 'INTERVIEW_EVALUATION.ISSUES.WAIVE',
        cancelText: 'INTERVIEW_EVALUATION.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_EVALUATION.ISSUES.RESOLUTION_NOTES_LABEL',
        inputPlaceholder: 'INTERVIEW_EVALUATION.ISSUES.RESOLUTION_NOTES_PLACEHOLDER',
      })
      ?.onClose.subscribe((notes: string | undefined) => {
        if (notes === undefined) return;
        this.api.waiveOperationalIssue(issue.id, notes.trim() || null).subscribe({
          next: () => {
            this.notify.success(this.translate.instant('INTERVIEW_EVALUATION.ISSUES.WAIVED'));
            this.load();
          },
        });
      });
  }

  toggleBlocking(issue: OperationalIssueModel) {
    this.api.updateOperationalIssueBlocking(issue.id, !issue.isBlocking).subscribe({ next: () => this.load() });
  }

  close() {
    this.dialogRef.close();
  }
}
