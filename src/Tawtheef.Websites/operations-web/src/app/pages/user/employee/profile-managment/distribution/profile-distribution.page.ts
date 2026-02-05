import {CommonModule} from '@angular/common';
import {Component, DestroyRef, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {debounceTime, distinctUntilChanged, finalize, Subject} from 'rxjs';
import {TranslateModule, TranslateService} from '@ngx-translate/core';
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
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {NotificationService} from '../../../../../core/services/notification.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import { AssignFiles } from './dialogs/assign-files/assign-files';
import { ProgressBarModule } from 'primeng/progressbar';

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
    ProgressBarModule
  ],
  providers: [DialogService],
  templateUrl: './profile-distribution.page.html',
  styleUrl: './profile-distribution.page.scss',
})
export class ProfileDistributionPage implements OnInit {
  private api = inject(ProfileDistributionService);
  private dialogService = inject(DialogService);
  private authService = inject(AuthService);
  private notifications = inject(NotificationService);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  files = signal<DistributionFile[]>([]);
  employees = signal<DistributionEmployee[]>([]);
  targetEntities = signal<dropdownOptionsModel[]>([]);
  loading = signal(false);
  paginationMetadata = signal<PaginationMetadata | null>(null);

  statusFilter = signal<ProfileStatusNumber | 'all'>('all');
  search = signal('');
  targetEntityId = signal<string>('');
  selectedIds = signal<Set<string>>(new Set());
  pageNumber = signal(1);
  pageSize = signal(10);
  manualEmployeeId = signal<string>('');
  autoEmployeeIds = signal<Set<string>>(new Set());
  autoLimit = signal<number | null>(null);

  private searchChanges$ = new Subject<string>();

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
    this.setupSearchListener();
    this.loadData();
    this.loadTargetEntities();
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
    const targetEntityId = this.targetEntityId();
    if (targetEntityId) filters.targetEntityId = targetEntityId;

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

  loadTargetEntities(): void {
    this.api.getTargetEntities().subscribe({
      next: entities => this.targetEntities.set(entities ?? [])
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
    this.applySearch();
  }

  onSearchChange(value: string): void {
    this.search.set(value);
    this.searchChanges$.next(value);
  }

  onTargetEntityChange(value: string): void {
    this.targetEntityId.set(value);
    this.applySearch();
  }

  onSelectionChange(selection: DistributionFile[]): void {
    this.selectedIds.set(new Set(selection.map(item => item.profileId)));
  }

  clearSelection(): void {
    this.selectedIds.set(new Set());
  }

  clearSearch(): void {
    this.search.set('');
    this.statusFilter.set('all');
    this.targetEntityId.set('');
    this.applySearch();
  }

  applySearch(): void {
    this.pageNumber.set(1);
    this.clearSelection();
    this.loadData();
  }
  openManualDialog(profileId?: string): void {
    if (!this.canManageDistribution()) return;
    if (profileId) this.selectedIds.set(new Set([profileId]));
    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) {
      this.notifyNoSelection();
      return;
    }

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
 openAssignFilesDialog(): void {
  this.dialogService.open(AssignFiles, {
    header: 'تعيين الملفات على موظف',
      width: '720px',
      modal: true,

    })
 }

  openAutoDialog(): void {
    if (!this.canManageDistribution()) return;
    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) {
      this.notifyNoSelection();
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
      this.notifyNoSelection();
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

  private setupSearchListener(): void {
    this.searchChanges$
      .pipe(debounceTime(500), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applySearch());
  }

  private notifyNoSelection(): void {
    const message = this.translate.instant('distribution.errors.noProfilesSelected');
    this.notifications.warn(message);
  }

  protected readonly Number = Number;
}
