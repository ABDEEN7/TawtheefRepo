import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { SortEvent } from 'primeng/api';
import { LanguageService } from '../../../../core/services/language.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { routes } from '../../../../routes/routes';
import { AssignmentStatuses, MyAssignment } from './models/question-bank-assignment.models';
import { QuestionBankAssignmentsService } from './services/question-bank-assignments.service';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { PaginatedRequest } from '../../../../core/models/paginated-request.model';
import { questionBankAssignmentStatusSeverity } from '../../../../shared/utils/question-bank-status.util';

@Component({
  selector: 'app-question-bank-assignments',
  standalone: true,
  templateUrl: './question-bank-assignments.page.html',
  imports: [
    CommonModule,
    TranslatePipe,
    ButtonModule,
    TableModule,
    TagModule,
    TooltipModule,
    I18nNamespaceDirective,
    PaginationComponent,
  ],
})
export class QuestionBankAssignmentsPage {
  private readonly service = inject(QuestionBankAssignmentsService);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);
  private readonly notification = inject(NotificationService);

  readonly assignments = signal<MyAssignment[]>([]);
  readonly loading = signal(true);
  readonly loadFailed = signal(false);
  readonly total = signal(0);
  readonly filters = signal<PaginatedRequest>({
    pageNumber: 1,
    pageSize: 10,
    sortBy: 'assignedAt',
    sortDirection: 'desc',
  });
  readonly statuses = AssignmentStatuses;
  readonly statusSeverity = questionBankAssignmentStatusSeverity;

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

    if (assignment.statusId === this.statuses.returnedForModification) {
      return 'QUESTION_ASSIGNMENTS.MODIFY';
    }

    return 'QUESTION_ASSIGNMENTS.VIEW';
  }

  onSort(event: SortEvent): void {
    if (!event.field) return;
    const sortDirection = event.order === -1 ? 'desc' : 'asc';
    const filters = this.filters();
    if (filters.sortBy === event.field && filters.sortDirection === sortDirection) return;

    this.filters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      sortBy: event.field,
      sortDirection,
    }));
    this.load();
  }

  page(pageNumber: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber }));
    this.load();
  }

  pageSize(pageSize: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber: 1, pageSize }));
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.loadFailed.set(false);
    this.service.list(this.filters()).subscribe({
      next: (response) => {
        this.assignments.set(response.items ?? []);
        this.total.set(response.metadata?.totalCount ?? 0);
        this.loading.set(false);
      },
      error: () => {
        this.loadFailed.set(true);
        this.loading.set(false);
        this.notification.error();
      },
    });
  }
}
