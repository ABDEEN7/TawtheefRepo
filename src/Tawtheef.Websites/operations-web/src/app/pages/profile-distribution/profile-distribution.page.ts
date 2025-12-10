import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';
import { ProfileDistributionService } from './services/profile-distribution.service';
import {
  AutoAssignRequest,
  DistributionEmployee,
  DistributionFile,
  DistributionResult,
  EmployeeAvailability,
  ManualAssignRequest,
  ReassignRequest,
} from './models/profile-distribution.models';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';
import {ProfileStatusNumber} from '../../core/enums/lookups.enum';

@Component({
  selector: 'app-profile-distribution-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, I18nNamespaceDirective],
  templateUrl: './profile-distribution.page.html',
  styleUrl: './profile-distribution.page.scss',
})
export class ProfileDistributionPage implements OnInit {
  private api = inject(ProfileDistributionService);

  files = signal<DistributionFile[]>([]);
  employees = signal<DistributionEmployee[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  statusFilter = signal<ProfileStatusNumber | 'all'>('all');
  search = signal('');
  selectedIds = signal<Set<string>>(new Set());

  manualEmployeeId = signal<string>('');
  autoEmployeeIds = signal<Set<string>>(new Set());
  autoLimit = signal<number | null>(null);

  readonly filteredFiles = computed(() => {
    const searchTerm = this.search().trim().toLowerCase();
    const status = this.statusFilter();

    return this.files().filter(file => {
      const matchesStatus = status === 'all' ? true : file.status === status;
      const matchesSearch =
        !searchTerm ||
        file.candidateName.toLowerCase().includes(searchTerm) ||
        file.specialization.toLowerCase().includes(searchTerm) ||
        (file.targetEntity ?? '').toLowerCase().includes(searchTerm);
      return matchesStatus && matchesSearch;
    });
  });

  readonly availableEmployees = computed(() =>
    this.employees().filter(e => e.isActive && e.availability === EmployeeAvailability.Available)
  );

  readonly statusOptions: { value: ProfileStatusNumber | 'all'; label: string }[] = [
    { value: 'all', label: 'distribution.filters.statusAll' },
    { value: ProfileStatusNumber.Submitted, label: 'distribution.filters.statusSubmitted' },
    { value: ProfileStatusNumber.UnderReview, label: 'distribution.filters.statusUnderReview' },
    { value: ProfileStatusNumber.RequiresUpdate, label: 'distribution.filters.statusNeedsChanges' },
  ];

  protected readonly ProfileFileStatus = ProfileStatusNumber;
  protected readonly EmployeeAvailability = EmployeeAvailability;

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);
    this.successMessage.set(null);
    this.api
      .getFiles()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: files => this.files.set(files),
        error: () => this.error.set('تعذر تحميل الملفات القابلة للتوزيع'),
      });

    this.api.getEmployees().subscribe({
      next: employees => this.employees.set(employees),
      error: () => this.error.set('تعذر تحميل بيانات الموظفين'),
    });
  }

  toggleSelection(id: string): void {
    const set = new Set(this.selectedIds());
    set.has(id) ? set.delete(id) : set.add(id);
    this.selectedIds.set(set);
  }

  selectAll(): void {
    this.selectedIds.set(new Set(this.filteredFiles().map(f => f.profileId)));
  }

  clearSelection(): void {
    this.selectedIds.set(new Set());
  }

  assignManual(): void {
    const employeeId = this.manualEmployeeId();
    if (!employeeId || this.selectedIds().size === 0) return;

    const payload: ManualAssignRequest = {
      employeeId,
      profileIds: Array.from(this.selectedIds()),
    };

    this.loading.set(true);
    this.api
      .assignManually(payload)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
        error: err => this.handleError(err, 'تعذر إسناد الملفات يدويًا'),
      });
  }

  assignAuto(): void {
    const employeeIds = Array.from(this.autoEmployeeIds());
    if (employeeIds.length === 0) {
      this.error.set('يرجى اختيار موظفين للتوزيع الآلي');
      return;
    }
    const profileIds= Array.from(this.selectedIds());
    if(profileIds.length===0){
      this.error.set('يرجى اختيار ملفات للتوزيع الآلي');
      return;
    }

    const payload: AutoAssignRequest = {
      employeeIds,
      profileIds,
      perEmployeeCount: this.autoLimit(),
    };

    this.loading.set(true);
    this.api
      .assignAutomatically(payload)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
        error: err => this.handleError(err, 'تعذر التوزيع الآلي'),
      });
  }

  redistribute(mode: 'manual' | 'auto'): void {
    if (this.selectedIds().size === 0) return;

    const payload: ReassignRequest = {
      mode,
      profileIds: Array.from(this.selectedIds()),
      employeeId: this.manualEmployeeId(),
      employeeIds: Array.from(this.autoEmployeeIds()),
      perEmployeeCount: this.autoLimit(),
    };

    this.loading.set(true);
    this.api
      .reassign(payload)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
        error: err => this.handleError(err, 'تعذر إعادة التوزيع'),
      });
  }

  toggleEmployee(employeeId: string): void {
    const set = new Set(this.autoEmployeeIds());
    set.has(employeeId) ? set.delete(employeeId) : set.add(employeeId);
    this.autoEmployeeIds.set(set);
  }

  statusClass(status: ProfileStatusNumber): string {
    switch (status) {
      case ProfileStatusNumber.Submitted:
        return 'pill neutral';
      case ProfileStatusNumber.UnderReview:
        return 'pill info';
      case ProfileStatusNumber.Cancelled:
        return 'pill warning';
      default:
        return 'pill soft';
    }
  }

  statusLabel(status: ProfileStatusNumber): string {
    switch (status) {
      case ProfileStatusNumber.Submitted:
        return 'distribution.status.submitted';
      case ProfileStatusNumber.UnderReview:
        return 'distribution.status.underReview';
      case ProfileStatusNumber.RequiresUpdate:
        return 'distribution.status.needsChanges';
      case ProfileStatusNumber.Approved:
        return 'distribution.status.approved';
      case ProfileStatusNumber.Rejected:
        return 'distribution.status.rejected';
      case ProfileStatusNumber.Cancelled:
        return 'distribution.status.cancelled';
      default:
        return '------';
    }
  }

  availabilityLabel(value: EmployeeAvailability): string {
    switch (value) {
      case EmployeeAvailability.Available:
        return 'distribution.availability.available';
      case EmployeeAvailability.OnLeave:
        return 'distribution.availability.leave';
      case EmployeeAvailability.Suspended:
        return 'distribution.availability.suspended';
      default:
        return 'distribution.availability.inactive';
    }
  }

  private handleResult(assigned: number, result: DistributionResult): void {
    this.successMessage.set(`تم توزيع ${assigned} ملف بنجاح.`);
    this.error.set(null);
    this.files.set(result.profiles);
    this.employees.set(result.employees);
    this.clearSelection();
  }

  private handleError(error: any, fallback: string): void {
    const message = error?.error ?? fallback;
    this.error.set(typeof message === 'string' ? message : fallback);
    this.successMessage.set(null);
  }

  protected readonly Number = Number;
}
