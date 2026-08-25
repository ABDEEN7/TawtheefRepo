import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { Select } from 'primeng/select';
import { Skeleton } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { Tooltip } from 'primeng/tooltip';
import {
  EMPTY,
  Subject,
  catchError,
  debounceTime,
  defer,
  distinctUntilChanged,
  filter,
  finalize,
  map,
  merge,
  startWith,
  switchMap,
  take,
} from 'rxjs';
import { Permissions } from '../../../../core/constants/permissions';
import { DialogHelperService } from '../../../../core/services/dialog-helper.service';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { NotificationService } from '../../../../core/services/notification.service';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { GUID } from '../../../../shared/types/guid.type';
import {
  InvitationExceptionListItem,
  InvitationExceptionStatus,
  InvitationExceptionsSummary,
} from './models/invitation-exception.models';
import { InvitationExceptionsRequest } from './models/invitation-exception.requests';
import { ExceptionsService } from './services/exceptions.service';
import { AddExceptionDialogComponent } from './components/add-exception-dialog/add-exception-dialog.component';
import {
  ExceptionDetailsDialogComponent,
  ExceptionDetailsDialogData,
} from './components/exception-details-dialog/exception-details-dialog.component';
import {
  InvitationExceptionStatusPresentation,
  invitationExceptionStatusPresentations,
} from './models/invitation-exception-status.presentation';
import { PageFiltersComponent } from '../../../../shared/components/page-filters/page-filters.component';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import {
  CancelExceptionDialogComponent,
  CancelExceptionDialogData,
} from './components/cancel-exception-dialog/cancel-exception-dialog.component';

interface ExceptionStatusOption extends InvitationExceptionStatusPresentation {
  value: InvitationExceptionStatus;
}

interface SummaryCard {
  value: keyof InvitationExceptionsSummary;
  labelKey: string;
  className: string;
}

type ExceptionMutationAction = 'send' | 'cancel';

interface ExceptionMutationState {
  exceptionId: GUID;
  action: ExceptionMutationAction;
}

function areListRequestsEqual(
  previous: InvitationExceptionsRequest,
  current: InvitationExceptionsRequest,
): boolean {
  return (
    previous.pageNumber === current.pageNumber &&
    previous.pageSize === current.pageSize &&
    previous.search === current.search &&
    previous.status === current.status
  );
}

@Component({
  selector: 'app-exceptions',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    Select,
    Skeleton,
    TableModule,
    Tooltip,
    PaginationComponent,
    HasPermissionDirective,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    PageFiltersComponent,
    I18nNamespaceDirective,
  ],
  templateUrl: './exceptions.page.html',
  styleUrl: './exceptions.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [DialogService],
})
export class ExceptionsPage implements OnInit {
  private readonly exceptionsService = inject(ExceptionsService);
  private readonly dialogService = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly dialogHelper = inject(DialogHelperService);
  private readonly notificationService = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly refresh$ = new Subject<void>();

  protected readonly Permissions = Permissions;
  protected readonly summary = signal<InvitationExceptionsSummary | null>(null);
  protected readonly exceptions = signal<InvitationExceptionListItem[]>([]);
  protected readonly pagination = signal<PaginationMetadata | null>(null);
  protected readonly summaryLoading = signal(false);
  protected readonly listLoading = signal(false);
  protected readonly processingMutation = signal<ExceptionMutationState | null>(null);
  protected readonly search = signal('');
  protected readonly selectedStatus = signal<InvitationExceptionStatus | null>(null);
  protected readonly pageNumber = signal(1);
  protected readonly pageSize = signal(10);
  private readonly appliedSearch = signal('');
  private readonly listRequest = computed<InvitationExceptionsRequest>(() => ({
    pageNumber: this.pageNumber(),
    pageSize: this.pageSize(),
    search: this.appliedSearch() || undefined,
    status: this.selectedStatus() ?? undefined,
  }));
  private readonly search$ = toObservable(this.search);
  private readonly listRequest$ = toObservable(this.listRequest);
  protected readonly totalItems = computed(() => this.pagination()?.totalCount ?? 0);
  protected readonly summaryPlaceholders = [0, 1, 2, 3, 4, 5] as const;
  protected readonly tableColumnPlaceholders = [0, 1, 2, 3, 4, 5, 6] as const;
  protected readonly tablePlaceholders = [0, 1, 2, 3, 4] as const;

