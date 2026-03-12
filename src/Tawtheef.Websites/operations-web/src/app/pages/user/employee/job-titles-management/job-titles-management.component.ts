import {CommonModule} from '@angular/common';
import {Component, computed, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ConfirmationService} from 'primeng/api';
import {ConfirmDialog} from 'primeng/confirmdialog';
import {Tooltip} from 'primeng/tooltip';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NotificationService} from '../../../../core/services/notification.service';
import {JobTitleDto} from './models/job-title.dto';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {JobTitleFilters} from './models/job-title-filters.dto';
import {JobTitleModalComponent} from './components/job-title-modal/job-title-modal.component';
import {JobTitlesService} from './services/job-titles.service';
import {debounceTime, distinctUntilChanged, Subject} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-job-titles-management',
  standalone: true,
  templateUrl: './job-titles-management.component.html',
  styleUrls: ['./job-titles-management.component.scss'],
  providers: [ConfirmationService],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    ConfirmDialog,
    PaginationComponent,
    I18nNamespaceDirective,
    Tooltip,
    JobTitleModalComponent
  ]
})
export class JobTitlesManagementComponent implements OnInit {
  private jobTitlesService = inject(JobTitlesService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private confirmationService = inject(ConfirmationService);
  private destroyRef = inject(DestroyRef);

  private _allJobTitles = signal<JobTitleDto[]>([]);
  private _jobTitles = signal<JobTitleDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public jobTitles = this._jobTitles.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<JobTitleFilters>({
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
  editingJobTitle = signal<JobTitleDto | null>(null);
  private searchChanges$ = new Subject<string>();

  ngOnInit(): void {
    this.setupSearchListener();
    this.loadJobTitles();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadJobTitles() {
    this.jobTitlesService.getJobTitles().subscribe({
      next: response => {
        this._allJobTitles.set(response || []);
        this.applyFilters();
      }
    });
  }

  applyFilters() {
    const {pageNumber, pageSize, search} = this.filters();
    const normalizedSearch = (search || '').trim().toLowerCase();

    const filtered = normalizedSearch
      ? this._allJobTitles().filter(item =>
          item.jobNameAr.toLowerCase().includes(normalizedSearch) ||
          item.jobNameEn.toLowerCase().includes(normalizedSearch) ||
          item.jobNumber.toLowerCase().includes(normalizedSearch)
        )
      : this._allJobTitles();

    const totalCount = filtered.length;
    const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
    const safePage = Math.min(Math.max(1, pageNumber), totalPages);
    const start = (safePage - 1) * pageSize;
    const pagedItems = filtered.slice(start, start + pageSize);

    this._jobTitles.set(pagedItems);
    this._paginationMetadata.set({
      totalCount,
      pageSize,
      currentPage: safePage,
      totalPages,
      hasPreviousPage: safePage > 1,
      hasNext: safePage < totalPages
    });

    if (safePage !== pageNumber) {
      this.filters.update(f => ({...f, pageNumber: safePage}));
    }
  }

  onSearchChange() {
    this.searchChanges$.next(this.searchTerm);
  }

  private setupSearchListener() {
    this.searchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(search => {
        this.filters.update(f => ({...f, pageNumber: 1, search}));
        this.applyFilters();
      });
  }

  onPageChange(page: number) {
    this.filters.update(f => ({...f, pageNumber: page}));
    this.applyFilters();
  }

  onPageSizeChange(size: number) {
    this.filters.update(f => ({...f, pageNumber: 1, pageSize: size}));
    this.applyFilters();
  }

  openAdd() {
    this.modalMode.set('create');
    this.editingJobTitle.set(null);
    this.isModalOpen.set(true);
  }

  openEdit(jobTitle: JobTitleDto) {
    this.modalMode.set('edit');
    this.editingJobTitle.set(jobTitle);
    this.isModalOpen.set(true);
  }

  createJobTitle(payload: JobTitleDto) {
    this.jobTitlesService.createJobTitle(payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('JOB_TITLES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadJobTitles();
      }
    });
  }

  updateJobTitle(payload: {id: string; payload: JobTitleDto}) {
    this.jobTitlesService.updateJobTitle(payload.id, payload.payload).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('JOB_TITLES.SAVE_SUCCESS'));
        this.isModalOpen.set(false);
        this.loadJobTitles();
      }
    });
  }

  confirmDelete(jobTitle: JobTitleDto) {
    this.confirmationService.confirm({
      message: this.translate.instant('JOB_TITLES.DELETE_CONFIRM'),
      header: this.translate.instant('JOB_TITLES.DELETE_HEADER'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('JOB_TITLES.DELETE'),
      rejectLabel: this.translate.instant('JOB_TITLES.CANCEL'),
      acceptButtonStyleClass: 'btn btn-danger',
      rejectButtonStyleClass: 'btn btn-outline-secondary',
      defaultFocus: 'reject',
      accept: () => {
        this.jobTitlesService.deleteJobTitle(jobTitle.id!).subscribe({
          next: () => {
            this.notification.success(this.translate.instant('JOB_TITLES.DELETE_SUCCESS'));
            this.loadJobTitles();
          }
        });
      }
    });
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.modalMode.set('create');
    this.editingJobTitle.set(null);
  }
}
