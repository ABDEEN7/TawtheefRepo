import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Job, Invite, Application, JobSummary, FilterState } from './job-invitation-summary.model';

@Component({
  selector: 'app-job-invitation-summary',
  templateUrl: './job-invitation-summary.html',
  styleUrls: ['./job-invitation-summary.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslatePipe]
})
export class JobInvitationSummary implements OnInit {
  private translate = inject(TranslateService);

  // Mock data
  private readonly JOBS: Job[] = [
    { id: 1, title: 'معلم رياضيات', entity: 'إدارة شؤون المدارس', type: 'academic', status: 'open' },
    { id: 2, title: 'أخصائي موارد بشرية', entity: 'إدارة الموارد البشرية', type: 'administrative', status: 'open' },
    { id: 3, title: 'فني شبكات', entity: 'إدارة نظم المعلومات', type: 'labor', status: 'open' },
    { id: 4, title: 'مشرف نشاط طلابي', entity: 'إدارة التقييم', type: 'academic', status: 'closed' }
  ];

  private readonly INVITES: Invite[] = [
    { jobId: 1, profileId: 101, status: 'new', sentAt: '2025-10-28' },
    { jobId: 1, profileId: 102, status: 'viewed', sentAt: '2025-10-28' },
    { jobId: 1, profileId: 105, status: 'applied', sentAt: '2025-10-29' },
    { jobId: 2, profileId: 103, status: 'applied', sentAt: '2025-10-27' },
    { jobId: 2, profileId: 106, status: 'declined', sentAt: '2025-10-27' },
    { jobId: 3, profileId: 104, status: 'new', sentAt: '2025-10-25' },
    { jobId: 3, profileId: 102, status: 'viewed', sentAt: '2025-10-25' },
    { jobId: 4, profileId: 101, status: 'viewed', sentAt: '2025-10-20' },
    { jobId: 4, profileId: 105, status: 'declined', sentAt: '2025-10-20' }
  ];

  private readonly APPLICATIONS: Application[] = [
    { jobId: 1, profileId: 105, appliedAt: '2025-10-30', status: 'قيد المراجعة' },
    { jobId: 2, profileId: 103, appliedAt: '2025-10-28', status: 'قيد المراجعة' }
  ];

  readonly TYPE_LABEL = {
    academic: { en: 'Academic', ar: 'أكاديمية' },
    administrative: { en: 'Administrative', ar: 'إدارية' },
    labor: { en: 'Labor', ar: 'عمالية' }
  };

  // Signals
  private allJobSummaries = signal<JobSummary[]>([]);
  currentPage = signal(1);
  itemsPerPage = signal(10);
  filters = signal<FilterState>({ type: '', entity: '', status: '' });

  // Computed values
  filteredJobSummaries = computed(() => {
    const summaries = this.allJobSummaries();
    const filter = this.filters();

    return summaries.filter(summary => {
      const job = summary.job;
      return (
        (!filter.type || job.type === filter.type) &&
        (!filter.entity || job.entity === filter.entity) &&
        (!filter.status || job.status === filter.status)
      );
    });
  });

  paginatedJobSummaries = computed(() => {
    const filtered = this.filteredJobSummaries();
    const start = (this.currentPage() - 1) * this.itemsPerPage();
    const end = start + this.itemsPerPage();
    return filtered.slice(start, end);
  });

  totalPages = computed(() =>
    Math.max(1, Math.ceil(this.filteredJobSummaries().length / this.itemsPerPage()))
  );

  paginationStats = computed(() => {
    const total = this.filteredJobSummaries().length;
    if (total === 0) {
      return this.translate.instant('JOB_INVITATION_SUMMARY.NO_RESULTS');
    }

    const start = (this.currentPage() - 1) * this.itemsPerPage() + 1;
    const end = Math.min(total, this.currentPage() * this.itemsPerPage());

    return this.translate.instant('JOB_INVITATION_SUMMARY.SHOWING_RESULTS', {
      start,
      end,
      total
    });
  });

  ngOnInit(): void {
    this.initializeData();
  }

  private initializeData(): void {
    const summaries = this.JOBS.map(job => {
      const jobInvites = this.INVITES.filter(invite => invite.jobId === job.id);
      const jobApplications = this.APPLICATIONS.filter(app => app.jobId === job.id);

      return {
        job,
        totalInv: jobInvites.length,
        applied: jobApplications.length,
        declined: jobInvites.filter(inv => inv.status === 'declined').length,
        unseen: jobInvites.filter(inv => inv.status === 'new').length
      };
    });

    this.allJobSummaries.set(summaries);
  }

  onFilterChange(): void {
    this.currentPage.set(1);
  }

  onTypeFilterChange(type: string): void {
    this.filters.update(filters => ({ ...filters, type }));
    this.onFilterChange();
  }

  onEntityFilterChange(entity: string): void {
    this.filters.update(filters => ({ ...filters, entity }));
    this.onFilterChange();
  }

  onStatusFilterChange(status: string): void {
    this.filters.update(filters => ({ ...filters, status }));
    this.onFilterChange();
  }

  clearFilters(): void {
    this.filters.set({ type: '', entity: '', status: '' });
    this.currentPage.set(1);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  getTypeLabel(type: keyof typeof this.TYPE_LABEL): string {
    const currentLang = this.translate.currentLang;
    return this.TYPE_LABEL[type][currentLang as 'en' | 'ar'] || type;
  }

  getPaginationRange(): number[] {
    const pages: number[] = [];
    for (let i = 1; i <= this.totalPages(); i++) {
      pages.push(i);
    }
    return pages;
  }
}