  protected readonly statusPresentations = invitationExceptionStatusPresentations;

  protected readonly statusOptions: ExceptionStatusOption[] = Object.entries(
    this.statusPresentations,
  ).map(([value, presentation]) => ({
    value: Number(value) as InvitationExceptionStatus,
    ...presentation,
  }));

  protected readonly summaryCards: SummaryCard[] = [
    { value: 'total', labelKey: 'EXCEPTIONS.SUMMARY.TOTAL', className: 'bg-light' },
    {
      value: 'readyToSend',
      labelKey: 'EXCEPTIONS.SUMMARY.READY_TO_SEND',
      className: 'bg-alert-light',
    },
    {
      value: 'invitationSent',
      labelKey: 'EXCEPTIONS.SUMMARY.INVITATION_SENT',
      className: 'bg-info-light',
    },
    { value: 'applied', labelKey: 'EXCEPTIONS.SUMMARY.APPLIED', className: 'bg-success-light' },
    { value: 'expired', labelKey: 'EXCEPTIONS.SUMMARY.EXPIRED', className: 'bg-light' },
    { value: 'cancelled', labelKey: 'EXCEPTIONS.SUMMARY.CANCELLED', className: 'bg-warning-light' },
  ];

  protected readonly showAdvancedFilters = signal(false);

  protected readonly activeAdvancedFilterCount = computed(() =>
    this.selectedStatus() !== null ? 1 : 0,
  );

  protected toggleAdvancedFilters(): void {
    this.showAdvancedFilters.update((value) => !value);
  }

  ngOnInit(): void {
    this.setupSearchPipeline();
    this.setupListPipeline();
    this.setupSummaryPipeline();
  }

  protected onSearchChange(value: string): void {
    this.search.set(value);
  }

  protected onStatusChange(status: InvitationExceptionStatus | null | undefined): void {
    this.selectedStatus.set(status ?? null);
    this.pageNumber.set(1);
  }

  protected openAddExceptionDialog(): void {
    const dialogRef = this.dialogService.open(AddExceptionDialogComponent, {
      header: this.translate.instant('EXCEPTIONS.ADD'),
      width: '48rem',
      breakpoints: { '640px': 'calc(100vw - 2rem)' },
      closable: true,
      draggable: false,
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
    });

    dialogRef?.onClose
      .pipe(
        filter((created: unknown): created is true => created === true),
        take(1),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => this.refresh());
  }

  protected openDetailsDialog(exceptionId: InvitationExceptionListItem['exceptionId']): void {
    const data: ExceptionDetailsDialogData = { exceptionId };

    this.dialogService.open(ExceptionDetailsDialogComponent, {
      header: this.translate.instant('EXCEPTIONS.DETAILS.TITLE'),
      data,
      styleClass: 'exception-details-dialog-container',
      width: '880px',
      breakpoints: {
        '992px': 'calc(100vw - 2rem)',
      },
      focusOnShow: false,
      closable: true,
      draggable: false,
      contentStyle: {
        'max-height': '85vh',
        overflow: 'auto',
      },
    });
  }

  protected onPageChange(pageNumber: number): void {
    this.pageNumber.set(pageNumber);
  }

  protected onPageSizeChange(pageSize: number): void {
    this.pageSize.set(pageSize);
    this.pageNumber.set(1);
  }

  protected getStatusPresentation(
    status: InvitationExceptionStatus,
  ): InvitationExceptionStatusPresentation {
    return this.statusPresentations[status];
  }

  protected canSendInvitation(item: InvitationExceptionListItem): boolean {
    return item.status === InvitationExceptionStatus.ReadyToSend && item.invitationId === null;
  }

  protected canCancelException(item: InvitationExceptionListItem): boolean {
    return (
      item.status === InvitationExceptionStatus.ReadyToSend ||
      item.status === InvitationExceptionStatus.InvitationSent
    );
  }

  protected isProcessing(exceptionId: GUID, action: ExceptionMutationAction): boolean {
    const processing = this.processingMutation();
    return processing?.exceptionId === exceptionId && processing.action === action;
  }

