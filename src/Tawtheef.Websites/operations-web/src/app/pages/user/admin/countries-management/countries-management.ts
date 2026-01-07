import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Select} from 'primeng/select';
import {Tooltip} from 'primeng/tooltip';
import {CountriesService} from './services/countries.service';
import {CountryDto} from './models/country.dto';
import {CountryFilters} from './models/country-filters.dto';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';

@Component({
  selector: 'app-countries-management',
  standalone: true,
  templateUrl: './countries-management.html',
  styleUrls: ['./countries-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    Select,
    Tooltip,
  ],
})
export class CountriesManagement implements OnInit {
  private countriesService = inject(CountriesService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private _countries = signal<CountryDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public countries = this._countries.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<CountryFilters>({
    pageNumber: 1,
    pageSize: 10,
    name: '',
    isActive: undefined
  });

  nameFilter = '';
  itemsPerPageOptions = [10, 20, 50];
  statusOptions = [
    { id: true, name: 'COUNTRIES.STATUS_ACTIVE' },
    { id: false, name: 'COUNTRIES.STATUS_INACTIVE' }
  ];

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  ngOnInit(): void {
    this.loadCountries();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadCountries() {
    this.countriesService.getCountries(this.filters()).subscribe({
      next: (response: PaginatedResult<CountryDto>) => {
        this._countries.set(response.items || []);
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

  onSearchChange() {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      name: this.nameFilter
    }));
    this.loadCountries();
  }

  onStatusChange(value: boolean | null) {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      isActive: value ?? null
    }));
    this.loadCountries();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadCountries();
  }

  onPageSizeChange(size: number) {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      pageSize: size
    }));
    this.loadCountries();
  }

  toggleStatus(country: CountryDto) {
    const desiredState = !country.isActive;
    this.countriesService.updateStatus(country.id, desiredState)
      .subscribe({
        next: () => {
          this._countries.update(items =>
            items.map(c => c.id === country.id ? { ...c, isActive: desiredState } : c)
          );
          this.notification.success(this.translate.instant(desiredState ? 'COUNTRIES.ACTIVATE_SUCCESS' : 'COUNTRIES.DEACTIVATE_SUCCESS'));
        }
      });
  }
}
