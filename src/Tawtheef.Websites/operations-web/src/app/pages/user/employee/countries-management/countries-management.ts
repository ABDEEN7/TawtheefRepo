import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { Tooltip } from 'primeng/tooltip';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { CountriesService } from './services/countries.service';
import { CitiesService } from './services/cities.service';
import { CountryDto, CountryVM } from './models/country.dto';
import { CityDto, CityVM } from './models/city.dto';
import { CountryFilters } from './models/country-filters.dto';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../core/services/notification.service';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../core/constants/permissions';

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
    ToggleSwitchModule,
    Tooltip,
    DialogModule,
    ButtonModule,
    HasPermissionDirective
  ],
})
export class CountriesManagement implements OnInit {
  private countriesService = inject(CountriesService);
  private citiesService = inject(CitiesService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);
  protected readonly Permissions = Permissions;

  private _countries = signal<CountryVM[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public countries = this._countries.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  // Cities related signals
  public showCitiesDialog = signal(false);
  public selectedCountryForCities = signal<CountryVM | null>(null);
  private _cities = signal<CityVM[]>([]);
  public cities = this._cities.asReadonly();
  public loadingCities = signal(false);

  // City Form related
  public showCityForm = signal(false);
  public editingCity = signal<CityVM | null>(null);
  public cityForm = {
    nameAr: '',
    nameEn: '',
    isActive: true
  };

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

  // Country Form related
  public showCountryForm = signal(false);
  public editingCountry = signal<CountryVM | null>(null);
  public countryForm = {
    nameAr: '',
    nameEn: '',
    code: '',
    isoCode: '',
    codeAlpha: '',
    isActive: true
  };

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  private searchChanges$ = new Subject<string>();

  ngOnInit(): void {
    this.setupSearchListener();
    this.loadCountries();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadCountries() {
    this.countriesService.getCountries(this.filters()).subscribe({
      next: (response: PaginatedResult<CountryDto>) => {
        const items = (response.items ?? [])
          .map((c: any) => new CountryVM({
            id: c.id,
            backendName: c.backendName,
            description: '',
            name: this.currentLang() == "ar" ? c.nameAr : c.nameEn,
            additionalData: {
              nameAr: c.nameAr,
              nameEn: c.nameEn,
              isoCode: c.isoCode,
              codeAlpha: c.codeAlpha
            },
            isActive: c.isActive,
            code: c.code
          }));

        this._countries.set(items);
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
    this.searchChanges$.next(this.nameFilter);
  }

  private setupSearchListener() {
    this.searchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(name => {
        this.filters.update(f => ({
          ...f,
          pageNumber: 1,
          name
        }));
        this.loadCountries();
      });
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
    this.filters.update(f => ({ ...f, pageNumber: page }));
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

  toggleStatus(country: CountryVM) {
    const desiredState = !country.isActive;
    this.countriesService.updateStatus(country.id, desiredState)
      .subscribe({
        next: () => {
          this._countries.update(items =>
            items.map(c => c.id === country.id ? new CountryVM(c, desiredState) : new CountryVM(c))
          );
          this.notification.success(this.translate.instant(desiredState ? 'COUNTRIES.ACTIVATE_SUCCESS' : 'COUNTRIES.DEACTIVATE_SUCCESS'));
        }
      });
  }

  // Country Form Methods
  openAddCountry() {
    this.editingCountry.set(null);
    this.countryForm = {
      nameAr: '',
      nameEn: '',
      code: '',
      isoCode: '',
      codeAlpha: '',
      isActive: true
    };
    this.showCountryForm.set(true);
  }

  openEditCountry(country: CountryVM) {
    this.editingCountry.set(country);
    this.countryForm = {
      nameAr: country.additionalData?.nameAr || '',
      nameEn: country.additionalData?.nameEn || '',
      code: country.code || '',
      isoCode: country.additionalData?.['isoCode'] as string || '',
      codeAlpha: country.additionalData?.['codeAlpha'] as string || '',
      isActive: country.isActive
    };
    this.showCountryForm.set(true);
  }

  saveCountry() {
    if (!this.countryForm.nameAr || !this.countryForm.nameEn || !this.countryForm.code || !this.countryForm.isoCode || !this.countryForm.codeAlpha) {
      this.notification.error(this.translate.instant('COUNTRIES.FILL_REQUIRED'));
      return;
    }

    if (this.editingCountry()) {
      this.countriesService.updateCountry(this.editingCountry()!.id, this.countryForm).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('COUNTRIES.UPDATE_SUCCESS'));
          this.loadCountries();
          this.showCountryForm.set(false);
        }
      });
    } else {
      this.countriesService.createCountry(this.countryForm).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('COUNTRIES.CREATE_SUCCESS'));
          this.loadCountries();
          this.showCountryForm.set(false);
        }
      });
    }
  }

  // City Management Methods
  openCitiesManagement(country: CountryVM) {
    this.selectedCountryForCities.set(country);
    this.loadCities(country.id);
    this.showCitiesDialog.set(true);
  }

  loadCities(countryId: string) {
    this.loadingCities.set(true);
    this.citiesService.getCities({ pageNumber: 1, pageSize: 100, countryId }).subscribe({
      next: (response) => {
        const items = (response.items ?? []).map(c => new CityVM({
          id: c.id,
          backendName: c.backendName,
          description: '',
          name: this.currentLang() === 'ar' ? c.additionalData?.nameAr : c.additionalData?.nameEn,
          additionalData: {
            nameAr: c.additionalData?.nameAr,
            nameEn: c.additionalData?.nameEn,
          },
          countryId: c.countryId,
          isActive: c.isActive,
          code: c.code
        } as any));
        this._cities.set(items);
        this.loadingCities.set(false);
      },
      error: () => this.loadingCities.set(false)
    });
  }

  openAddCity() {
    this.editingCity.set(null);
    this.cityForm = { nameAr: '', nameEn: '', isActive: true };
    this.showCityForm.set(true);
  }

  openEditCity(city: CityVM) {
    this.editingCity.set(city);
    this.cityForm = {
      nameAr: city.additionalData?.['nameAr'] || '',
      nameEn: city.additionalData?.['nameEn'] || '',
      isActive: city.isActive
    };
    this.showCityForm.set(true);
  }

  saveCity() {
    if (!this.cityForm.nameAr || !this.cityForm.nameEn) {
      this.notification.error(this.translate.instant('COUNTRIES.CITIES.FILL_REQUIRED'));
      return;
    }

    const countryId = this.selectedCountryForCities()?.id;
    if (!countryId) return;

    const cityData = {
      ...this.cityForm,
      countryId
    };

    if (this.editingCity()) {
      this.citiesService.updateCity(this.editingCity()!.id, cityData as any).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('COUNTRIES.CITIES.UPDATE_SUCCESS'));
          this.loadCities(countryId);
          this.showCityForm.set(false);
        }
      });
    } else {
      this.citiesService.createCity(cityData as any).subscribe({
        next: () => {
          this.notification.success(this.translate.instant('COUNTRIES.CITIES.CREATE_SUCCESS'));
          this.loadCities(countryId);
          this.showCityForm.set(false);
        }
      });
    }
  }

  toggleCityStatus(city: CityVM) {
    const desiredState = !city.isActive;
    this.citiesService.updateStatus(city.id, desiredState).subscribe({
      next: () => {
        this._cities.update(items =>
          items.map(c => c.id === city.id ? new CityVM(c as any, desiredState) : new CityVM(c as any))
        );
        this.notification.success(this.translate.instant(desiredState ? 'COUNTRIES.CITIES.ACTIVATE_SUCCESS' : 'COUNTRIES.CITIES.DEACTIVATE_SUCCESS'));
      }
    });
  }
}
