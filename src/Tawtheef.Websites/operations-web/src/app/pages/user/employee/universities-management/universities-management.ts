import {CommonModule} from '@angular/common';
import {Component, computed, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Select} from 'primeng/select';
import {ToggleSwitchModule} from 'primeng/toggleswitch';
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
import {debounceTime, distinctUntilChanged, Subject} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

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
    ToggleSwitchModule,
    Tooltip,
    UniversityModalComponent
  ]
})
export class UniversitiesManagement implements OnInit {
  private universitiesService = inject(UniversitiesService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);

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
  sortedCountries = computed(() => this.sortOptionsByCurrentLanguage(this.countries()));
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  isModalOpen = signal(false);
  modalMode = signal<'create' | 'edit'>('create');
  isModalLoading = signal(false);
  editingUniversity = signal<UniversityDto | null>(null);
  private searchChanges$ = new Subject<string>();

  ngOnInit(): void {
    this.setupSearchListener();
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
      }
    });
  }

  loadCountries() {
    this.universitiesService.getCountries().subscribe({
      next: res => this.countries.set(res)
    });
  }

  private sortOptionsByCurrentLanguage(options: dropdownOptionsModel[]): dropdownOptionsModel[] {
    const locale = this.currentLang() === 'ar' ? 'ar' : 'en';
    const collator = new Intl.Collator(locale, {numeric: true, sensitivity: 'base'});

    return [...options].sort((a, b) => collator.compare(this.getOptionDisplayName(a), this.getOptionDisplayName(b)));
  }

  private getOptionDisplayName(option: dropdownOptionsModel): string {
    const nameKey = this.currentLang() === 'ar' ? 'nameAr' : 'nameEn';
    const localizedName = option.additionalData?.[nameKey];

    return (typeof localizedName === 'string' && localizedName.trim())
      ? localizedName.trim()
      : option.name;
  }

  onSearchChange() {
    this.searchChanges$.next(this.searchTerm);
  }

  private setupSearchListener() {
    this.searchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(search => {
        this.filters.update(f => ({...f, pageNumber: 1, search}));
        this.loadUniversities();
      });
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
        }
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
      }
    });
  }

  createUniversity(payload: UniversityFormPayload) {
    this.universitiesService.createUniversity(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('UNIVERSITIES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadUniversities();
      }
    });
  }

  updateUniversity(payload: { id: string; payload: UniversityFormPayload }) {
    this.universitiesService.updateUniversity(payload.id, payload.payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('UNIVERSITIES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadUniversities();
      }
    });
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.modalMode.set('create');
    this.editingUniversity.set(null);
    this.isModalLoading.set(false);
  }
}
