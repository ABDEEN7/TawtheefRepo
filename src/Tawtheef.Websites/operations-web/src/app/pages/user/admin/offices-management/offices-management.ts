import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ConfirmDialog} from 'primeng/confirmdialog';
import {ConfirmationService} from 'primeng/api';
import {Tooltip} from 'primeng/tooltip';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {OfficesService} from './services/offices.service';
import {OfficeDto} from './models/office.dto';
import {OfficeFilters} from './models/office-filters.dto';
import {UpdateOfficeRequest} from './models/update-office-request.dto';
import {CreateOfficeRequest} from './models/create-office-request.dto';
import {OfficeModalComponent} from './components/office-modal/office-modal.component';
import {OfficeDetailsDto} from './models/office-details.dto';
import {finalize} from 'rxjs/operators';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';

@Component({
  selector: 'app-offices-management',
  standalone: true,
  templateUrl: './offices-management.html',
  styleUrls: ['./offices-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    ConfirmDialog,
    Tooltip,
    OfficeModalComponent
  ],
  providers: [ConfirmationService]
})
export class OfficesManagement implements OnInit {
  private officesService = inject(OfficesService);
  private confirmationService = inject(ConfirmationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private notification = inject(NotificationService);

  // Component now manages its own state
  private _offices = signal<OfficeDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public offices = this._offices.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<OfficeFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: ''
  });

  searchTerm = '';
  countries = signal<dropdownOptionsModel[]>([]);
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  isModalOpen = signal(false);
  editingOffice = signal<OfficeDetailsDto | null>(null);
  modalMode = signal<'create' | 'edit' | 'view'>('create');
  isModalLoading = signal(false);

  ngOnInit(): void {
    this.loadOffices();
    this.loadCountries();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadOffices() {
    this.officesService.getOffices(this.filters()).subscribe({
      next: (response: PaginatedResult<OfficeDto>) => {
        this._offices.set(response.items || []);
        this._paginationMetadata.set(response.metadata);

        if (response.metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: response.metadata.currentPage,
            pageSize: response.metadata.pageSize
          }));
        }
      }
    });
  }

  loadCountries() {
    this.officesService.getCountries().subscribe({
      next: res => this.countries.set(res)
    });
  }

  onSearchChange() {
    this.filters.update(f => ({...f, pageNumber: 1, search: this.searchTerm}));
    this.loadOffices();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadOffices();
  }

  onPageSizeChange(size: number) {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      pageSize: size,
    }));
    this.loadOffices();
  }

  assignedCountryIds() {
    return this.offices()
      .map(o => o.country?.id)
      .filter(Boolean) as string[];
  }

  openAdd() {
    this.modalMode.set('create');
    this.editingOffice.set(null);
    this.isModalLoading.set(false);
    this.isModalOpen.set(true);
  }

  openView(office: OfficeDto) {
    this.modalMode.set('view');
    this.fetchOfficeDetails(office.id);
  }

  openEdit(office: OfficeDto) {
    this.modalMode.set('edit');
    this.fetchOfficeDetails(office.id);
  }

  private fetchOfficeDetails(id: string) {
    this.isModalLoading.set(true);
    this.officesService
      .getOfficeDetails(id)
      .pipe(finalize(() => this.isModalLoading.set(false)))
      .subscribe({
        next: details => {
          this.editingOffice.set(details);
          this.isModalOpen.set(true);
        }
      });
  }

  localizedOfficeName(office: OfficeDto) {
    return this.currentLang() === 'ar'
      ? office.nameAr || office.nameEn
      : office.nameEn || office.nameAr;
  }

  localizedCountry(nameAr: string, nameEn: string) {
    return this.currentLang() === 'ar'
      ? nameAr || nameEn
      : nameEn || nameAr;
  }

  supportedCountriesLabel(office: OfficeDto) {
    if (!office.supportedCountries?.length) {
      return [this.translate.instant('OFFICES.NO_SUPPORTED')];
    }

    return office.supportedCountries.map(sc => sc.name);
  }

  private refreshActiveOffice() {
    const activeOffice = this.editingOffice();
    if (!activeOffice) {
      return;
    }

    this.fetchOfficeDetails(activeOffice.id);
    this.loadOffices();
  }

  toggleUserBlockStatus(change: { userId: string; isBlocked: boolean }) {
    const activeOffice = this.editingOffice();
    if (!activeOffice) {
      return;
    }

    this.isModalLoading.set(true);
    this.officesService.updateOfficeUserBlockStatus(activeOffice.id, change.userId, change.isBlocked).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('OFFICES.BLOCK_STATUS_UPDATED'));
        this.refreshActiveOffice();
      },
      error: () => {
        this.isModalLoading.set(false);
      }
    });
  }

  moveOfficeAdmin(userId: string) {
    const activeOffice = this.editingOffice();
    if (!activeOffice) {
      return;
    }

    this.isModalLoading.set(true);
    this.officesService.setOfficeAdmin(activeOffice.id, userId).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('OFFICES.ADMIN_CHANGED'));
        this.refreshActiveOffice();
      },
      error: () => {
        this.isModalLoading.set(false);
      }
    });
  }

  createOffice(payload: CreateOfficeRequest) {
    if (this.assignedCountryIds().includes(payload.countryId)) {
      this.notification.error(this.translate.instant('OFFICES.COUNTRY_ALREADY_HAS_OFFICE'));
      return;
    }

    this.officesService.createOffice(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('OFFICES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadOffices();
      }
    });
  }

  updateOffice(payload: { id: string; payload: UpdateOfficeRequest }) {
    this.officesService.updateOffice(payload.id, payload.payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('OFFICES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadOffices();
      }
    });
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.modalMode.set('create');
    this.editingOffice.set(null);
    this.isModalLoading.set(false);
  }

  confirmDelete(office: OfficeDto) {
    this.confirmationService.confirm({
      message: this.translate.instant('OFFICES.DELETE_CONFIRM'),
      header: this.translate.instant('OFFICES.DELETE_HEADER'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('OFFICES.DELETE'),
      rejectLabel: this.translate.instant('OFFICES.CANCEL'),
      acceptButtonStyleClass: 'btn btn-danger',
      rejectButtonStyleClass: 'btn btn-outline-secondary',
      defaultFocus: 'reject',
      accept: () => {
        this.officesService.deleteOffice(office.id).subscribe({
          next: () => {
            this.notification.success(this.translate.instant('OFFICES.DELETE_SUCCESS'));
            // Remove office from local state
            this._offices.update(offices => offices.filter(o => o.id !== office.id));

            // Update pagination metadata
            this._paginationMetadata.update(metadata => {
              if (!metadata) return metadata;
              return {
                ...metadata,
                totalCount: Math.max(0, metadata.totalCount - 1),
                totalPages: Math.ceil(Math.max(0, metadata.totalCount - 1) / metadata.pageSize)
              };
            });
          }
        });
      }
    });
  }
}
