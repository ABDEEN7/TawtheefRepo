import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import {TranslateModule, TranslateService} from '@ngx-translate/core';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';
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
  private translate = inject(TranslateService);
  private dialogService = inject(DialogService);
  private authService = inject(AuthService);

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

  readonly selectedFiles = computed(() =>
    this.files().filter(file => this.selectedIds().has(file.profileId))
  );

  readonly filteredFiles = computed(() => {
    const searchTerm = this.search().trim().toLowerCase();
    const status = this.statusFilter();

    return this.files().filter(file => {
      const matchesStatus = status === 'all' ? true : file.status === status;
      const matchesSearch =
        !searchTerm ||
        file.candidateName.toLowerCase().includes(searchTerm) ||
        (file.specialization ?? '').toLowerCase().includes(searchTerm) ||
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
        error: () => this.error.set(this.translate.instant('distribution.errors.loadFilesFailed')),
      });

    this.api.getEmployees().subscribe({
      next: employees => this.employees.set(employees),
      error: () => this.error.set(this.translate.instant('distribution.errors.loadEmployeesFailed')),
    });
  }

  onSelectionChange(selection: DistributionFile[]): void {
    this.selectedIds.set(new Set(selection.map(item => item.profileId)));
  }

  selectAll(): void {
    this.selectedIds.set(new Set(this.filteredFiles().map(f => f.profileId)));
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
      this.error.set(this.translate.instant('distribution.errors.noProfilesSelected'));
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
  private assignManual(payload: ManualAssignRequest): void {
    if (!this.canManageDistribution()) return;
    this.loading.set(true);
    this.api.assignManually(payload)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
        error: err => this.handleError(err, this.translate.instant('distribution.errors.assignFailed')),
      });
  }

  private assignAuto(payload: AutoAssignRequest): void {
    if (!this.canManageDistribution()) return;
    this.loading.set(true);
    this.api.assignAutomatically(payload)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
        error: err => this.handleError(err, this.translate.instant('distribution.errors.assignFailed')),
      });
  }

  redistribute(mode: 'manual' | 'auto'): void {
    if (!this.canManageDistribution()) return;
    if (this.selectedIds().size === 0) return;

    const payload: ReassignRequest = {
      mode,
      profileIds: Array.from(this.selectedIds()),
      employeeId: this.manualEmployeeId(),
      employeeIds: Array.from(this.autoEmployeeIds()),
      perEmployeeCount: this.autoLimit(),
    };

    this.loading.set(true);
    this.api.reassign(payload)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
        error: err => this.handleError(err, this.translate.instant('distribution.errors.reassignFailed')),
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

  canManageDistribution(): boolean {
    return this.authService.hasPermission(Permissions.ProfileDistribution.Manage);
  }

  protected readonly Number = Number;
}
