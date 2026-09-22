import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';
import { LanguageService } from '../../../../../../core/services/language.service';
import { TestSlotCandidateFilters, TestSlotCandidateListItemDto, TestSlotSessionDto } from '../../models/test-slot-details.dto';
import { TestSlotsService } from '../../services/test-slots.service';

interface TestSlotCandidatesDialogData { testSlotId: string; }

@Component({
  selector: 'app-test-slot-candidates-dialog',
  standalone: true,
  templateUrl: './test-slot-candidates-dialog.component.html',
  styleUrl: './test-slot-candidates-dialog.component.scss',
  imports: [FormsModule, TranslatePipe, IconFieldModule, InputIconModule, InputTextModule, SelectModule, TableModule, TagModule, PaginationComponent],
})
export class TestSlotCandidatesDialogComponent implements OnInit {
  private readonly service = inject(TestSlotsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly ref = inject(DynamicDialogRef);
  private readonly data = inject(DynamicDialogConfig<TestSlotCandidatesDialogData>).data!;

  readonly candidates = signal<TestSlotCandidateListItemDto[]>([]);
  readonly sessions = signal<TestSlotSessionDto[]>([]);
  readonly sessionOptions = computed(() => this.sessions().map(session => ({
    id: session.id,
    label: `${session.examNo} - ${session.sessionNo}`,
  })));
  readonly totalItems = signal(0);
  readonly loading = signal(false);
  readonly filters = signal<TestSlotCandidateFilters>({
    pageNumber: 1,
    pageSize: 10,
    language: this.language.get(),
    search: null,
    sessionId: null,
  });
  search = '';
  selectedSessionId: string | null = null;

  ngOnInit(): void {
    this.service.sessions(this.data.testSlotId, this.language.get())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(sessions => this.sessions.set(sessions));
    this.loadCandidates();
  }

  applyFilters(): void {
    this.filters.update(filters => ({
      ...filters,
      pageNumber: 1,
      search: this.search.trim() || null,
      sessionId: this.selectedSessionId,
    }));
    this.loadCandidates();
  }

  clearFilters(): void {
    this.search = '';
    this.selectedSessionId = null;
    this.applyFilters();
  }

  onPageChange(pageNumber: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber }));
    this.loadCandidates();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update(filters => ({ ...filters, pageSize, pageNumber: 1 }));
    this.loadCandidates();
  }

  close(): void {
    this.ref.close();
  }

  private loadCandidates(): void {
    this.loading.set(true);
    this.service.candidates(this.data.testSlotId, this.filters())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: response => {
          this.candidates.set(response.items);
          this.totalItems.set(response.metadata.totalCount);
          this.loading.set(false);
        },
        error: () => {
          this.candidates.set([]);
          this.totalItems.set(0);
          this.loading.set(false);
        },
      });
  }
}
