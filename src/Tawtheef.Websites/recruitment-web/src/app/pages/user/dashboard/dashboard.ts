import { Component, OnInit, signal, computed, inject, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import { Select } from 'primeng/select';
import { CandidateInvitationFilters } from './models/candidate-invitation-filters';
import { CandidateInvitationModel } from './models/candidate-invitation.model';
import { dropdownOptionsModel, DropdownOptionVM } from '../../../shared/models/dropdown-options.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { TableModule } from 'primeng/table';
import { routes } from '../../../routes/routes';
import { AuthService } from '../../../core/auth/auth.service';
import { GUID } from '../../../shared/types/guid.type';
import { ActionConfig } from './types/action-config.type';
import { STATUS_PILL_CLASSES, TYPE_BADGE_CLASSES, ACTION_CONFIGS } from './constants/constants';
import { CandidateDashboardService } from './services/candidate-dashboard.service';
import { InvitationStatus } from '../../../core/enums/lookups.enum';
import { ProfileViewCqrs } from '../profile/view/profile-view.cqrs';
import { rxResource } from '@angular/core/rxjs-interop';
import { AvatarUtils } from '../../../core/utils/avatar-utils';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    I18nNamespaceDirective,
    Select,
    PaginationComponent,
    TableModule,
    RouterLink
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class Dashboard implements OnInit {

 //private readonly languageService = Inject(LanguageService)
  private readonly profileCqrs = inject(ProfileViewCqrs)  // Re-use an exsisting profile data instead of duplicate services
  

  candidateService = inject(CandidateDashboardService);
  authService = inject(AuthService);
  translate = inject(TranslateService);
  routes = routes;

  // Loading states
  isRefreshing = signal(false);

  // Reactive signals

    private readonly profileBasic = rxResource ({
    params: () => true,
    stream: ()=> this.profileCqrs.basics(),
    })

  candidate = computed(() => {
    const p = this.profileBasic.value();
    if(!p) return null;
    const isAr = this.translate.getCurrentLang() === 'ar';
    return {
      fullName : isAr ?  p.fullNameAr : p.fullNameEn,
      photoUrl : p.avatar || AvatarUtils.build((isAr ?  p.fullNameAr : p.fullNameEn) ?? null),
      address: [
      p.address,
      p.naZone && (isAr ? `منطقة ${p.naZone}` : `Zone: ${p.naZone}`),
      p.naStreet && (isAr ? `شارع ${p.naStreet}` : `Street: ${p.naStreet}`),
      p.naBuilding && (isAr ? `مبنى ${p.naBuilding}` : `Building: ${p.naBuilding}`),
     // p.naUnit && (isAr ? `وحدة ${p.naUnit}` : `Unit: ${p.naUnit}`),
  ]
    .filter(Boolean)
    .join(' - '), // Joined with a dash (-) as in your example
      country: p.residenceCountry?.name,
      phone : p.phone,
      email : p.email     
    }
  });

 defaultAvatar = AvatarUtils.default;
  
  candidateInvitations = this.candidateService.candidateInvitations;
  paginationMetadata = this.candidateService.paginationMetadata;
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  totalPages = computed(() => this.paginationMetadata()?.totalPages || 0);

  // Filter signals
  currentPage = signal(1);
  itemsPerPage = signal(3);
  selectedCategory = signal<string>('');
  selectedDepartment = signal<string>('');
  selectedInvitationStatus = signal<string>('');
  jobTitle = signal<string>('');

  ngOnInit() {
    this.candidateService.loadCandidateLookups();
    this.candidateService.loadCandidateInvitationStatistics();
    this.loadCandidateInvitations();
    console.log('candidaaaaaaaat :' , this.candidate);
  }

  // Refresh data
  refreshData(): void {
    this.isRefreshing.set(true);
    this.loadCandidateInvitations()
  }

  // Filter methods

  loadCandidateInvitations() {
    const searchFilters: CandidateInvitationFilters = {
      jobCategoryId: this.selectedCategory() || '',
      departmentId: this.selectedDepartment() || '',
      invitationStatusId: this.selectedInvitationStatus() || '',
      jobTitle: this.jobTitle() || '',
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: 'CreatedDate',
      sortDirection: 'desc'
    }

    this.candidateService.loadCandidateInvitations(searchFilters);
  }

  // Clear all filters
  clearFilters(): void {
    this.selectedCategory.set('');
    this.selectedDepartment.set('');
    this.selectedInvitationStatus.set('');
    this.jobTitle.set('');
    this.currentPage.set(1);
  }

  getStatusClass(status: string): string {
    return STATUS_PILL_CLASSES[status as InvitationStatus] ?? 'neutral';
  }

  getJobCategoryClass(record: CandidateInvitationModel): string {
    return TYPE_BADGE_CLASSES[record.jobCategoryBackendName as keyof typeof TYPE_BADGE_CLASSES] ?? '';
  }

  getActionButtons(invitationStatus: DropdownOptionVM): ActionConfig {
    const status: InvitationStatus = invitationStatus.backendName as InvitationStatus;

    return ACTION_CONFIGS[status] ?? ACTION_CONFIGS[InvitationStatus.Closed];
  }

  getStatus(invitationStatus: DropdownOptionVM): string {
    return invitationStatus.name;
  }

  // Statistics helpers
  getReceivedCount(): number {
    return this.candidateService.invitationStatistics()?.received || 0;
  }

  getAcceptedCount(): number {
    return this.candidateService.invitationStatistics()?.accepted || 0;
  }

  getRejectedCount(): number {
    return this.candidateService.invitationStatistics()?.rejected || 0;
  }

  // Retry loading data
  retry(): void {
    this.loadCandidateInvitations();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadCandidateInvitations();
  }

  onFilterChange() {
    this.loadCandidateInvitations();
  }

  onPhotoError(event: Event): void {
  (event.target as HTMLImageElement).src = AvatarUtils.default;
  }
}
