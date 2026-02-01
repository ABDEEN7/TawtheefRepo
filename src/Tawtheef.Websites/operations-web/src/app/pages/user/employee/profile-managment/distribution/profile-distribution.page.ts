import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {finalize} from 'rxjs';
import {TranslateModule} from '@ngx-translate/core';
import {TableModule} from 'primeng/table';
import {InputTextModule} from 'primeng/inputtext';
import {ButtonModule} from 'primeng/button';
import {TagModule} from 'primeng/tag';
import {DialogModule} from 'primeng/dialog';
import {InputNumberModule} from 'primeng/inputnumber';
import {AvatarModule} from 'primeng/avatar';
import {BadgeModule} from 'primeng/badge';
import {ProfileDistributionService} from './services/profile-distribution.service';
import {
  AutoAssignRequest,
  DistributionEmployee,
  DistributionFile,
  DistributionProfilesFilters,
  DistributionResult,
  EmployeeAvailability,
  ManualAssignRequest,
  ReassignRequest,
} from './models/profile-distribution.models';
import {Select} from 'primeng/select';
import {Ripple} from 'primeng/ripple';
import {Tooltip} from 'primeng/tooltip';
import {ToggleSwitch} from 'primeng/toggleswitch';
import {ProfileStatusNumber} from '../../../../../core/enums/lookups.enum';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {DistributionDialogResult} from './models/profile-distribution.dialogs';
import {ManualAssignDialog} from './dialogs/manual-assign-dialog/manual-assign-dialog';
import {AutoAssignDialog} from './dialogs/auto-assign-dialog/auto-assign-dialog';
import {DialogService} from 'primeng/dynamicdialog';
import {AuthService} from '../../../../../core/auth/auth.service';
import {Permissions} from '../../../../../core/constants/permissions';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {PaginationMetadata} from '../../../../../core/models/pagination-metadata.model';
import {PaginationComponent} from '../../../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-profile-distribution-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    TableModule,
    InputTextModule,
    ButtonModule,
    TagModule,
    DialogModule,
    InputNumberModule,
    AvatarModule,
    BadgeModule,
    PaginationComponent,
    I18nNamespaceDirective,
    Select,
    Ripple,
    Tooltip,
    ToggleSwitch,
  ],
  providers: [DialogService],
  templateUrl: './profile-distribution.page.html',
  styleUrl: './profile-distribution.page.scss',
})
export class ProfileDistributionPage implements OnInit {
  private api = inject(ProfileDistributionService);
  private dialogService = inject(DialogService);
  private authService = inject(AuthService);

  files = signal<DistributionFile[]>([]);
  employees = signal<DistributionEmployee[]>([]);
  loading = signal(false);
  paginationMetadata = signal<PaginationMetadata | null>(null);

  statusFilter = signal<ProfileStatusNumber | 'all'>('all');
  search = signal('');
  selectedIds = signal<Set<string>>(new Set());
  pageNumber = signal(1);
  pageSize = signal(10);

  manualEmployeeId = signal<string>('');
  autoEmployeeIds = signal<Set<string>>(new Set());
  autoLimit = signal<number | null>(null);
  private searchDebounce?: number;

  readonly selectedFiles = computed(() =>
    this.files().filter(file => this.selectedIds().has(file.profileId))
  );

  readonly displayedFiles = computed(() => this.files());

  readonly kpis = computed(() => {
    const displayedFiles = this.displayedFiles();
    const totalCount = this.paginationMetadata()?.totalCount ?? this.files().length;

    return {
      total: totalCount,
      filtered: totalCount,
      submitted: displayedFiles.filter(file => file.status === ProfileStatusNumber.Submitted).length,
      underReview: displayedFiles.filter(file => file.status === ProfileStatusNumber.UnderReview).length,
      needsChanges: displayedFiles.filter(file => file.status === ProfileStatusNumber.RequiresUpdate)
        .length,
    };
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

  readonly rowsPerPageOptions = [10, 20, 50];

  protected readonly EmployeeAvailability = EmployeeAvailability;

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    const filters: DistributionProfilesFilters = {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
    const status = this.statusFilter();
    if (status !== 'all') filters.status = status;
    const searchTerm = this.search().trim();
    if (searchTerm) filters.searchTerm = searchTerm;

    this.api
      .getFiles(filters)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response: PaginatedResult<DistributionFile>) => {
          this.files.set(response.items);
          this.paginationMetadata.set(response.metadata);
          this.pageNumber.set(response.metadata.currentPage);
          this.pageSize.set(response.metadata.pageSize);
        }
      });

