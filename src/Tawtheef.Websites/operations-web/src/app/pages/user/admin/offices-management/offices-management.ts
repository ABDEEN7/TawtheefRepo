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
import {CountryLookupDto} from './models/country-lookup.dto';
import {OfficeModalComponent} from './components/office-modal/office-modal.component';

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

  offices = this.officesService.offices;
  paginationMetadata = this.officesService.paginationMetadata;

  filters = signal<OfficeFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: ''
  });

  searchTerm = '';
  countries = signal<CountryLookupDto[]>([]);
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  isModalOpen = signal(false);
  editingOffice = signal<OfficeDto | null>(null);

  ngOnInit(): void {
    this.loadOffices();
    this.loadCountries();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadOffices() {
    this.officesService.getOffices(this.filters()).subscribe({
      next: () => {
        const metadata = this.paginationMetadata();
        if (metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: metadata.currentPage,
            pageSize: metadata.pageSize
          }));
        }
      },
      error: () => this.notification.error(this.translate.instant('OFFICES.LOAD_FAILED'))
    });
  }

  loadCountries() {
    this.officesService.getCountries().subscribe({
      next: res => this.countries.set(res),
      error: () => this.notification.error(this.translate.instant('OFFICES.LOAD_FAILED'))
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

  openAdd() {
    this.editingOffice.set(null);
    this.isModalOpen.set(true);
  }

  openEdit(office: OfficeDto) {
    this.editingOffice.set(office);
    this.isModalOpen.set(true);
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

    return office.supportedCountries.map(sc => this.localizedCountry(sc.nameAr, sc.nameEn));
  }

  createOffice(payload: CreateOfficeRequest) {
    this.officesService.createOffice(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('OFFICES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadOffices();
      },
      error: () => this.notification.error(this.translate.instant('OFFICES.SAVE_FAILED'))
    });
  }

  updateOffice(payload: { id: string; payload: UpdateOfficeRequest }) {
    this.officesService.updateOffice(payload.id, payload.payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('OFFICES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadOffices();
      },
      error: () => this.notification.error(this.translate.instant('OFFICES.SAVE_FAILED'))
    });
  }

  closeModal() {
    this.isModalOpen.set(false);
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
            this.loadOffices();
          },
          error: () => this.notification.error(this.translate.instant('OFFICES.DELETE_FAILED'))
        });
      }
    });
  }
}