  protected confirmSendInvitation(item: InvitationExceptionListItem): void {
    if (!this.canSendInvitation(item) || this.processingMutation() !== null) {
      return;
    }

    const dialogRef = this.dialogHelper.openConfirmDialog({
      type: 'submit',
      title: 'EXCEPTIONS.ACTIONS.SEND_TITLE',
      description: 'EXCEPTIONS.ACTIONS.SEND_CONFIRMATION',
      descriptionParams: {
        candidateName: item.candidateName,
        jobTitle: item.jobTitle,
      },
      cancelText: 'common.cancel',
      confirmText: 'EXCEPTIONS.ACTIONS.CONFIRM_SEND',
    });

    dialogRef?.onClose
      .pipe(
        filter((confirmed: unknown): confirmed is true => confirmed === true),
        switchMap(() => {
          this.processingMutation.set({ exceptionId: item.exceptionId, action: 'send' });
          return this.exceptionsService
            .sendInvitation(item.exceptionId)
            .pipe(finalize(() => this.processingMutation.set(null)));
        }),
        take(1),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.notificationService.success(
            this.translate.instant('EXCEPTIONS.ACTIONS.SEND_SUCCESS'),
          );
          this.refresh();
        },
        error: (error) => this.handleMutationError(error),
      });
  }

  protected confirmCancelException(item: InvitationExceptionListItem): void {
    if (!this.canCancelException(item) || this.processingMutation() !== null) {
      return;
    }

    const data: CancelExceptionDialogData = {
      candidateName: item.candidateName,
      jobTitle: item.jobTitle,
      statusLabel: this.translate.instant(this.getStatusPresentation(item.status).labelKey),
      invitationSent: item.status === InvitationExceptionStatus.InvitationSent,
    };

    const dialogRef = this.dialogService.open(CancelExceptionDialogComponent, {
      header: this.translate.instant('EXCEPTIONS.ACTIONS.CANCEL_TITLE'),
      data,
      width: '42rem',
      breakpoints: {
        '640px': 'calc(100vw - 2rem)',
      },
      closable: true,
      draggable: false,
      focusOnShow: false,
    });

    dialogRef?.onClose
      .pipe(
        filter(
          (reason: unknown): reason is string =>
            typeof reason === 'string' && reason.trim().length > 0,
        ),
        switchMap((reason) => {
          this.processingMutation.set({
            exceptionId: item.exceptionId,
            action: 'cancel',
          });

          return this.exceptionsService
            .cancelException(item.exceptionId, reason.trim())
            .pipe(finalize(() => this.processingMutation.set(null)));
        }),
        take(1),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.notificationService.success(
            this.translate.instant('EXCEPTIONS.ACTIONS.CANCEL_SUCCESS'),
          );

          this.refresh();
        },
        error: (error) => this.handleMutationError(error),
      });
  }

  protected clearFilters(): void {
    this.search.set('');
    this.appliedSearch.set('');
    this.selectedStatus.set(null);
    this.pageNumber.set(1);
  }

  protected refresh(): void {
    this.refresh$.next();
  }

  private setupSearchPipeline(): void {
    this.search$
      .pipe(
        map((value) => value.trim()),
        debounceTime(400),
        filter((value) => value !== this.appliedSearch()),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((search) => {
        this.pageNumber.set(1);
        this.appliedSearch.set(search);
      });
  }

  private setupListPipeline(): void {
    const queryChanges$ = this.listRequest$.pipe(distinctUntilChanged(areListRequestsEqual));

    merge(queryChanges$, this.refresh$.pipe(map(() => this.listRequest())))
      .pipe(
        switchMap((request) =>
          defer(() => {
            this.listLoading.set(true);
            return this.exceptionsService.getExceptions(request).pipe(
              catchError(() => EMPTY),
              finalize(() => this.listLoading.set(false)),
            );
          }),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((response) => {
        this.exceptions.set(response.items);
        this.pagination.set(response.metadata);
        this.pageNumber.set(response.metadata.currentPage);
        this.pageSize.set(response.metadata.pageSize);
      });
  }

  private setupSummaryPipeline(): void {
    this.refresh$
      .pipe(
        startWith(undefined),
        switchMap(() =>
          defer(() => {
            this.summaryLoading.set(true);
            return this.exceptionsService.getSummary().pipe(
              catchError(() => EMPTY),
              finalize(() => this.summaryLoading.set(false)),
            );
          }),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((summary) => this.summary.set(summary));
  }

  private handleMutationError(error: unknown): void {
    if (error instanceof HttpErrorResponse && error.status === 409) {
      this.refresh();
    }
  }
}
