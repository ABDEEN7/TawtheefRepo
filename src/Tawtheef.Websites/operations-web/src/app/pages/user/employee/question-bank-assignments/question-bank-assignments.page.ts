import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { LanguageService } from '../../../../core/services/language.service';
import { routes } from '../../../../routes/routes';
import {
  AssignmentStatuses,
  MyAssignment,
} from './models/question-bank-assignment.models';
import { QuestionBankAssignmentsService } from './services/question-bank-assignments.service';

@Component({
  selector: 'app-question-bank-assignments',
  standalone: true,
  templateUrl: './question-bank-assignments.page.html',
  imports: [CommonModule, TranslatePipe, ButtonModule, TableModule, TagModule],
})
export class QuestionBankAssignmentsPage {
  private readonly service = inject(QuestionBankAssignmentsService);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);

  readonly assignments = signal<MyAssignment[]>([]);
  readonly loading = signal(true);
  readonly statuses = AssignmentStatuses;

  constructor() {
    this.load();
  }

  localized(ar?: string, en?: string): string {
    return (this.language.get() === 'ar' ? ar : en) || '-';
  }

  open(assignment: MyAssignment): void {
    this.router.navigateByUrl(
      routes.portal.questionBankAssignmentQuestions(assignment.assignmentId),
    );
  }

  action(assignment: MyAssignment): string {
    if (assignment.statusId === this.statuses.assigned) {
      return 'QUESTION_ASSIGNMENTS.START';
    }

    if (assignment.statusId === this.statuses.inProgress) {
      return 'QUESTION_ASSIGNMENTS.CONTINUE';
    }

    return 'QUESTION_ASSIGNMENTS.VIEW';
  }

  private load(): void {
    this.service.list(1, 50).subscribe({
      next: (response) => {
        this.assignments.set(response.items ?? []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
