import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ProgressBar } from 'primeng/progressbar';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { RichContentRendererComponent } from '../../../../shared/rich-content/rich-content-renderer.component';
import { finalize, Observable } from 'rxjs';
import { LanguageService } from '../../../../core/services/language.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { routes } from '../../../../routes/routes';
import {
  AssignmentQuestion,
  AssignmentStatuses,
  RequestItemStatuses,
  AssignmentWorkspace,
  MaintenanceBaseQuestion,
  QuestionInput,
  QuestionChangeTypes,
} from './models/question-bank-assignment.models';
import { QuestionDialogComponent } from './question-dialog/question-dialog.component';
import { QuestionBankAssignmentsService } from './services/question-bank-assignments.service';
import {
  questionBankAssignmentStatusSeverity,
  questionBankRequestItemStatusSeverity,
} from '../../../../shared/utils/question-bank-status.util';

@Component({
  selector: 'app-question-entry',
  standalone: true,
  templateUrl: './question-entry.page.html',
  styleUrl: './question-entry.page.scss',
  imports: [
    CommonModule,
    TranslatePipe,
    ButtonModule,
    ProgressBar,
    TableModule,
    TagModule,
    TooltipModule,
    RichContentRendererComponent,
    DynamicDialogModule,
    ConfirmDialogModule,
  ],
  providers: [DialogService, ConfirmationService],
})
export class QuestionEntryPage {
  private readonly assignmentId = inject(ActivatedRoute).snapshot.paramMap.get('assignmentId')!;
  private readonly service = inject(QuestionBankAssignmentsService);
  private readonly dialogs = inject(DialogService);
  private readonly notification = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly language = inject(LanguageService);
  private readonly router = inject(Router);
  private readonly confirmation = inject(ConfirmationService);

  readonly workspace = signal<AssignmentWorkspace | null>(null);
  readonly loading = signal(true);
  readonly statuses = AssignmentStatuses;
  readonly itemStatuses = RequestItemStatuses;
  readonly changeTypes = QuestionChangeTypes;
  readonly assignmentStatusSeverity = questionBankAssignmentStatusSeverity;
  readonly itemStatusSeverity = questionBankRequestItemStatusSeverity;

  constructor() {
    this.load();
  }

  get editable(): boolean {
    const statusId = this.workspace()?.assignment.statusId;
    return statusId === this.statuses.assigned || statusId === this.statuses.inProgress;
  }

  get correctionMode(): boolean {
    return this.workspace()?.assignment.statusId === this.statuses.returnedForModification;
  }

  canEdit(question: AssignmentQuestion): boolean {
    return (
      (this.editable && question.statusId === this.itemStatuses.draft) ||
      (this.correctionMode &&
        (question.statusId === this.itemStatuses.needsModification ||
          question.statusId === this.itemStatuses.draft))
    );
  }

  canRemove(question: AssignmentQuestion): boolean {
    return (
      (this.editable && question.statusId === this.itemStatuses.draft) ||
      (this.correctionMode &&
        (question.statusId === this.itemStatuses.rejected ||
          question.statusId === this.itemStatuses.needsModification ||
          question.statusId === this.itemStatuses.draft))
    );
  }

  canProposeDeletion(question: AssignmentQuestion): boolean {
    if (!this.workspace()?.baseQuestions.length || question.changeTypeId === this.changeTypes.add)
      return false;
    if (question.changeTypeId === this.changeTypes.delete)
      return this.correctionMode && question.statusId === this.itemStatuses.needsModification;
    return this.canEdit(question);
  }

  localized(ar?: string, en?: string): string {
    return (this.language.get() === 'ar' ? ar : en) || '-';
  }

  localizedContent(ar?: string, en?: string): string {
    return (this.language.get() === 'ar' ? ar : en) || '';
  }

  changeTypeLabel(id: string): string {
    if (id === this.changeTypes.update) return 'QUESTION_ASSIGNMENTS.CHANGE_UPDATE';
    if (id === this.changeTypes.delete) return 'QUESTION_ASSIGNMENTS.CHANGE_DELETE';
    return 'QUESTION_ASSIGNMENTS.CHANGE_ADD';
  }

  percent(): number {
    const progress = this.workspace()?.progress;
    if (!progress) {
      return 0;
    }

    return Math.min(
      100,
      Math.round(
        (100 * progress.currentQuestionCount) / Math.max(1, progress.minimumQuestionCount),
      ),
    );
  }

  add(): void {
    this.openDialog();
  }

  edit(question: AssignmentQuestion): void {
    this.openDialog(question);
  }

