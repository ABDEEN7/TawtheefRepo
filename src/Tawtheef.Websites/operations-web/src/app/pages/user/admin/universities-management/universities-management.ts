import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Select} from 'primeng/select';
import {Tooltip} from 'primeng/tooltip';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {UniversitiesService} from './services/universities.service';
import {UniversityDto} from './models/university.dto';
import {UniversityFilters} from './models/university-filters.dto';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {UniversityModalComponent} from './components/university-modal/university-modal.component';
import {finalize} from 'rxjs/operators';
import {UniversityFormPayload} from './models/university-form.payload';

@Component({
  selector: 'app-universities-management',
  standalone: true,
  templateUrl: './universities-management.html',
  styleUrls: ['./universities-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    Select,
    Tooltip,
    UniversityModalComponent
  ]
})
export class UniversitiesManagement implements OnInit {
  private universitiesService = inject(UniversitiesService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private _universities = signal<UniversityDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public universities = this._universities.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<UniversityFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: '',
    countryId: null
  });

  searchTerm = '';
  selectedCountry: string | null = null;
  countries = signal<dropdownOptionsModel[]>([]);
  itemsPerPageOptions = [10, 20, 50];

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  isModalOpen = signal(false);
  modalMode = signal<'create' | 'edit'>('create');
  isModalLoading = signal(false);
  editingUniversity = signal<UniversityDto | null>(null);

  ngOnInit(): void {
    this.loadUniversities();
    this.loadCountries();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadUniversities() {
    this.universitiesService.getUniversities(this.filters()).subscribe({
      next: (response: PaginatedResult<UniversityDto>) => {
        this._universities.set(response.items || []);
        this._paginationMetadata.set(response.metadata);

        if (response.metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: response.metadata.currentPage,
            pageSize: response.metadata.pageSize
          }));
        }
      },
      error: () => this.notification.error(this.translate.instant('UNIVERSITIES.LOAD_FAILED'))
    });
  }

  loadCountries() {
    this.universitiesService.getCountries().subscribe({
      next: res => this.countries.set(res),
      error: () => this.notification.error(this.translate.instant('UNIVERSITIES.LOAD_FAILED'))
    });
  }

  onSearchChange() {
    this.filters.update(f => ({...f, pageNumber: 1, search: this.searchTerm}));
    this.loadUniversities();
  }

  onCountryFilterChange(value: string | null) {
    this.selectedCountry = value;
    this.filters.update(f => ({...f, pageNumber: 1, countryId: value}));
    this.loadUniversities();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadUniversities();
  }

  onPageSizeChange(size: number) {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      pageSize: size
    }));
    this.loadUniversities();
  }

  openAdd() {
    this.modalMode.set('create');
    this.editingUniversity.set(null);
    this.isModalLoading.set(false);
    this.isModalOpen.set(true);
  }

  openEdit(university: UniversityDto) {
    if (!university.id) {
      return;
    }
    this.modalMode.set('edit');
    this.fetchUniversityDetails(university.id);
  }

  private fetchUniversityDetails(id: string) {
    this.isModalLoading.set(true);
    this.universitiesService
      .getUniversityDetails(id)
      .pipe(finalize(() => this.isModalLoading.set(false)))
      .subscribe({
        next: uni => {
          this.editingUniversity.set(uni);
          this.isModalOpen.set(true);
        },
        error: () => this.notification.error(this.translate.instant('UNIVERSITIES.DETAILS_LOAD_FAILED'))
      });
  }

  toggleStatus(university: UniversityDto) {
    if (!university.id) {
      return;
    }
    const desiredState = !university.isActive;
    this.universitiesService.updateStatus(university.id, desiredState).subscribe({
      next: () => {
        this._universities.update(items =>
          items.map(u => (u.id === university.id ? {...u, isActive: desiredState} : u))
        );
        this.notification.success(
          this.translate.instant(desiredState ? 'UNIVERSITIES.ACTIVATE_SUCCESS' : 'UNIVERSITIES.DEACTIVATE_SUCCESS')
        );
      },
      error: () => this.notification.error(
        this.translate.instant(desiredState ? 'UNIVERSITIES.ACTIVATE_FAILED' : 'UNIVERSITIES.DEACTIVATE_FAILED')
      )
    });
  }

  createUniversity(payload: UniversityFormPayload) {
    this.universitiesService.createUniversity(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('UNIVERSITIES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadUniversities();
      },
      error: () => this.notification.error(this.translate.instant('UNIVERSITIES.SAVE_FAILED'))
    });
  }

  updateUniversity(payload: { id: string; payload: UniversityFormPayload }) {
    this.universitiesService.updateUniversity(payload.id, payload.payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('UNIVERSITIES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadUniversities();
      },
      error: () => this.notification.error(this.translate.instant('UNIVERSITIES.SAVE_FAILED'))
    });
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.modalMode.set('create');
    this.editingUniversity.set(null);
    this.isModalLoading.set(false);
  }
}