    this.api.getEmployees().subscribe({
      next: employees => this.employees.set(employees)
    });
  }

  onPageChange(page: number): void {
    this.pageNumber.set(page);
    this.loadData();
  }

  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadData();
  }

  onStatusChange(value: ProfileStatusNumber | 'all'): void {
    this.statusFilter.set(value);
    this.pageNumber.set(1);
    this.loadData();
  }

  onSearchChange(value: string): void {
    this.search.set(value);
    if (this.searchDebounce) {
      window.clearTimeout(this.searchDebounce);
    }
    this.searchDebounce = window.setTimeout(() => {
      this.pageNumber.set(1);
      this.clearSelection();
      this.loadData();
    }, 400);
  }

  onSelectionChange(selection: DistributionFile[]): void {
    this.selectedIds.set(new Set(selection.map(item => item.profileId)));
  }

  selectAll(): void {
    this.selectedIds.set(new Set(this.displayedFiles().map(f => f.profileId)));
  }

  clearSelection(): void {
    this.selectedIds.set(new Set());
  }
  openManualDialog(profileId?: string): void {
    if (!this.canManageDistribution()) return;
    if (profileId) this.selectedIds.set(new Set([profileId]));
    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) return;

    this.dialogService.open(ManualAssignDialog, {
      header: 'distribution.dialog.manual.title',
      width: '520px',
      modal: true,
      dismissableMask: false,
      data: {
        employees: this.employees(),
        selectedProfileIds: ids,
        initialEmployeeId: this.manualEmployeeId() || null,
      },
    })?.onClose.subscribe((res: DistributionDialogResult) => {
      if (!res || res.kind !== 'manual') return;
      this.assignManual(res.payload);
    });
  }

  openAutoDialog(): void {
    if (!this.canManageDistribution()) return;
    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) {
      return;
    }
    this.dialogService.open(AutoAssignDialog, {
      header: 'distribution.dialog.auto.title',
      width: '640px',
      modal: true,
      dismissableMask: false,
      data: {
        employees: this.employees(),
        selectedProfileIds: ids,
        initialLimit: this.autoLimit(),
        initialEmployeeIds: Array.from(this.autoEmployeeIds()),
      },
    })?.onClose.subscribe((res: DistributionDialogResult) => {
      if (!res || res.kind !== 'auto') return;
      this.assignAuto(res.payload);
    });
  }

  openReassignDialog(mode: 'manual' | 'auto', profileId?: string): void {
    if (!this.canManageDistribution()) return;
    if (profileId) this.selectedIds.set(new Set([profileId]));
    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) {
      return;
    }

    if (mode === 'manual') {
      this.dialogService.open(ManualAssignDialog, {
        header: 'distribution.dialog.manual.title',
        width: '520px',
        modal: true,
        dismissableMask: false,
        data: {
          employees: this.employees(),
          selectedProfileIds: ids,
          initialEmployeeId: this.manualEmployeeId() || null,
        },
      })?.onClose.subscribe((res: DistributionDialogResult) => {
        if (!res || res.kind !== 'manual') return;
        this.reassignManual(res.payload);
      });
      return;
    }

    this.dialogService.open(AutoAssignDialog, {
      header: 'distribution.dialog.auto.title',
      width: '640px',
      modal: true,
      dismissableMask: false,
      data: {
        employees: this.employees(),
        selectedProfileIds: ids,
        initialLimit: this.autoLimit(),
        initialEmployeeIds: Array.from(this.autoEmployeeIds()),
      },
    })?.onClose.subscribe((res: DistributionDialogResult) => {
      if (!res || res.kind !== 'auto') return;
      this.reassignAuto(res.payload);
    });
  }

  private assignManual(payload: ManualAssignRequest): void {
    if (!this.canManageDistribution()) return;
    this.loading.set(true);
    this.api.assignManually(payload)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result)
      });
  }

  private assignAuto(payload: AutoAssignRequest): void {
    if (!this.canManageDistribution()) return;
    this.loading.set(true);
    this.api.assignAutomatically(payload)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result)
      });
  }

  private reassignManual(payload: ManualAssignRequest): void {
    if (!this.canManageDistribution()) return;
    const request: ReassignRequest = {
      mode: 'manual',
      profileIds: payload.profileIds,
      employeeId: payload.employeeId,
    };
    this.loading.set(true);
    this.api.reassign(request)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result)
      });
  }

  private reassignAuto(payload: AutoAssignRequest): void {
    if (!this.canManageDistribution()) return;
    const request: ReassignRequest = {
      mode: 'auto',
      profileIds: payload.profileIds ?? [],
      employeeIds: payload.employeeIds,
      perEmployeeCount: payload.perEmployeeCount,
    };
    this.loading.set(true);
    this.api.reassign(request)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result)
      });
  }


  toggleEmployee(employeeId: string): void {
    const set = new Set(this.autoEmployeeIds());
    set.has(employeeId) ? set.delete(employeeId) : set.add(employeeId);
    this.autoEmployeeIds.set(set);
  }

  statusSeverity(status: ProfileStatusNumber): 'info' | 'warn' | 'success' | 'danger' | 'secondary' {
    switch (status) {
      case ProfileStatusNumber.UnderReview:
        return 'info';
      case ProfileStatusNumber.RequiresUpdate:
        return 'warn';
      case ProfileStatusNumber.Approved:
        return 'success';
      case ProfileStatusNumber.Rejected:
      case ProfileStatusNumber.Cancelled:
        return 'danger';
      default:
        return 'secondary';
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
    this.employees.set(result.employees);
    this.clearSelection();
    this.loadData();
  }

  canManageDistribution(): boolean {
    return this.authService.hasPermission(Permissions.ProfileDistribution.Manage);
  }

  protected readonly Number = Number;
}
