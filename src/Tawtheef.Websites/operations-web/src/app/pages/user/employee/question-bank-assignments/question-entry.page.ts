import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { ProgressBar } from 'primeng/progressbar';
import { TableModule } from 'primeng/table';
import { LanguageService } from '../../../../core/services/language.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { routes } from '../../../../routes/routes';
import {
  AssignmentQuestion,
  AssignmentStatuses,
  AssignmentWorkspace,
  QuestionInput,
} from './models/question-bank-assignment.models';
import { QuestionDialogComponent } from './question-dialog/question-dialog.component';
import { QuestionBankAssignmentsService } from './services/question-bank-assignments.service';

@Component({
  selector: 'app-question-entry',
  standalone: true,
  templateUrl: './question-entry.page.html',
  imports: [
    CommonModule,
    TranslatePipe,
    ButtonModule,
    ProgressBar,
    TableModule,
    DynamicDialogModule,
  ],
  providers: [DialogService],
})
export class QuestionEntryPage {
  private readonly assignmentId =
    inject(ActivatedRoute).snapshot.paramMap.get('assignmentId')!;
  private readonly service = inject(QuestionBankAssignmentsService);
  private readonly dialogs = inject(DialogService);
  private readonly notification = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly language = inject(LanguageService);
  private readonly router = inject(Router);

  readonly workspace = signal<AssignmentWorkspace | null>(null);
  readonly statuses = AssignmentStatuses;

  constructor() {
    this.load();
  }

  get editable(): boolean {
    const statusId = this.workspace()?.assignment.statusId;
    return statusId === this.statuses.assigned || statusId === this.statuses.inProgress;
  }

  localized(ar?: string, en?: string): string {
    return (this.language.get() === 'ar' ? ar : en) || '-';
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

  remove(question: AssignmentQuestion): void {
    this.service.remove(this.assignmentId, question.itemId).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('QUESTION_ASSIGNMENTS.REMOVED'));
        this.load();
      },
      error: () => this.notification.error(),
    });
  }

  finish(): void {
    this.service.finish(this.assignmentId).subscribe({
      next: () => {
        this.notification.success(
          this.translate.instant('QUESTION_ASSIGNMENTS.FINISH_SUCCESS'),
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
      data: { question },
    });

    ref.onClose.subscribe((input?: QuestionInput) => {
      if (!input) {
        return;
      }

      const request = question
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
    this.service.workspace(this.assignmentId).subscribe({
      next: (workspace) => this.workspace.set(workspace),
      error: () => {
        this.notification.error();
        this.back();
      },
    });
  }
}
