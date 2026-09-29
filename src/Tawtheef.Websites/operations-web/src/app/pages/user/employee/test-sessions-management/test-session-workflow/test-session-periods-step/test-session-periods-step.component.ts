import { CommonModule } from '@angular/common';
import {
  Component,
  DestroyRef,
  inject,
  input,
  OnChanges,
  OnInit,
  output,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DatePickerModule } from 'primeng/datepicker';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { DialogService } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { catchError, debounceTime, finalize, of, Subject } from 'rxjs';
import { LanguageService } from '../../../../../../core/services/language.service';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';
import { PageFiltersComponent } from '../../../../../../shared/components/page-filters/page-filters.component';
import { TestSessionCandidateSummaryDto } from '../../models/test-session-candidate.dto';
import {
  TestSessionExamDetailsDto,
  TestSessionExamPartDto,
} from '../../models/test-session-exam-details.dto';
import {
  LocalTestSession,
  ReadyTestSlotFilters,
  ReadyTestSlotListItemDto,
  ReadyTestSlotsSummaryDto,
} from '../../models/ready-test-slot.dto';
import { TestSessionLookupsDto } from '../../models/test-session-list-item.dto';
import { TestSessionsService } from '../../services/test-sessions.service';
import { TestSessionSetupDialogComponent } from '../test-session-setup-dialog/test-session-setup-dialog.component';

@Component({
  selector: 'app-test-session-periods-step',
  standalone: true,
  templateUrl: './test-session-periods-step.component.html',
  styleUrl: './test-session-periods-step.component.scss',
  providers: [DialogService],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    DatePickerModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    Select,
    TableModule,
    PaginationComponent,
    PageFiltersComponent,
  ],
})
export class TestSessionPeriodsStepComponent implements OnInit, OnChanges {
  readonly examId = input.required<string>();
  readonly examDetails = input.required<TestSessionExamDetailsDto>();
  readonly candidateSummary = input.required<TestSessionCandidateSummaryDto>();
  readonly selectedCandidateIds = input.required<string[]>();
  readonly rooms = input.required<TestSessionLookupsDto['rooms']>();
  readonly localSession = input<LocalTestSession | null>(null);
  readonly sessionApplied = output<LocalTestSession>();
  readonly sessionDeleted = output<void>();

  private readonly service = inject(TestSessionsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialogService = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly filterChanges$ = new Subject<void>();

  readonly slots = signal<ReadyTestSlotListItemDto[]>([]);
  readonly summary = signal<ReadyTestSlotsSummaryDto>({
    existingExamSessionCount: 0,
    existingExamCandidateCount: 0,
    availableCapacity: 0,
  });
  readonly totalItems = signal(0);
  readonly loading = signal(false);
  readonly loadFailed = signal(false);
  readonly filters = signal<ReadyTestSlotFilters>({
    examId: '',
    language: this.language.get(),
    pageNumber: 1,
    pageSize: 10,
    selectedCandidateCount: 0,
  });

  searchText = '';
  selectedRoomId?: string;
  selectedDate: Date | null = null;

  ngOnInit(): void {
    this.filterChanges$
      .pipe(debounceTime(400), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());
    this.language.current$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((language) => {
      this.filters.update((filters) => ({ ...filters, language }));
      this.loadSlots();
    });
  }

  ngOnChanges(): void {
    this.filters.update((filters) => ({
      ...filters,
      examId: this.examId(),
      selectedCandidateCount: this.selectedCandidateIds().length,
      pageNumber: 1,
    }));
    this.loadSlots();
  }

  onSearchChanged(): void {
    this.filterChanges$.next();
  }

  applyFilters(): void {
    this.filters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      searchText: this.searchText.trim() || undefined,
      roomId: this.selectedRoomId,
      date: this.toDateFilter(this.selectedDate),
    }));
    this.loadSlots();
  }

  clearFilters(): void {
    this.searchText = '';
    this.selectedRoomId = undefined;
    this.selectedDate = null;
    this.applyFilters();
  }

  onPageChange(pageNumber: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber }));
    this.loadSlots();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber: 1, pageSize }));
    this.loadSlots();
  }

  openSetup(slot: ReadyTestSlotListItemDto, session?: LocalTestSession): void {
    if (this.localSession() && !session) return;

    const ref = this.dialogService.open(TestSessionSetupDialogComponent, {
      header: this.translate.instant('TEST_SESSION_WIZARD.SESSION_SETUP_TITLE'),
      width: '80%',
      modal: true,
      closable: true,
      dismissableMask: false,
      breakpoints: { '768px': '95vw' },
      data: {
        slot,
        exam: this.examDetails(),
        selectedCandidateCount: this.selectedCandidateIds().length,
        initialSession: session,
      },
    });
    ref?.onClose.subscribe((result) => {
      if (result) {
        this.sessionApplied.emit({
          slot,
          startTime: result.startTime,
          endTime: result.endTime,
          availableCapacity: result.availableSeats,
        });
      }
    });
  }

  deleteSession(): void {
    this.sessionDeleted.emit();
  }

  remainingCandidates(): number {
    return this.selectedCandidateIds().length;
  }

  isSessionCapacitySufficient(): boolean {
    const session = this.localSession();
    return (
      !!session &&
      this.selectedCandidateIds().length > 0 &&
      session.availableCapacity >= this.selectedCandidateIds().length &&
      this.isSessionTimeValid(session)
    );
  }

  sessionCapacityGap(): number {
    const session = this.localSession();
    return session
      ? Math.max(0, this.selectedCandidateIds().length - session.availableCapacity)
      : 0;
  }

  isSessionTimeValid(session: LocalTestSession): boolean {
    const timeValue = (time: string): number => {
      const [hour = '0', minute = '0'] = time.split(':');
      return Number(hour) * 60 + Number(minute.slice(0, 2));
    };
    const start = timeValue(session.startTime);
    const end = timeValue(session.endTime);
    return (
      start >= timeValue(session.slot.startTime) &&
      end <= timeValue(session.slot.endTime) &&
      end > start &&
      end - start >= this.examDetails().durationMinutes
    );
  }

  firstExamPart(): TestSessionExamPartDto | undefined {
    return this.examDetails().parts.find((part) => part.partNo === 1);
  }

  secondExamPart(): TestSessionExamPartDto | undefined {
    return this.examDetails().parts.find((part) => part.partNo === 2);
  }

  private loadSlots(): void {
    if (!this.filters().examId) return;

    this.loading.set(true);
    this.loadFailed.set(false);
    this.service
      .readyTestSlots(this.filters())
      .pipe(
        catchError(() => {
          this.loadFailed.set(true);
          return of(null);
        }),
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((response) => {
        this.slots.set(response?.slots.items ?? []);
        this.totalItems.set(response?.slots.metadata.totalCount ?? 0);
        this.summary.set(
          response?.summary ?? {
            existingExamSessionCount: 0,
            existingExamCandidateCount: 0,
            availableCapacity: 0,
          },
        );
      });
  }

  private toDateFilter(date: Date | null): string | undefined {
    if (!date) return undefined;
    const year = String(date.getFullYear()).padStart(4, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
