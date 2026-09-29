import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { TestSessionCandidateSummaryDto } from '../models/test-session-candidate.dto';
import { TestSessionExamDetailsDto } from '../models/test-session-exam-details.dto';
import { LocalTestSession } from '../models/ready-test-slot.dto';
import { TestSessionLookupsDto } from '../models/test-session-list-item.dto';
import {
  TestSessionGenderFilter,
  TestSessionNationalityFilter,
} from '../models/test-session-setup.dto';
import { LanguageService } from '../../../../../core/services/language.service';
import { TestSessionsService } from '../services/test-sessions.service';
import { TestSessionCandidatesStepComponent } from './test-session-candidates-step/test-session-candidates-step.component';
import { TestSessionExamSelectionStepComponent } from './test-session-exam-selection-step/test-session-exam-selection-step.component';
import { TestSessionPeriodsStepComponent } from './test-session-periods-step/test-session-periods-step.component';

@Component({
  selector: 'app-test-session-workflow',
  standalone: true,
  templateUrl: './test-session-workflow.component.html',
  styleUrl: './test-session-workflow.component.scss',
  imports: [
    CommonModule,
    TranslatePipe,
    TestSessionExamSelectionStepComponent,
    TestSessionCandidatesStepComponent,
    TestSessionPeriodsStepComponent,
  ],
})
export class TestSessionWorkflowComponent {
  private readonly service = inject(TestSessionsService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly language = inject(LanguageService);

  readonly lookups = signal<Pick<TestSessionLookupsDto, 'exams' | 'rooms'>>({
    exams: [],
    rooms: [],
  });
  readonly details = signal<TestSessionExamDetailsDto | undefined>(undefined);
  readonly candidateSummary = signal<TestSessionCandidateSummaryDto>({
    total: 0,
    eligible: 0,
    notReady: 0,
    excluded: 0,
  });
  readonly candidateGenderFilter = signal<TestSessionGenderFilter>(null);
  readonly candidateNationalityFilter = signal<TestSessionNationalityFilter>(null);
  readonly selectedCandidateIds = signal<string[]>([]);
  readonly localSession = signal<LocalTestSession | null>(null);
  readonly candidateSelectionInitialized = signal(false);
  readonly candidateSearch = signal('');
  step = 1;
  selectedExamId?: string;
  loading = true;
  readonly wizardSteps = [
    {
      id: 1,
      label: 'TEST_SESSION_WIZARD.STEP_EXAM',
      icon: 'hgi hgi-stroke hgi-clipboard-check-01 me-1',
    },
    {
      id: 2,
      label: 'TEST_SESSION_WIZARD.STEP_CANDIDATES',
      icon: 'hgi hgi-stroke hgi-user-group me-1',
    },
    {
      id: 3,
      label: 'TEST_SESSION_WIZARD.STEP_PERIODS',
      icon: 'hgi hgi-stroke hgi-calendar-03 me-1',
    },
    {
      id: 4,
      label: 'TEST_SESSION_WIZARD.STEP_ROOM_TEAM',
      icon: 'hgi hgi-stroke hgi-user-group me-1',
    },
  ];

  constructor() {
    this.loadLookups(this.language.get());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((language) => this.loadLookups(language));
  }

  onExamSelected(examId?: string): void {
    this.selectedExamId = examId;
    this.details.set(undefined);
    this.candidateGenderFilter.set(null);
    this.candidateNationalityFilter.set(null);
    this.selectedCandidateIds.set([]);
    this.candidateSelectionInitialized.set(false);
    this.candidateSearch.set('');
    if (examId) {
      this.service
        .examDetails(examId, this.language.get())
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe((details) => this.details.set(details));
    }
  }

  onCandidateGenderFilterChanged(filter: TestSessionGenderFilter): void {
    this.candidateGenderFilter.set(filter);
  }

  onCandidateNationalityFilterChanged(filter: TestSessionNationalityFilter): void {
    this.candidateNationalityFilter.set(filter);
  }

  onCandidateSelectionInitialized(candidateIds: string[]): void {
    if (!this.candidateSelectionInitialized()) {
      this.selectedCandidateIds.set(candidateIds);
      this.candidateSelectionInitialized.set(true);
    }
  }

  goTo(target: number): void {
    if (target < 1 || target > 4 || (target > 1 && !this.selectedExamId)) return;
    if (target === 4 && (this.step !== 3 || !this.canGoNext())) return;
    this.step = target;
  }

  canGoNext(): boolean {
    if (this.step === 1) return !!this.selectedExamId;
    if (this.step !== 3) return true;

    const session = this.localSession();
    const candidateCount = this.selectedCandidateIds().length;
    const examDuration = this.details()?.durationMinutes ?? 0;
    if (!session || candidateCount === 0 || session.availableCapacity < candidateCount)
      return false;

    const startTime = this.timeValue(session.startTime);
    const endTime = this.timeValue(session.endTime);
    return (
      startTime >= this.timeValue(session.slot.startTime) &&
      endTime <= this.timeValue(session.slot.endTime) &&
      endTime > startTime &&
      endTime - startTime >= examDuration
    );
  }

  private timeValue(time: string): number {
    const [hour = '0', minute = '0'] = time.split(':');
    return Number(hour) * 60 + Number(minute.slice(0, 2));
  }

  private loadLookups(language: string): void {
    this.loading = true;
    this.service
      .lookups(language)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (lookups) => {
          this.lookups.set(lookups);
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }
}
