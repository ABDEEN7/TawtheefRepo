// candidate-dashboard.component.ts
import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';

interface JobRecord {
  id: number;
  title: string;
  entity: string;
  type: 'academic' | 'administrative' | 'labor';
  status: 'invited' | 'applied' | 'under_review' | 'withdrawn' | 'closed';
  date: string;
  jobId: number;
}

@Component({
  selector: 'app-candidate-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './candidate-dashboard.html',
  styleUrls: ['./candidate-dashboard.scss']
})
export class CandidateDashboard implements OnInit {

  // Sample data
  private readonly DATA: JobRecord[] = [
    {id:1, title:'معلم رياضيات', entity:'إدارة شؤون المدارس', type:'academic', status:'invited', date:'2025-10-28', jobId:1},
    {id:2, title:'أخصائي موارد بشرية', entity:'إدارة الموارد البشرية', type:'administrative', status:'applied', date:'2025-10-28', jobId:2},
    {id:3, title:'فني شبكات', entity:'إدارة نظم المعلومات', type:'labor', status:'invited', date:'2025-10-25', jobId:3},
    {id:4, title:'مشرف نشاط طلابي', entity:'إدارة التقييم', type:'academic', status:'withdrawn', date:'2025-10-20', jobId:4},
    {id:5, title:'منسق مختبرات', entity:'إدارة التقييم', type:'academic', status:'under_review', date:'2025-10-30', jobId:5},
    {id:6, title:'كاتب إداري', entity:'إدارة الموارد البشرية', type:'administrative', status:'closed', date:'2025-10-10', jobId:6}
  ];

  // Reactive signals
  allRecords = signal<JobRecord[]>(this.DATA);
  currentPage = signal(1);
  itemsPerPage = signal(10);

  // Filter signals
  statusFilter = signal<string>('');
  typeFilter = signal<string>('');
  entityFilter = signal<string>('');
  searchFilter = signal<string>('');

  // Computed filtered data
  filteredRecords = computed(() => {
    const status = this.statusFilter();
    const type = this.typeFilter();
    const entity = this.entityFilter().toLowerCase();
    const search = this.searchFilter().toLowerCase();

    return this.allRecords().filter(record => {
      if (status && record.status !== status) return false;
      if (type && record.type !== type) return false;
      if (entity && !record.entity.toLowerCase().includes(entity)) return false;
      if (search && !record.title.toLowerCase().includes(search)) return false;
      return true;
    });
  });

  // Computed paginated data
  paginatedRecords = computed(() => {
    const startIndex = (this.currentPage() - 1) * this.itemsPerPage();
    const endIndex = startIndex + this.itemsPerPage();
    return this.filteredRecords().slice(startIndex, endIndex);
  });

  // Computed KPIs
  kpiInvited = computed(() =>
    this.filteredRecords().filter(r => r.status === 'invited').length
  );

  kpiUnderReview = computed(() =>
    this.filteredRecords().filter(r => r.status === 'under_review').length
  );

  kpiWithdrawn = computed(() =>
    this.filteredRecords().filter(r => r.status === 'withdrawn').length
  );

  kpiApplied = computed(() =>
    this.filteredRecords().filter(r => r.status === 'applied').length
  );

  // Computed pagination info
  totalPages = computed(() =>
    Math.ceil(this.filteredRecords().length / this.itemsPerPage())
  );

  paginationStats = computed(() => {
    const total = this.filteredRecords().length;
    if (total === 0) return { start: 0, end: 0, total: 0 };

    const start = (this.currentPage() - 1) * this.itemsPerPage() + 1;
    const end = Math.min(start + this.itemsPerPage() - 1, total);

    return { start, end, total };
  });

  // Helper method for pagination numbers
  getPageNumbers(): number[] {
    const pages: number[] = [];
    for (let i = 1; i <= this.totalPages(); i++) {
      pages.push(i);
    }
    return pages;
  }

  ngOnInit() {
    // Initialize any required setup
  }

  // Filter methods
  onStatusFilterChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.statusFilter.set(value);
    this.currentPage.set(1);
  }

  onTypeFilterChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.typeFilter.set(value);
    this.currentPage.set(1);
  }

  onEntityFilterChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.entityFilter.set(value);
    this.currentPage.set(1);
  }

  onSearchFilterChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchFilter.set(value);
    this.currentPage.set(1);
  }

  // Clear all filters
  clearFilters(): void {
    this.statusFilter.set('');
    this.typeFilter.set('');
    this.entityFilter.set('');
    this.searchFilter.set('');
    this.currentPage.set(1);
  }

  // Pagination methods
  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update(page => page - 1);
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(page => page + 1);
    }
  }

  // Action methods
  withdrawApplication(recordId: number): void {
    this.allRecords.update(records =>
      records.map(record =>
        record.id === recordId ? { ...record, status: 'withdrawn' as const } : record
      )
    );
    this.currentPage.set(1);
  }

  // Helper methods for templates
  getTypeBadge(type: string): string {
    const classes = {
      academic: 'badge-soft academic',
      administrative: 'badge-soft administrative',
      labor: 'badge-soft labor'
    };
    return classes[type as keyof typeof classes] || 'badge-soft';
  }

  getTypeText(type: string): string {
    const texts = {
      academic: 'أكاديمي',
      administrative: 'إداري',
      labor: 'عمالي'
    };
    return texts[type as keyof typeof texts] || type;
  }

  getStatusPill(status: string): { class: string, text: string } {
    const statusMap = {
      invited: { class: 'status-invited', text: 'دعوة جديدة' },
      applied: { class: 'status-applied', text: 'تم التقديم' },
      under_review: { class: 'status-underreview', text: 'قيد المراجعة' },
      withdrawn: { class: 'status-withdrawn', text: 'ملغي' },
      closed: { class: 'status-closed', text: 'مغلقة' }
    };

    return statusMap[status as keyof typeof statusMap] || { class: 'status-closed', text: status };
  }

  getActionButtons(record: JobRecord): { showApply: boolean, showView: boolean, showTrack: boolean, showDetails: boolean, showWithdraw: boolean } {
    return {
      showApply: record.status === 'invited',
      showView: record.status === 'applied',
      showTrack: record.status === 'under_review',
      showDetails: ['withdrawn', 'closed'].includes(record.status),
      showWithdraw: record.status === 'applied'
    };
  }
}
