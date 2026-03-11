import {CommonModule} from '@angular/common';
import {Component, computed, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ToggleSwitchModule} from 'primeng/toggleswitch';
import {Tooltip} from 'primeng/tooltip';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {LanguagesService} from './services/languages.service';
import {LanguageDto} from './models/language.dto';
import {LanguageFilters} from './models/language-filters.dto';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {LanguageModalComponent} from './components/language-modal/language-modal.component';
import {finalize} from 'rxjs/operators';
import {debounceTime, distinctUntilChanged, Subject} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-languages-management',
  standalone: true,
  templateUrl: './languages-management.html',
  styleUrls: ['./languages-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    ToggleSwitchModule,
    Tooltip,
    LanguageModalComponent
  ]
})
export class LanguagesManagement implements OnInit {
  private languagesService = inject(LanguagesService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);

  private _languages = signal<LanguageDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public languages = this._languages.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<LanguageFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: ''
  });

  searchTerm = '';
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  isModalOpen = signal(false);
  modalMode = signal<'create' | 'edit'>('create');
  isModalLoading = signal(false);
  editingLanguage = signal<LanguageDto | null>(null);
  private searchChanges$ = new Subject<string>();

  ngOnInit(): void {
    this.setupSearchListener();
    this.loadLanguages();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadLanguages() {
    this.languagesService.getLanguages(this.filters()).subscribe({
      next: (response: PaginatedResult<LanguageDto>) => {
        this._languages.set(response.items || []);
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
    this.searchChanges$.next(this.searchTerm);
  }

  private setupSearchListener() {
    this.searchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(search => {
        this.filters.update(f => ({...f, pageNumber: 1, search}));
        this.loadLanguages();
      });
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.loadLanguages();
  }

  onPageSizeChange(size: number) {
    this.filters.update(f => ({
      ...f,
      pageNumber: 1,
      pageSize: size
    }));
    this.loadLanguages();
  }

  openAdd() {
    this.modalMode.set('create');
    this.editingLanguage.set(null);
    this.isModalLoading.set(false);
    this.isModalOpen.set(true);
  }

  openEdit(language: LanguageDto) {
    if (!language.id) {
      return;
    }
    this.modalMode.set('edit');
    this.fetchLanguageDetails(language.id);
  }

  private fetchLanguageDetails(id: string) {
    this.isModalLoading.set(true);
    this.languagesService
      .getLanguageDetails(id)
      .pipe(finalize(() => this.isModalLoading.set(false)))
      .subscribe({
        next: lang => {
          this.editingLanguage.set(lang);
          this.isModalOpen.set(true);
        }
      });
  }

  toggleStatus(language: LanguageDto) {
    if (!language.id) {
      return;
    }
    const desiredState = !language.isActive;
    this.languagesService.updateStatus(language.id, desiredState).subscribe({
      next: () => {
        this._languages.update(items =>
          items.map(l => (l.id === language.id ? {...l, isActive: desiredState} : l))
        );
        this.notification.success(
          this.translate.instant(desiredState ? 'LANGUAGES.ACTIVATE_SUCCESS' : 'LANGUAGES.DEACTIVATE_SUCCESS')
        );
      }
    });
  }

  createLanguage(payload: LanguageDto) {
    this.languagesService.createLanguage(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('LANGUAGES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadLanguages();
      }
    });
  }

  updateLanguage(payload: { id: string; payload: LanguageDto }) {
    this.languagesService.updateLanguage(payload.id, payload.payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('LANGUAGES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadLanguages();
      }
    });
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.modalMode.set('create');
    this.editingLanguage.set(null);
    this.isModalLoading.set(false);
  }
}
