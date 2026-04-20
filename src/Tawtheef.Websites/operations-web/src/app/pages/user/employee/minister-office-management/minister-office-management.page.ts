import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, signal, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../core/services/notification.service';
import { MinisterOfficeService } from './services/minister-office.service';
import {
  MinisterOfficeCandidateDto,
  MinisterOfficeCandidateStatus,
  MinisterOfficeCandidateInvitationDto,
  MinisterOfficeCandidateAuditLogDto
} from './models/minister-office-candidate.model';
import { finalize, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { DatePickerModule } from 'primeng/datepicker';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { TooltipModule } from 'primeng/tooltip';
import { Permissions } from '../../../../core/constants/permissions';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { SelectModule } from 'primeng/select';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { DropdownOptionVM } from '../../../../shared/models/dropdown-options.model';
import { InvitationStatus } from '../../../../core/enums/lookups.enum';

export const JOB_INVITATION_STATUSES = {
  SUBMITTED: 'Submitted',
} as const;

@Component({
  selector: 'app-minister-office-management',
  standalone: true,
  templateUrl: './minister-office-management.page.html',
  styleUrls: ['./minister-office-management.page.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslateModule,
    I18nNamespaceDirective,
    ButtonModule,
    TableModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    TagModule,
    DialogModule,
    DatePickerModule,
    FloatLabelModule,
    InputGroupModule,
    InputGroupAddonModule,
    ToggleSwitchModule,
    TooltipModule,
    HasPermissionDirective,
    DatePipe,
    PaginationComponent,
    SelectModule
  ]
})
export class MinisterOfficeManagementPage implements OnInit {
  private service = inject(MinisterOfficeService);
  private notifications = inject(NotificationService);
  private language = inject(LanguageService);
  private translate = inject(TranslateService);

  readonly Permissions = Permissions;
  readonly CandidateStatus = MinisterOfficeCandidateStatus;

  // Table State
  items = signal<MinisterOfficeCandidateDto[]>([]);
  totalRecords = signal(0);
  loading = signal(false);
  searchControl = new FormControl('');
  includeInactiveControl = new FormControl(false);
  genderFilterControl = new FormControl<string | null>(null);
  targetEntityFilterControl = new FormControl<string | null>(null);
  candidateTypeFilterControl = new FormControl<string | null>(null);
  currentLang = signal<Lang>(this.language.get());
  showMoreFilters = signal(false);

  currentPage = 1;
  pageSize = 10;

  // Lookups
  genders = signal<any[]>([]);
  targetEntities = signal<any[]>([]);
  candidateTypes = signal<any[]>([]);

  // Dialogs Visibility
  showAddDialog = signal(false);
  showInvitationsDialog = signal(false);
  showAuditDialog = signal(false);
  showPhoneDialog = signal(false);

  // Forms
  addForm = new FormGroup({
    qid: new FormControl('', [Validators.required, Validators.pattern(/^\d{11}$/)]),
    phoneNumber: new FormControl('', [Validators.required, Validators.pattern(/^\d{8}$/)]),
    qidExpiryDate: new FormControl<Date | null>(null, Validators.required)
  });

  phoneForm = new FormGroup({
    candidateId: new FormControl(''),
    phoneNumber: new FormControl('', [Validators.required, Validators.pattern(/^\d{8}$/)])
  });

  // Selected State
  selectedCandidate = signal<MinisterOfficeCandidateDto | null>(null);
  invitations = signal<MinisterOfficeCandidateInvitationDto[]>([]);
  auditLogs = signal<MinisterOfficeCandidateAuditLogDto[]>([]);
  invitationsLoading = signal(false);
  auditLoading = signal(false);
  isSubmitting = signal(false);

  ngOnInit() {
    this.setupSearch();
    this.loadLookups();
    this.loadData();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));

  }

  private loadLookups() {
    this.service.getGenders().subscribe(res => this.genders.set(res));
    this.service.getTargetEntities().subscribe(res => this.targetEntities.set(res));
    this.service.getCandidateTypes().subscribe(res => this.candidateTypes.set(res));
  }

  private setupSearch() {
    this.searchControl.valueChanges
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe(() => {
        this.currentPage = 1;
        this.loadData();
      });

    this.includeInactiveControl.valueChanges.subscribe(() => {
      this.currentPage = 1;
      this.loadData();
    });

    this.genderFilterControl.valueChanges.subscribe(() => {
      this.currentPage = 1;
      this.loadData();
    });

    this.targetEntityFilterControl.valueChanges.subscribe(() => {
      this.currentPage = 1;
      this.loadData();
    });

    this.candidateTypeFilterControl.valueChanges.subscribe(() => {
      this.currentPage = 1;
      this.loadData();
    });
  }

  onPageChange(page: number) {
    this.currentPage = page;
    this.loadData();
  }

  onPageSizeChange(size: number) {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadData();
  }

  clearFilters() {
    this.searchControl.setValue('');
    this.genderFilterControl.setValue(null);
    this.targetEntityFilterControl.setValue(null);
    this.candidateTypeFilterControl.setValue(null);
    this.includeInactiveControl.setValue(false);
    this.currentPage = 1;
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.service.getCandidates({
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchControl.value || undefined,
      includeInactive: this.includeInactiveControl.value || undefined,
      genderId: this.genderFilterControl.value || undefined,
      targetEntityId: this.targetEntityFilterControl.value || undefined,
      candidateTypeId: this.candidateTypeFilterControl.value || undefined
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (res) => {
          this.items.set(res.items);
          this.totalRecords.set(res.metadata.totalCount);
        }
      });
  }

  openAddDialog() {
    this.addForm.reset({ phoneNumber: '' });
    this.showAddDialog.set(true);
  }

  onAddSubmit() {
    if (this.addForm.invalid) {
      this.addForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    const formVal = this.addForm.getRawValue();
    const expiryDate = new Date(formVal.qidExpiryDate!);
    expiryDate.setDate(expiryDate.getDate() + 1);
    const request = {
      qid: formVal.qid!,
      phoneNumber: '+974' + formVal.phoneNumber!,
      qidExpiryDate: expiryDate.toISOString().split('T')[0],
      preferredLanguage: this.language.get()
    };

    this.service.createCandidate(request)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success('Candidate registered successfully');
          this.showAddDialog.set(false);
          this.loadData();
        }
      });
  }

  toggleFollowUp(item: MinisterOfficeCandidateDto) {
    const nextState = !item.isFollowUpActive;
    this.service.updateFollowUpStatus(item.id, nextState).subscribe({
      next: () => {
        item.isFollowUpActive = nextState;
        //this.notifications.success(`Candidate follow-up ${nextState ? 'activated' : 'deactivated'}`);
        this.notifications.success(this.translate.instant(nextState ? 'ministerOffice.CANDIDATE_FOLLOW_UP_ACTIVATED' : 'ministerOffice.CANDIDATE_FOLLOW_UP_DEACTIVATED'));
      }
    });
  }

  openPhoneDialog(item: MinisterOfficeCandidateDto) {
    this.phoneForm.reset({
      candidateId: item.id,
      phoneNumber: item.phoneNumber.replace('+974', '')
    });
    this.showPhoneDialog.set(true);
  }

  onPhoneSubmit() {
    if (this.phoneForm.invalid) return;
    const { candidateId, phoneNumber } = this.phoneForm.getRawValue();
    this.isSubmitting.set(true);
    this.service.updatePhone(candidateId!, { phoneNumber: '+974' + phoneNumber! })
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success('Phone updated successfully');
          this.showPhoneDialog.set(false);
          this.loadData();
        }
      });
  }

  viewInvitations(item: MinisterOfficeCandidateDto) {
    this.selectedCandidate.set(item);
    this.invitations.set([]);
    this.invitationsLoading.set(true);
    this.showInvitationsDialog.set(true);

    this.service.getInvitations(item.id)
      .pipe(finalize(() => this.invitationsLoading.set(false)))
      .subscribe({
        next: (res) => {
          const mapped = res.map(inv => ({
            ...inv,
            invitationStatus: new DropdownOptionVM(inv.invitationStatus)
          }));
          this.invitations.set(mapped);
        }
      });
  }

  getStatus(invitationStatus: DropdownOptionVM): string {
    const status = invitationStatus.backendName as InvitationStatus;
    if (status == JOB_INVITATION_STATUSES.SUBMITTED) {
      return this.translate.instant('ministerOffice.JOB_INVITATION_STATUSES.SUBMITTED');
    }
    return invitationStatus.name;
  }

  viewAuditLog(item: MinisterOfficeCandidateDto) {
    this.selectedCandidate.set(item);
    this.auditLogs.set([]);
    this.auditLoading.set(true);
    this.showAuditDialog.set(true);

    this.service.getAuditLog(item.id, { pageNumber: 1, pageSize: 50 })
      .pipe(finalize(() => this.auditLoading.set(false)))
      .subscribe({
        next: (res) => this.auditLogs.set(res.items)
      });
  }

  getStatusSeverity(status: MinisterOfficeCandidateStatus): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    switch (status) {
      case MinisterOfficeCandidateStatus.ApprovedProfile:
      case MinisterOfficeCandidateStatus.InvitationsReceived:
        return 'success';
      case MinisterOfficeCandidateStatus.SubmittedForApproval:
        return 'info';
      case MinisterOfficeCandidateStatus.ReturnedForCorrection:
        return 'warn';
      case MinisterOfficeCandidateStatus.DraftProfile:
        return 'secondary';
      default:
        return 'secondary';
    }
  }

  getStatusLabel(status: MinisterOfficeCandidateStatus): string {
    const key = `ministerOffice.status.${MinisterOfficeCandidateStatus[status]}`;
    return key; // Translate pipe will handle it in HTML
  }
}