  editBase(question: MaintenanceBaseQuestion): void {
    const ref = this.dialogs.open(QuestionDialogComponent, {
      header: this.translate.instant('QUESTION_ASSIGNMENTS.EDIT'),
      width: 'min(800px, 95vw)',
      data: { question, questionTypeReadOnly: true },
    });
    ref?.onClose.subscribe((input?: QuestionInput) => {
      if (!input) return;
      this.service.updateBaseQuestion(this.assignmentId, question.questionId, input).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('QUESTION_ASSIGNMENTS.SAVED'));
          this.load();
        },
        error: (error: HttpErrorResponse) => this.handleMaintenanceError(error),
      });
    });
  }

  deleteBase(question: MaintenanceBaseQuestion): void {
    this.confirmation.confirm({
      message: this.translate.instant('QUESTION_ASSIGNMENTS.DELETE_BASE_CONFIRM'),
      accept: () => this.service.deleteBaseQuestion(this.assignmentId, question.questionId).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('QUESTION_ASSIGNMENTS.SAVED'));
          this.load();
        },
        error: (error: HttpErrorResponse) => this.handleMaintenanceError(error),
      }),
    });
  }

  proposeDeletion(question: AssignmentQuestion): void {
    this.confirmation.confirm({
      message: this.translate.instant('QUESTION_ASSIGNMENTS.DELETE_BASE_CONFIRM'),
      accept: () => this.service.deleteBaseQuestion(this.assignmentId, question.questionId).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('QUESTION_ASSIGNMENTS.SAVED'));
          this.load();
        },
        error: (error: HttpErrorResponse) => this.handleMaintenanceError(error),
      }),
    });
  }

  remove(question: AssignmentQuestion): void {
    if (this.correctionMode && question.statusId === this.itemStatuses.rejected) {
      this.confirmation.confirm({
        message: this.translate.instant('QUESTION_ASSIGNMENTS.REMOVE_REJECTED_CONFIRM'),
        accept: () => this.performRemove(question),
      });
      return;
    }
    this.performRemove(question);
  }

  private performRemove(question: AssignmentQuestion): void {
    this.service.remove(this.assignmentId, question.itemId).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('QUESTION_ASSIGNMENTS.REMOVED'));
        this.load();
      },
      error: () => this.notification.error(),
    });
  }

  finish(): void {
    const request = this.correctionMode
      ? this.service.finishModifications(this.assignmentId)
      : this.service.finish(this.assignmentId);
    request.subscribe({
      next: () => {
        this.notification.success(
          this.translate.instant(
            this.correctionMode
              ? 'QUESTION_ASSIGNMENTS.MODIFICATIONS_SUCCESS'
              : 'QUESTION_ASSIGNMENTS.FINISH_SUCCESS',
          ),
        );
        this.load();
      },
      error: () =>
        this.notification.error(this.translate.instant('QUESTION_ASSIGNMENTS.FINISH_ERROR')),
    });
  }

  back(): void {
    this.router.navigateByUrl(routes.portal.questionBankAssignments);
  }

  private openDialog(question?: AssignmentQuestion): void {
    const ref = this.dialogs.open(QuestionDialogComponent, {
      header: this.translate.instant(
        question ? 'QUESTION_ASSIGNMENTS.EDIT' : 'QUESTION_ASSIGNMENTS.ADD',
      ),
      width: 'min(800px, 95vw)',
      data: {
        question,
        questionTypeReadOnly:
          !!question && !!this.workspace()?.baseQuestions.length &&
          question.changeTypeId !== this.changeTypes.add,
      },
    });

    if (!ref) {
      return;
    }

    ref.onClose.subscribe((input?: QuestionInput) => {
      if (!input) {
        return;
      }

      const request: Observable<unknown> = question
        ? this.service.edit(this.assignmentId, question.itemId, input)
        : this.service.add(this.assignmentId, input);

      request.subscribe({
        next: () => {
          this.notification.success(this.translate.instant('QUESTION_ASSIGNMENTS.SAVED'));
          this.load();
        },
        error: () => this.notification.error(),
      });
    });
  }

  private load(): void {
    this.loading.set(true);
    this.service
      .workspace(this.assignmentId)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (workspace) => this.workspace.set(workspace),
        error: () => {
          this.notification.error();
          this.back();
        },
      });
  }

  private handleMaintenanceError(error: HttpErrorResponse): void {
    // The HTTP interceptor owns the toast, including the localized claim error.
    // Refresh ownership only for the backend's specific duplicate-claim code.
    const claimCode = 'QUESTION_BANK_QUESTION_ALREADY_ASSIGNED_FOR_MAINTENANCE';
    let body = error.error;
    if (typeof body === 'string') {
      try { body = JSON.parse(body); } catch { return; }
    }
    if (body?.detail === claimCode || body?.message === claimCode ||
      (Array.isArray(body?.error) && body.error.some((item: { message?: string; reasons?: string[] }) =>
        item.message === claimCode || item.reasons?.includes(claimCode)))) {
      this.load();
    }
  }
}
