import {
  Component,
  computed,
  DestroyRef,
  inject,
  input,
  OnChanges,
  output,
  signal,
  SimpleChanges,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { InputIconModule } from 'primeng/inputicon';
import { IconFieldModule } from 'primeng/iconfield';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TestSessionsService } from '../../services/test-sessions.service';
import { InvitationSource } from '../../../../../../core/enums/invitation-source.enum';
import { LanguageService } from '../../../../../../core/services/language.service';
import {
  TestSessionCandidateFilters,
  TestSessionCandidateConflictDto,
  TestSessionCandidateListItemDto,
  TestSessionCandidateSummaryDto,
} from '../../models/test-session-candidate.dto';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-test-session-candidates-step',
  standalone: true,
  templateUrl: './test-session-candidates-step.component.html',
  styleUrl: './test-session-candidates-step.component.scss',
  imports: [
    FormsModule,
    TranslatePipe,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    CheckboxModule,
    SelectModule,
    TableModule,
    TagModule,
    PaginationComponent,
  ],
})
export class TestSessionCandidatesStepComponent implements OnChanges {
  protected readonly InvitationSource = InvitationSource;
  readonly examId = input.required<string>();
  readonly testSessionId = input<string | null>(null);
  readonly readOnly = input(false);
  readonly genderFilter = input<'Male' | 'Female' | null>(null);
  readonly nationalityFilter = input<'Qatari' | 'NonQatari' | null>(null);
  readonly selectedCandidateIds = input<string[]>([]);
  readonly conflicts = input<TestSessionCandidateConflictDto[]>([]);
  readonly selectionInitialized = input(false);
  readonly search = input('');
  readonly genderFilterChanged = output<'Male' | 'Female' | null>();
  readonly nationalityFilterChanged = output<'Qatari' | 'NonQatari' | null>();
  readonly selectedCandidateIdsChanged = output<string[]>();
  readonly selectionInitializedChanged = output<string[]>();
  readonly searchChanged = output<string>();
  readonly stateChanged = output<boolean>();
  readonly summaryChanged = output<TestSessionCandidateSummaryDto>();

  private readonly service = inject(TestSessionsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);
  private loadSequence = 0;

  readonly candidates = signal<TestSessionCandidateListItemDto[]>([]);
  readonly searchedCandidates = computed(() => {
    const search = this.search().trim().toLocaleLowerCase();
    if (!search) return this.candidates();

    return this.candidates().filter(
      (candidate) =>
        candidate.candidateName.toLocaleLowerCase().includes(search) ||
        candidate.candidateNumber?.toLocaleLowerCase().includes(search),
    );
  });
  readonly pagedCandidates = computed(() => {
    const start = (this.currentPage() - 1) * this.itemsPerPage();
    return this.searchedCandidates().slice(start, start + this.itemsPerPage());
  });
  readonly allCandidatesSelected = computed(() => {
    const candidates = this.candidates();
    const selectedIds = new Set(this.selectedCandidateIds());
    return (
      candidates.length > 0 &&
      candidates.every((candidate) => selectedIds.has(candidate.invitationId))
    );
  });
  readonly summary = signal<TestSessionCandidateSummaryDto>({
    total: 0,
    eligible: 0,
    notReady: 0,
    excluded: 0,
  });
  readonly currentPage = signal(1);
  readonly itemsPerPage = signal(10);
  readonly totalItems = computed(() => this.searchedCandidates().length);
  readonly loading = signal(false);
  readonly loadFailed = signal(false);
  readonly filters = signal<TestSessionCandidateFilters>({
    examId: '',
    language: this.language.get(),
  });
  readonly genderOptions = [
    { value: 'Male' as const, label: 'TEST_SESSION_WIZARD.MALE' },
    { value: 'Female' as const, label: 'TEST_SESSION_WIZARD.FEMALE' },
  ];
  readonly nationalityOptions = [
    { value: 'Qatari' as const, label: 'TEST_SESSION_WIZARD.QATARI' },
    { value: 'NonQatari' as const, label: 'TEST_SESSION_WIZARD.NON_QATARI' },
  ];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['search']) this.currentPage.set(1);
    const backendFiltersChanged =
      (!!changes['genderFilter'] && !changes['genderFilter'].firstChange) ||
      (!!changes['nationalityFilter'] && !changes['nationalityFilter'].firstChange);
    if (!changes['examId'] && !changes['genderFilter'] && !changes['nationalityFilter'] &&
      !changes['testSessionId']) return;

    this.filters.update((filters) => ({
      ...filters,
      examId: this.examId(),
      genderFilter: this.genderFilter(),
      nationalityFilter: this.nationalityFilter(),
      testSessionId: this.testSessionId() ?? undefined,
    }));
    this.currentPage.set(1);
    this.loadCandidates(backendFiltersChanged);
  }

  onSearchChanged(search: string): void {
    this.searchChanged.emit(search);
  }

  clearSearch(): void {
    this.searchChanged.emit('');
  }

  toggleAllCandidates(checked: boolean): void {
    const selectedIds = new Set(this.selectedCandidateIds());
    this.candidates().forEach((candidate) => {
      checked
        ? selectedIds.add(candidate.invitationId)
        : selectedIds.delete(candidate.invitationId);
    });
    this.selectedCandidateIdsChanged.emit([...selectedIds]);
  }

  onPageChange(pageNumber: number): void {
    this.currentPage.set(pageNumber);
  }

  onPageSizeChange(pageSize: number): void {
    this.itemsPerPage.set(pageSize);
    this.currentPage.set(1);
  }

  statusSeverity(
    status: TestSessionCandidateListItemDto['eligibilityStatus'],
  ): 'success' | 'warn' | 'danger' {
    return status === 'Eligible' ? 'success' : status === 'NotReady' ? 'warn' : 'danger';
  }

  toggleCandidate(invitationId: string, checked: boolean): void {
    const ids = new Set(this.selectedCandidateIds());
    checked ? ids.add(invitationId) : ids.delete(invitationId);
    this.selectedCandidateIdsChanged.emit([...ids]);
  }

  private loadCandidates(reselectAllOnResult = false): void {
    if (!this.filters().examId) return;

    const loadSequence = ++this.loadSequence;
    this.loading.set(true);
    this.loadFailed.set(false);
    this.stateChanged.emit(false);
    this.service
      .candidates(this.filters())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          if (loadSequence !== this.loadSequence) return;

          this.candidates.set(response.candidates);
          const candidateIds = response.candidates.map((candidate) => candidate.invitationId);
          const currentCandidateIds = new Set(candidateIds);
          const selectedIds = reselectAllOnResult || !this.selectionInitialized()
            ? candidateIds
            : this.selectedCandidateIds().filter((id) => currentCandidateIds.has(id));
          this.selectedCandidateIdsChanged.emit(selectedIds);
          if (!this.selectionInitialized()) {
            this.selectionInitializedChanged.emit(candidateIds);
          }
          this.summary.set(response.summary);
          this.summaryChanged.emit(response.summary);
          this.loading.set(false);
          this.stateChanged.emit(true);
        },
        error: () => {
          if (loadSequence !== this.loadSequence) return;

          this.candidates.set([]);
          this.selectedCandidateIdsChanged.emit([]);
          this.summary.set({ total: 0, eligible: 0, notReady: 0, excluded: 0 });
          this.summaryChanged.emit({ total: 0, eligible: 0, notReady: 0, excluded: 0 });
          this.loading.set(false);
          this.loadFailed.set(true);
          this.stateChanged.emit(false);
        },
      });
  }
}
